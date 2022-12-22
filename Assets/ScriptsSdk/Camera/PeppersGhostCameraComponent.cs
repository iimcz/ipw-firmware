using emt_sdk.Settings;
using emt_sdk.ScenePackage;
using UnityEngine;
using System.Linq;
using Assets.Extensions;
using System.Net;
using System.Collections;

public class PeppersGhostCameraComponent : MonoBehaviour, ICameraRig
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    Naki3D.Common.Protocol.DeviceType ICameraRig.DeviceType => Naki3D.Common.Protocol.DeviceType.Pge;

    public IPWSetting.IPWOrientation Orientation => IPWSetting.IPWOrientation.Single;

    public emt_sdk.Generated.ScenePackage.CanvasDimensions DefaultCanvasDimensions => new emt_sdk.Generated.ScenePackage.CanvasDimensions { Width = 2048, Height = 2048 };

    public Vector2 CanvasDimensions { get; private set; }
    public Viewport Viewport { get; private set; }

    public Camera Camera;

    // Extra lens shift defined by Sync offset
    private Vector2 _syncLensShift;
    public Vector2 SyncLensShift
    {
        get => _syncLensShift;
        set
        {
            Camera.lensShift -= _syncLensShift;
            _syncLensShift = value;
            Camera.lensShift += _syncLensShift;
        }
    }

    void Awake()
    {
        Display.displays[0].SetRenderingResolution(2048, 2048);
        Camera.aspect = 1;

        StartCoroutine(ApplyDelay());
    }

    private IEnumerator ApplyDelay()
    {
        yield return new WaitForEndOfFrame();
        ApplySettings();
    }

    public Rect GetBoundaries(float? distance = null)
    {
        var dist = distance ?? Camera.nearClipPlane;
        var bottomLeft = Camera.ViewportToWorldPoint(new Vector3(0, 0, dist));
        var topRight = Camera.ViewportToWorldPoint(new Vector3(1, 1, dist));

        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    public void ApplySettings()
    {
        // Sync transform for feature parity with IPW
        var canvasDimensions = ExhibitConnectionComponent.ActivePackage?.Sync?.CanvasDimensions ?? DefaultCanvasDimensions;
        var canvasDimensionVector = new Vector2((int)DefaultCanvasDimensions.Width.Value, (int)DefaultCanvasDimensions.Height.Value);

        if (ExhibitConnectionComponent.ActivePackage != null)
        {
            if (ExhibitConnectionComponent.ActivePackage.Sync.CanvasDimensions == null)
            {
                Logger.Info("Loaded a package without sync info, using default canvas size with no shift");
                SetViewport(canvasDimensionVector, ((ICameraRig)this).DefaultViewport);
            }
            else
            {
                var selfElement = ExhibitConnectionComponent.ActivePackage.Sync.Elements.First(e => e.Hostname == Dns.GetHostName());
                SetViewport(new Vector2(canvasDimensions.Width.Value, canvasDimensions.Height.Value), selfElement.Viewport);
            }
        }
        else
        {
            // Debug mode
            SetViewport(canvasDimensionVector, ((ICameraRig)this).DefaultViewport);
        }

        Camera.lensShift = _syncLensShift;
    }

    public void SaveSettings()
    {
        // Nothing to save
    }

    public void SetBackgroundColor(Color color)
    {
        Camera.clearFlags = CameraClearFlags.SolidColor;
        Camera.backgroundColor = color;
    }

    public void ShowSkybox()
    {
        Camera.clearFlags = CameraClearFlags.Skybox;
    }

    public void SetViewport(Vector2 canvasSize, Viewport viewport)
    {
        Viewport = viewport;
        CanvasDimensions = canvasSize;

        var expectedResolution = new Vector2(2048, 2048);
        if (expectedResolution != new Vector2(viewport.Width, viewport.Height))
        {
            Logger.ErrorUnity($"Attempted to set viewport size of an PGE camera to something other than {expectedResolution.x}x{expectedResolution.y} (tried to use {viewport.Width}x{viewport.Height}), this will not work");
            return;
        }

        _syncLensShift = new Vector2(viewport.X / 2048f, viewport.Y / 2048f);
    }
}
