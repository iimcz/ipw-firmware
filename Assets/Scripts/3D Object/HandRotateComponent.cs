using UnityEngine;

public class HandRotateComponent : MonoBehaviour
{
    [SerializeField]
    private CursorComponent _cursor;

    [SerializeField] 
    private OrbitComponent _orbit;

    // Update is called once per frame
    void Update()
    {
        if (_cursor.IsVisible)
        {
            _orbit.AutoOrbit = false;
            
            if (_cursor.ScreenPos.x > 0.8) _orbit.Advance(Time.deltaTime);
            else if (_cursor.ScreenPos.x < 0.2) _orbit.Advance(-Time.deltaTime);
        }
        else
        {
            _orbit.AutoOrbit = true;
        }
    }
}
