using UnityEngine;

[RequireComponent(typeof(NodeNavigatorComponent))]
[RequireComponent(typeof(NodeNavigatorUIComponent))]
public class NodeRotatorComponent : MonoBehaviour
{
    public float RotationSpeed = 1f;

    private NodeNavigatorComponent _navigator;
    private NodeNavigatorUIComponent _ui;

    [SerializeField]
    private int _index = 0;

    [SerializeField]
    private bool _right;

    private ILookable Target => _navigator.CurrentNode.GetLookable(_index);

    void Start()
    {
        _navigator = GetComponent<NodeNavigatorComponent>();
        _ui = GetComponent<NodeNavigatorUIComponent>();
    }

    // Late, since we're updating camera position
    // I really hope they don't, but they could attach the node to a moving platform or something
    void LateUpdate()
    {
        // We're currently moving, leave it to the other component
        if (_navigator.NextNode != null) return;

        Vector3 direction = Target.LookPosition() - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }

    public void TurnLeft()
    {
        _index--;
        _right = false;

        if (_index < 0) _index = _navigator.CurrentNode.LookList.Count - 1;
    }

    public void TurnRight()
    {
        _index++;
        _right = true;

        if (_index >= _navigator.CurrentNode.LookList.Count) _index = 0;
    }

    public void Activate()
    {
        if (Target.CanActivate()) Target.Activate();
        else _ui.CannotContinue();
    }

    public void CurrentNodeChanged(NavigationNodeComponent node)
    {
        var direction = node.transform.position - _navigator.CurrentNode.transform.position;
        var closest = node.GetClosestLookable(direction, transform.position);
        _index = node.IndexOfLookable(closest);
    }
}
