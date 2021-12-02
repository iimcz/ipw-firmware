using UnityEngine;

[RequireComponent(typeof(DualCameraComponent))]
public class DualCameraSettingsComponent : MonoBehaviour
{
    public float LensChangeSpeed = 0.001f;
    public float ShiftMultiplier = 0.1f;

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

        var delta = 0f;
        if (InputExtensions.GetKeyModified(KeyCode.P)) delta = LensChangeSpeed;
        else if (InputExtensions.GetKeyModified(KeyCode.M)) delta = -LensChangeSpeed;

        if (Input.GetKey(KeyCode.LeftShift)) delta *= ShiftMultiplier;
        if (delta != 0f && Input.GetKey(KeyCode.RightShift)) delta = Mathf.Sign(delta) * 0.001f;

        if (delta == 0f) return;
        _camera.Setting.LensShift += delta;
        _camera.ApplySettings();
    }
}
