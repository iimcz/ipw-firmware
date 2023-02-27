using emt_sdk.Settings;
using emt_sdk.ScenePackage;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Assets.Extensions;
using System.Linq;
using System.Net;
using emt_sdk.Settings.IPW;

public class DualCameraComponent : MonoBehaviour, ICameraRig
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public TransformCameraComponent TopCamera;
    public TransformCameraComponent BottomCamera;
    public IPWSetting.IPWOrientation Orientation { get; set; }

    public IPWSetting Setting;

    emt_sdk.Settings.EMT.DeviceTypeEnum ICameraRig.DeviceType => emt_sdk.Settings.EMT.DeviceTypeEnum.DEVICE_TYPE_IPW;

    public emt_sdk.Packages.CanvasDimensions DefaultCanvasDimensions => Orientation switch
    {
        IPWSetting.IPWOrientation.Horizontal => new emt_sdk.Packages.CanvasDimensions { Width = 4096, Height = 2048 },
        IPWSetting.IPWOrientation.Vertical => new emt_sdk.Packages.CanvasDimensions { Width = 2048, Height = 4096 },
        IPWSetting.IPWOrientation.Single => new emt_sdk.Packages.CanvasDimensions { Width = 2048, Height = 2048 },
        _ => throw new NotImplementedException()
    };

    public Vector2 CanvasDimensions { get; private set; }
    public Viewport Viewport { get; private set; }

    // Extra lens shift defined by Sync offset
    private Vector2 _syncLensShift;
    private IConfigurationProvider<IPWSetting> _settingsProvider;
    private IConfigurationProvider<emt_sdk.Packages.PackageDescriptor> _packageProvider;

    public Vector2 SyncLensShift
    {
        get => _syncLensShift;
        set
        {
            TopCamera.Camera.lensShift -= _syncLensShift;
            BottomCamera.Camera.lensShift -= _syncLensShift;

            // We shift each camere individually so the shift is half of what is expected
            _syncLensShift = value * 2f;

            TopCamera.Camera.lensShift += _syncLensShift;
            BottomCamera.Camera.lensShift += _syncLensShift;
        }
    }

    // TODO: Fix ortho sizing, lens shift is just physical worldspace shift scaled by orthosize
    // TODO: Disable culling on ortho scenes, leaves black squares for some reason

    void Awake()
    {
        _settingsProvider = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<IPWSetting>>();
        _packageProvider = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<emt_sdk.Packages.PackageDescriptor>>();

        for (int i = 1; i < Display.displays.Length; i++)
        {
            if (Display.displays[i].active) continue;
            Display.displays[i].Activate();
            Display.displays[i].SetRenderingResolution(2048, 2048);
        }

        LoadSettings();
        StartCoroutine(ApplyDelay());
    }

    private IEnumerator ApplyDelay()
    {
        yield return new WaitForEndOfFrame();
        ApplySettings();
    }

    public void LoadSettings()
    {
        Setting = _settingsProvider.Configuration;

        TopCamera.Settings = Setting.Displays;
        BottomCamera.Settings = Setting.Displays;
        Orientation = Setting.Orientation;
    }

    public Rect GetBoundaries(float? distance = null)
    {
        if (Setting.Orientation == IPWSetting.IPWOrientation.Single)
            return distance.HasValue ? TopCamera.GetCameraBoundries(distance.Value) : TopCamera.GetCameraBoundries();
        
        // Right in horizontal
        var topBoundary = distance.HasValue ? 
            TopCamera.GetCameraBoundries(distance.Value) : 
            TopCamera.GetCameraBoundries();
        
        // Left in horizontal
        var bottomBoundary = distance.HasValue ? 
            BottomCamera.GetCameraBoundries(distance.Value) : 
            BottomCamera.GetCameraBoundries();

        Vector2 size;
        switch (Setting.Orientation)
        {
            case IPWSetting.IPWOrientation.Vertical:
                size = new Vector2(bottomBoundary.width, topBoundary.yMax - bottomBoundary.yMin);
                return new Rect(topBoundary.x, bottomBoundary.yMin, size.x, size.y);
            case IPWSetting.IPWOrientation.Horizontal:
                size = new Vector2(topBoundary.xMax - bottomBoundary.x, bottomBoundary.height);
                return new Rect(bottomBoundary.x, bottomBoundary.y, size.x, size.y);
            default:
                throw new NotSupportedException();
        }
    }

    public void ApplySettings()
    {
        var canvasDimensions = _packageProvider.Configuration?.Sync?.CanvasDimensions ?? DefaultCanvasDimensions;
        var canvasDimensionVector = new Vector2((int)DefaultCanvasDimensions.Width.Value, (int)DefaultCanvasDimensions.Height.Value);

        if (_packageProvider.Configuration != null)
        {
            if (_packageProvider.Configuration.Sync.CanvasDimensions == null)
            {
                Logger.Info("Loaded a package without sync info, using default canvas size with no shift");
                SetViewport(canvasDimensionVector, ((ICameraRig)this).DefaultViewport);
            }
            else
            {
                var selfElement = _packageProvider.Configuration.Sync.Elements.First(e => e.Hostname == Dns.GetHostName());
                SetViewport(new Vector2(canvasDimensions.Width.Value, canvasDimensions.Height.Value), selfElement.Viewport);
            }
        }
        else
        {
            // Debug mode
            SetViewport(canvasDimensionVector, ((ICameraRig)this).DefaultViewport);
        }

        switch (Setting.Orientation)
        {
            case IPWSetting.IPWOrientation.Vertical:
                TopCamera.Camera.lensShift = new Vector2(0, Setting.LensShift);
                BottomCamera.Camera.lensShift = new Vector2(0, -Setting.LensShift);
                
                TopCamera.Camera.gateFit = Camera.GateFitMode.Vertical;
                BottomCamera.Camera.gateFit = Camera.GateFitMode.Vertical;
                break;
            case IPWSetting.IPWOrientation.Horizontal:
                TopCamera.Camera.lensShift = new Vector2(Setting.LensShift, 0);
                BottomCamera.Camera.lensShift = new Vector2(-Setting.LensShift, 0);
                break;
            case IPWSetting.IPWOrientation.Single:
                TopCamera.Camera.lensShift = Vector2.zero;
                BottomCamera.Camera.lensShift = Vector2.zero;
                break;
            default:
                throw new NotImplementedException();
        }

        if (TopCamera.Camera.orthographic)
        {
            TopCamera.gameObject.transform.position = new Vector3(TopCamera.Camera.orthographicSize - Setting.LensShift, 0, 0);
            BottomCamera.gameObject.transform.position = new Vector3(-BottomCamera.Camera.orthographicSize + Setting.LensShift, 0, 0);
        }

        ProjectorTransformationPass.Vertical = Setting.Orientation == IPWSetting.IPWOrientation.Vertical;

        // Apply Sync viewport transformation
        TopCamera.Camera.lensShift += _syncLensShift;
        BottomCamera.Camera.lensShift += _syncLensShift;

        TopCamera.ApplySettings();
        BottomCamera.ApplySettings();
    }

    public void SaveSettings()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configFile = Path.Combine(userFolder, "ipw.json");
        var json = JsonConvert.SerializeObject(Setting, Formatting.Indented);

        if (File.Exists(configFile))
        {
            var fileNameFriendlyDate = DateTime.Now.ToString("s").Replace(":", "");
            var backupPath = Path.Combine(userFolder, $"ipw_{fileNameFriendlyDate}.json");
            File.Move(configFile, backupPath);
        }

        File.WriteAllText(configFile, json);
        Logger.InfoUnity("Configuration saved");
    }

    public void SwapSettings()
    {
        (TopCamera.SettingIndex, BottomCamera.SettingIndex) = (BottomCamera.SettingIndex, TopCamera.SettingIndex);
    }

    public void SwapDisplays()
    {
        var firstDisplay = TopCamera.TargetDisplay;
        var secondDisplay = BottomCamera.TargetDisplay;

        TopCamera.TargetDisplay = secondDisplay;
        BottomCamera.TargetDisplay = firstDisplay;
    }

    public void SetBackgroundColor(Color color)
    {
        TopCamera.Camera.clearFlags = CameraClearFlags.SolidColor;
        TopCamera.Camera.backgroundColor = color;

        BottomCamera.Camera.clearFlags = CameraClearFlags.SolidColor;
        BottomCamera.Camera.backgroundColor = color;
    }

    public void ShowSkybox()
    {
        TopCamera.Camera.clearFlags = CameraClearFlags.Skybox;
        BottomCamera.Camera.clearFlags = CameraClearFlags.Skybox;
    }

    public void SetViewport(Vector2 canvasSize, Viewport viewport)
    {
        Viewport = viewport;
        CanvasDimensions = canvasSize;

        var expectedResolution = Orientation switch
        {
            IPWSetting.IPWOrientation.Vertical => new Vector2(2048, 4096),
            IPWSetting.IPWOrientation.Horizontal => new Vector2(4096, 2048),
            IPWSetting.IPWOrientation.Single => new Vector2(2048, 2048),
            _ => throw new NotImplementedException()
        };

        if (expectedResolution != new Vector2(viewport.Width, viewport.Height))
        {
            Logger.ErrorUnity($"Attempted to set viewport size of an IPW camera to something other than {expectedResolution.x}x{expectedResolution.y} (tried to use {viewport.Width}x{viewport.Height}), this will not work");
            return;
        }

        _syncLensShift = Orientation switch
        {
            IPWSetting.IPWOrientation.Vertical => new Vector2(viewport.X / 2048f, viewport.Y / 4096f),
            IPWSetting.IPWOrientation.Horizontal => new Vector2(viewport.X / 4096f, viewport.Y / 2048f),
            IPWSetting.IPWOrientation.Single => new Vector2(viewport.X / 2048f, viewport.Y / 2048f),
            _ => throw new NotImplementedException()
        };
    }
}
