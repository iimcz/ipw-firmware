using UnityEngine;

public class PointRotateComponent : MonoBehaviour
{
    public float Speed = 1.0f;
    
    void Update()
    {
        var progress = Speed * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, progress, 0);
    }
}
