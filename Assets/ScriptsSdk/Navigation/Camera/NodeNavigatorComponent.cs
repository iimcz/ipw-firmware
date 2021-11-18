using System;
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

    void Start()
    {
        // Snap to start point so the scene creator doesn't have to align
        transform.position = CurrentNode.transform.position;
    }

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
        float progress = 0;
        
        switch (_transition.MovementType)
        {
            case TransitionInfo.MovementTypeEnum.ConstantTime:
                progress = _time / _transition.TravelTime;
                break;
            case TransitionInfo.MovementTypeEnum.AnimationCurve:
                progress = _transition.Curve.Evaluate(_time);
                break;
        }

        var start = CurrentNode.LookPosition();
        var end = NextNode.LookPosition();

        transform.position = Vector3.Lerp(start, end, progress);

        if (progress >= 0.99f)
        {
            CurrentNode = NextNode;
            transform.position = end;

            NextNode = null;
            _transition = null;
            _time = 0f;

            CurrentNode.OnEnter.Invoke();

            if (CurrentNode.NavigationNodeType == NavigationNodeComponent.NavigationNodeTypeEnum.PathPoint)
            {
                SetTarget(CurrentNode.NextNodes.First().NextNode);
            }
            else
            {
                OnTransitionEnd.Invoke(CurrentNode);
                CurrentNode.SetLineVisibility(true);
            }
        }
    }
}
