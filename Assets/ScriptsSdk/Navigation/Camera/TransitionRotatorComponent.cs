using UnityEngine;

[RequireComponent(typeof(NodeNavigatorComponent))]
public class TransitionRotatorComponent : MonoBehaviour
{
    public AnimationCurve RotationCurve;

    private NodeNavigatorComponent _navigator;
    private NavigationNodeComponent _lastNode;
    private float _time;

    void Start()
    {
        _navigator = GetComponent<NodeNavigatorComponent>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Stationary, leave it to the other component
        if (_navigator.NextNode == null) return;

        if (_lastNode != _navigator.NextNode)
        {
            _time = 0;
            _lastNode = _navigator.NextNode;
        }

        _time += Time.deltaTime;
        var value = RotationCurve.Evaluate(_time);

        Vector3 direction = _navigator.NextNode.LookPosition() - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, value);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }
}
