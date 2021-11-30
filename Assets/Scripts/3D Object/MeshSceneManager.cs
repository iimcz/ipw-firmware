using emt_sdk.Events;
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

    private void Start()
    {
        EventManager.Instance.OnEventReceived += EventReceived;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnEventReceived -= EventReceived;
    }

    public void SetupScene()
    {
        // TODO: We need some protobuf class I can use here
        // TODO: Decide on GLTF vs GLB
        string gltfPath = "";
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
