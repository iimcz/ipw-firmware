using UnityEngine;
using UnityEngine.Video;

public class VideoLoader : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer _player;

    public void LoadVideo(string path)
    {
        _player.url = $"file://{path}";
        _player.Play();
    }
}
