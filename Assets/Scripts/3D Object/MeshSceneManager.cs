using System;
using System.IO;
using System.Collections;
using System.Linq;
using Assets.ScriptsSdk.Extensions;
using emt_sdk.Scene;
using emt_sdk.Packages;
using Siccity.GLTFUtility;
using Naki3D.Common.Protocol;
using UnityEngine;
using emt_sdk.Settings;

/// <summary>
/// Handles loading 3D object data, scene setup and event handling
/// </summary>
public class MeshSceneManager : MonoBehaviour
{
    [SerializeField]
    private FlagNavigator _flagNavigator;

    [SerializeField] 
    private OrbitComponent _cameraOrbit;

    [SerializeField]
    private CameraHandMovement _handMovement;

    [SerializeField]
    private CameraRigSpawnerComponent _rigSpawner;

    [SerializeField]
    private Ntp3DObjectSyncComponent _ntpSync;

    [SerializeField]
    private ModelVisibilityComponent _modelVisibility;

    private IConfigurationProvider<PackageDescriptor> _packageProvider;

    private void Start()
    {
        _packageProvider = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<PackageDescriptor>>();
        StartCoroutine(DelayApply());
    }

    public void Apply(GltfObject scene, string basePath)
    {
        if (ColorUtility.TryParseHtmlString(scene.SkyboxTint ?? "#FFFFFF", out var tint) == false)
        {
            throw new ArgumentException("Background color is not a valid HTML hex color string", nameof(scene.SkyboxTint));
        }

        if (!string.IsNullOrEmpty(scene.Skybox))
        {
            _rigSpawner.CameraRig.ShowSkybox();
            var skyboxPath = Path.Combine(basePath, scene.Skybox);
            SkyboxLoader.ApplySkybox(skyboxPath, tint);
        }
        else
        {
            _rigSpawner.CameraRig.SetBackgroundColor(tint);
            SkyboxLoader.ApplyTint(tint);
            SkyboxLoader.ApplyAmbientLight(tint);
        }

        switch (scene.CameraAnimation)
        {
            case GltfObject.OrbitAnimation orbit:

                // TODO: Figure out limits for height != 0 and/or add to JSON
                _cameraOrbit.LongitudeMin = -85;
                _cameraOrbit.LongitudeMax = 85;

                _cameraOrbit.Origin = orbit.Origin.FindPosition();
                
                _cameraOrbit.LookAt = orbit.LookAt.FindObject();
                if (_cameraOrbit.LookAt == null && orbit.LookAt.Offset != null)
                {
                    var lookAtObj = new GameObject("Orbit - Look at");
                    lookAtObj.transform.position = orbit.LookAt.FindPosition();
                    _cameraOrbit.LookAt = lookAtObj;
                }

                _cameraOrbit.RotationPeriod = orbit.RevolutionTime;
                _cameraOrbit.OrbitOffset = new UnityEngine.Vector3(orbit.Distance, orbit.Height, 0);
                    
                _cameraOrbit.Invalidate();
                break;
            default:
                throw new NotImplementedException();
        }
        
        string gltfPath = Path.Combine(basePath, scene.FileName);
        Importer.ImportGLBAsync(gltfPath, new ImportSettings(), (result, clips) =>
        {
            _modelVisibility.SetTarget(result);
        });

        _ntpSync.SendReset();
    }

    public void SwipeLeftReceived(SensorDataMessage e)
    {
        _flagNavigator?.SelectPrevious();
    }

    public void SwipeRightReceived(SensorDataMessage e)
    {
        _flagNavigator?.SelectNext();
    }

    public void SwipeActReceived(SensorDataMessage e)
    {
        _flagNavigator?.Activate();
    }

    private IEnumerator DelayApply()
    {
        // Wait two frames for the camera transformation to apply
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // Debug mode detection
        if (!_packageProvider.ConfigurationExists) SpawnDebugScene();
        else SpawnLoadedScene();
    }

    private void SpawnDebugScene()
    {
        Apply(new GltfObject
        {
            FileName = "Assets/Scenes/3DObject/Sample/monkey.glb",
            SkyboxTint = "#000000",
            Flags = new System.Collections.Generic.List<GltfObject.Flag>(),
            CameraAnimation = new GltfObject.OrbitAnimation
            {
                Distance = 5,
                Height = 2,
                RevolutionTime = 5f,
                LookAt = new GltfObject.GltfLocation { Offset = new Vector3Data() },
                Origin = new GltfObject.GltfLocation { Offset = new Vector3Data() },
            }
        }, string.Empty);
    }

    private void SpawnLoadedScene()
    {
        var settings = _packageProvider.Configuration.Parameters.Settings;
        Apply(new GltfObject
        {
            FileName = settings.FileName,
            Skybox = settings.Skybox,
            SkyboxTint = settings.SkyboxTint,
            CameraAnimation = new GltfObject.OrbitAnimation
            {
                Origin = new GltfObject.GltfLocation
                {
                    ObjectName = settings.CameraAnimation.Origin.ObjectName,
                    Offset = new Vector3Data
                    {
                        X = (float)settings.CameraAnimation.Origin.Offset.X,
                        Y = (float)settings.CameraAnimation.Origin.Offset.Y,
                        Z = (float)settings.CameraAnimation.Origin.Offset.Z,
                    }
                },
                LookAt = new GltfObject.GltfLocation
                {
                    ObjectName = settings.CameraAnimation.LookAt.ObjectName,
                    Offset = new Vector3Data
                    {
                        X = (float)settings.CameraAnimation.LookAt.Offset.X,
                        Y = (float)settings.CameraAnimation.LookAt.Offset.Y,
                        Z = (float)settings.CameraAnimation.LookAt.Offset.Z,
                    }
                },
                Distance = (float)settings.CameraAnimation.Distance,
                Height = (float)settings.CameraAnimation.Height,
                RevolutionTime = (float)settings.CameraAnimation.RevolutionTime,
            },
            FlagInteraction = settings.FlagInteraction switch
            {
                FlagInteraction.Swipe => GltfObject.FlagInteractionTypeEnum.Swipe,
                FlagInteraction.Point => GltfObject.FlagInteractionTypeEnum.Point,
                _ => throw new NotSupportedException(),
            },
            Flags = settings.Flags.Select(f =>
            {
                return new GltfObject.Flag
                {
                    Location = new GltfObject.GltfLocation
                    {
                        ObjectName = "",
                        Offset = new Vector3Data
                        {
                            X = (float)f.Location.X,
                            Y = (float)f.Location.Y,
                            Z = (float)f.Location.Z
                        }
                    },
                    Text = f.Text,
                    ActivatedAction = f.ActivatedAction,
                    SelectedAction = f.SelectedAction,
                    ForegroundColor = f.ForegroundColor,
                    BackgroundColor = f.BackgroundColor,
                    StalkColor = f.StalkColor,
                    CanSelect = (bool)f.CanSelect
                };
            }).ToList()
        }, _packageProvider.Configuration.DataRoot);
    }
}
