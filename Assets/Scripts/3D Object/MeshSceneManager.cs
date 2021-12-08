using System;
using System.IO;
using Assets.ScriptsSdk.Extensions;
using emt_sdk.Events;
using emt_sdk.Scene;
using Siccity.GLTFUtility;
using Naki3D.Common.Protocol;
using UnityEngine;

/// <summary>
/// Handles loading 3D object data, scene setup and event handling
/// </summary>
public class MeshSceneManager : MonoBehaviour
{
    [SerializeField]
    private FlagNavigator _flagNavigator;

    [SerializeField] 
    private OrbitComponent _cameraOrbit;
    
    private void Start()
    {
        EventManager.Instance.OnEventReceived += EventReceived;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnEventReceived -= EventReceived;
    }

    public void Apply(GltfObject scene, string basePath)
    {
        var skyboxPath = Path.Combine(basePath, scene.Skybox);
        if (ColorUtility.TryParseHtmlString(scene.SkyboxTint ?? "#FFFFFF", out var tint) == false)
            throw new ArgumentException("Background color is not a valid HTML hex color string",
                nameof(scene.SkyboxTint));
        
        SkyboxLoader.ApplySkybox(skyboxPath, tint);

        switch (scene.CameraAnimation)
        {
            case GltfObject.OrbitAnimation orbit:
                _cameraOrbit.Origin = orbit.Origin.FindPosition();
                
                _cameraOrbit.LookAt = orbit.LookAt.FindObject();
                if (_cameraOrbit.LookAt == null && orbit.LookAt.Offset != null)
                {
                    var lookAtObj = new GameObject("Orbit - Look at");
                    lookAtObj.transform.position = orbit.LookAt.FindPosition();
                    _cameraOrbit.LookAt = lookAtObj;
                }

                _cameraOrbit.RotationPeriod = orbit.RevolutionTime;
                _cameraOrbit.gameObject.transform.position = new UnityEngine.Vector3(orbit.Distance, orbit.Height, 0);
                    
                _cameraOrbit.Invalidate();
                break;
            default:
                throw new NotImplementedException();
        }
        
        string gltfPath = Path.Combine(basePath, scene.FileName);
        Importer.ImportGLBAsync(gltfPath, new ImportSettings(), (result, clips) => { });
    }

    private void EventReceived(object sender, SensorMessage e)
    {
        if (e.DataCase == SensorMessage.DataOneofCase.Gesture)
        {
            switch (e.Gesture.Type)
            {
                case GestureType.GestureSwipeLeft:
                    _flagNavigator.SelectPrevious();
                    break;
                case GestureType.GestureSwipeRight:
                    _flagNavigator.SelectNext();
                    break;
                case GestureType.GesturePush: // TODO: Maybe too many options?
                case GestureType.GestureSwipeUp: // TODO: Does push even work?
                case GestureType.GestureSwipeDown:
                    _flagNavigator.Activate();
                    break;
            }
        }
    }
}
