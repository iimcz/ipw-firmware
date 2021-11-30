using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class NavigationNodeComponent : MonoBehaviour, ILookable
{
    public enum NavigationNodeTypeEnum
    {
        StopPoint,
        PathPoint
    }

    public UnityEvent OnActivate;
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    public NavigationNodeTypeEnum NavigationNodeType = NavigationNodeTypeEnum.StopPoint;
    public List<TransitionInfo> NextNodes = new List<TransitionInfo>();

    /// <summary>
    /// Enables navigation lines between nodes
    /// </summary>
    public bool EnableNavigationLines = true;

    public Material NavigationLineMaterial;

    // TODO: Unity can't have interfaces in editor fields...
    // https://www.patrykgalach.com/2020/01/27/assigning-interface-in-unity-inspector/
    // check this out later
    public List<Object> LookList = new List<Object>();
    private readonly List<ILookable> _lookList = new List<ILookable>();

    private readonly List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    private NodeNavigatorComponent _navigator;

    void Start()
    {
        foreach (var obj in LookList)
        {
            if (!(obj is ILookable lookable))
            {
                var gameObj = obj as GameObject;
                if (gameObj == null) throw new System.Exception($"{obj} is not a valid ILookable!");

                lookable = gameObj.GetComponent<ILookable>();
                if (lookable == null) throw new System.Exception($"{obj} is not a valid ILookable!");
            }

            _lookList.Add(lookable);
        }

        _navigator = UnityEngine.GameObject.FindGameObjectWithTag("Player")
            .GetComponent<NodeNavigatorComponent>();

        if (!EnableNavigationLines) return;
        
        foreach (var nextNode in NextNodes)
        {
            var lineObject = new GameObject($"Line to {nextNode.NextNode.gameObject.name}");
            lineObject.transform.parent = transform;
            var line = lineObject.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.startColor = line.endColor = new Color(1f, 1f, 1f, 0.25f);
            line.positionCount = 2;
            line.material = NavigationLineMaterial;
            line.startWidth = line.endWidth = 0.1f;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, nextNode.NextNode.transform.position);

            _lineRenderers.Add(line);
        }
        
        if (_navigator.CurrentNode != this) SetLineVisibility(false);
    }

    public ILookable GetLookable(int index)
    {
        return _lookList[index];
    }

    public int IndexOfLookable(ILookable lookable)
    {
        return _lookList.IndexOf(lookable);
    }

    public ILookable GetClosestLookable(Vector3 forward, Vector3 position)
    {
        var closest = _lookList
            .OrderBy(l => Vector3.Angle((l.LookPosition() - position).normalized, forward))
            .FirstOrDefault(l => Vector3.Angle((l.LookPosition() - position).normalized, forward) != 0f);

        if (closest == null)
        {
            Debug.Log("Couldn't find closest lookable, defaulting to first");
            return _lookList.First();
        }
        else return closest;
    }

    public bool CanActivate()
    {
        return true;
    }

    public void Activate()
    {
        OnActivate.Invoke();
        
        _navigator.CurrentNode.SetLineVisibility(false);
        _navigator.CurrentNode.OnExit.Invoke();
        
        _navigator.SetTarget(this);
    }

    public GameObject GameObject()
    {
        return gameObject;
    }

    public Vector3 LookPosition()
    {
        // TODO: Variable player height?
        return transform.position + new Vector3(0, 1, 0);
    }
    
    public void SetLineVisibility(bool visible)
    {
        foreach (var lineRenderer in _lineRenderers)
        {
            lineRenderer.enabled = visible;
        }
    }
}
