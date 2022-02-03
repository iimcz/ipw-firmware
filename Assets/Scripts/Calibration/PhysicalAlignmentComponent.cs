using UnityEngine;

public class PhysicalAlignmentComponent : MonoBehaviour
{
    private void OnEnable()
    {
        ProjectorTransformationPass.PhysicalAlignment = true;
    }

    private void OnDisable()
    {
        ProjectorTransformationPass.PhysicalAlignment = false;
    }
}
