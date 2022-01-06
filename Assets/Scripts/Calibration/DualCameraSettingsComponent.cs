using UnityEngine;

[RequireComponent(typeof(DualCameraComponent))]
public class DualCameraSettingsComponent : MonoBehaviour
{
    private DualCameraComponent _camera;

    void Start()
    {
        _camera = GetComponent<DualCameraComponent>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) _camera.SwapDisplays();
        if (Input.GetKeyDown(KeyCode.W)) _camera.SwapSettings();

        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl)) _camera.SaveSettings();
        if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftControl)) _camera.LoadSettings();
    }
}
