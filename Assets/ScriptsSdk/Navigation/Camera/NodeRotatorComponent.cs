using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NodeNavigatorComponent))]
[RequireComponent(typeof(NodeNavigatorUIComponent))]
public class NodeRotatorComponent : MonoBehaviour
{
    public AnimationCurve RotationCurve;

    private NodeNavigatorComponent _navigator;
    private NodeNavigatorUIComponent _ui;

    [SerializeField]
    private int _index = 0;

    [SerializeField]
    private bool _shortWay;

    private ILookable Target => _navigator.CurrentNode.GetLookable(_index);

    /// <summary>
    /// Whether the user can queue up another input
    /// </summary>
    private bool CanActivate => RotationCurve.keys.Last().time <= _rotationTime;

    private float _rotationTime = 1.0f; // Start the player fully rotated
    private Quaternion _startRotation;

    void Start()
    {
        _navigator = GetComponent<NodeNavigatorComponent>();
        _ui = GetComponent<NodeNavigatorUIComponent>();

        _startRotation = transform.rotation;
    }

    // Late, since we're updating camera position
    // I really hope they don't, but they could attach the node to a moving platform or something
    void LateUpdate()
    {
        // We're currently moving, leave it to the other component
        if (_navigator.NextNode != null) return;
        _rotationTime += Time.deltaTime;

        Vector3 direction = Target.LookPosition() - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);

        var rotation = QuaternionExtension.Lerp(_startRotation, toRotation, RotationCurve.Evaluate(_rotationTime), _shortWay);

        // Cancel out tilt, I doubt anyone would want to tilt the camera on purpose
        rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
        transform.rotation = rotation;
    }

    public void TurnLeft()
    {
        if (!CanActivate) return;

        _index--;
        if (_index < 0) _index = _navigator.CurrentNode.LookList.Count - 1;

        ResetRotation(false);
    }

    public void TurnRight()
    {
        if (!CanActivate) return;

        _index++;
        if (_index >= _navigator.CurrentNode.LookList.Count) _index = 0;

        ResetRotation(true);
    }

    public void Activate()
    {
        if (!CanActivate) return;

        if (Target.CanActivate()) Target.Activate();
        else _ui.CannotContinue();
    }

    public void CurrentNodeChanged(NavigationNodeComponent node)
    {
        var direction = _navigator.transform.forward;
        var closest = node.GetClosestLookable(direction, transform.position);
        _index = node.IndexOfLookable(closest);

        ResetRotation(false);
        _shortWay = true; // Automatic rotate, no need to spin
    }

    private void ResetRotation(bool right)
    {
        _rotationTime = 0;
        _startRotation = transform.rotation;

        Vector3 direction = Target.LookPosition() - transform.position;
        var angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

        if (angle >= 0 && !right) _shortWay = false;
        else if (angle <= 0 && right) _shortWay = false;
        else _shortWay = true;
    }
}
