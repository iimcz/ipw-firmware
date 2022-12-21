using System;
using UnityEngine;

public class ModelVisibilityComponent : MonoBehaviour
{
    [SerializeField]
    public bool StartHidden = true;
    
    private GameObject _target;

    public void Hide()
    {
        _target.SetActive(false);
    }

    public void Show()
    {
        _target.SetActive(true);
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
        if (StartHidden)
            Hide();
    }
}