using Naki3D.Common.Protocol;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GalleryCursorComponent : MonoBehaviour
{
    [SerializeField]
    private GalleryPoolComponent _pool;
    
    [SerializeField]
    private CursorComponent _cursor;

    private GameObject _activatedImage;
    private Vector3 _activatedPos;
    private Vector3 _activatedScale;

    public void Activate(GameObject image)
    {
        if (_activatedImage != null) return;
        
        _activatedImage = image;
        _activatedPos = image.transform.localPosition;
        _activatedScale = image.transform.localScale;
        
        image.transform.localScale *= 2;
        image.transform.localPosition = new Vector3(-image.transform.localScale.x / 2f, 0, 0);

        _pool.EnableInteraction = false;
    }

    public void OnGesture(SensorMessage message)
    {
        if (message.DataCase != SensorMessage.DataOneofCase.Gesture) return;
        if (_activatedImage == null) return;
        
        if (message.Gesture.Type == GestureType.GestureSwipeUp)
        {
            _activatedImage.transform.localPosition = _activatedPos;
            _activatedImage.transform.localScale = _activatedScale;
            _activatedImage = null;
            _pool.EnableInteraction = true;
        }
    }
    
    void Update()
    {
        // Wait for init
        if (_pool.Layout == null) return;
        
        if (_activatedImage != null) _pool.Layout.ScrollDelay = 0f;
        else _pool.Layout.ScrollDelay = _cursor.IsVisible ? 0f : 2f;
    }
}
