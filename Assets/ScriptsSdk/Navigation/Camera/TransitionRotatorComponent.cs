using UnityEngine;

[RequireComponent(typeof(NodeNavigatorComponent))]
public class TransitionRotatorComponent : MonoBehaviour
{
    public float RotationSpeed = 2f;

    private NodeNavigatorComponent _navigator;

    void Start()
    {
        _navigator = GetComponent<NodeNavigatorComponent>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Stationary, leave it to the other component
        if (_navigator.NextNode == null) return;

        Vector3 direction = _navigator.NextNode.LookPosition() - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }
}
