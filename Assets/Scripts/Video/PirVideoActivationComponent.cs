using Naki3D.Common.Protocol;
using UnityEngine;

public class PirVideoActivationComponent : MonoBehaviour
{
    [SerializeField]
    private VideoDisplayComponent _video;

    private void Start()
    {
        _video.VideoPlayer.time = 1f;
        _video.VideoPlayer.Play();
        _video.VideoPlayer.Pause();
    }

    public void OnPir(SensorMessage sensor)
    {
        if (sensor.PirMovement.Event == PirMovementEvent.MovementStarted) _video.VideoPlayer.Play();
    }
}
