using UnityEngine;
using UnityEngine.Events;

public class LookNodeComponent : MonoBehaviour, ILookable
{
    public NavigationNodeComponent ParentNode;

    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    public UnityEvent OnActivate;

    public void Activate()
    {
        OnActivate.Invoke();
    }

    public bool CanActivate()
    {
        // TODO: What if someone adds an event at runtime
        return OnActivate.GetPersistentEventCount() != 0;
    }

    public GameObject GameObject()
    {
        return gameObject;
    }

    public Vector3 LookPosition()
    {
        return transform.position;
    }
}
