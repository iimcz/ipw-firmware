using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPointComponent : MonoBehaviour
{
    [SerializeField]
    private NodeNavigatorComponent _navigator;

    [SerializeField]
    private NavigationNodeComponent _node;

    public void Activate()
    {
        _navigator.CurrentNode.SetLineVisibility(false);
        _node.SetLineVisibility(true);
        
        _navigator.CurrentNode = _node;
        _navigator.SetPosition(_node);
    }
}
