using UnityEngine;

public class BillboardRotateComponent : MonoBehaviour
{
    public Transform Target;
    
    void Update()
    {
        transform.forward = transform.position - Target.position;
    }
}
