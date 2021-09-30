using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class NodeNavigatorComponent : MonoBehaviour
{
    public NavigationNodeComponent CurrentNode;
    public NavigationNodeComponent NextNode;

    private TransitionInfo _transition;
    private float _time = 0f;

    public UnityEvent<NavigationNodeComponent> OnTransitionStart;
    public UnityEvent<NavigationNodeComponent> OnTransitionEnd;

    public void SetTarget(NavigationNodeComponent target)
    {
        NextNode = target;
        _transition = CurrentNode.NextNodes
            .First(n => n.NextNode == target);

        if (_transition == null) throw new System.Exception("Cannot traverse between nodes without a transition!");

        OnTransitionStart.Invoke(NextNode);
    }

    void FixedUpdate()
    {
        if (NextNode == null) return;

        _time += Time.deltaTime;
        var progress = _time / _transition.TravelTime;

        var start = CurrentNode.LookPosition();
        var end = NextNode.LookPosition();

        transform.position = Vector3.Lerp(start, end, progress);

        if (progress >= 1f)
        {
            CurrentNode = NextNode;
            transform.position = end;

            NextNode = null;
            _transition = null;
            _time = 0f;

            if (CurrentNode.NavigationNodeType == NavigationNodeComponent.NavigationNodeTypeEnum.PathPoint)
            {
                SetTarget(CurrentNode.NextNodes.First().NextNode);
            }
            else
            {
                OnTransitionEnd.Invoke(CurrentNode);
            }
        }
    }
}
