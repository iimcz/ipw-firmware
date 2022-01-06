using System.Collections.Generic;
using UnityEngine;

public class ObjectDisableComponent : MonoBehaviour
{
    public List<GameObject> ControlledObjects;

    private void OnEnable()
    {
        foreach (var obj in ControlledObjects)
        {
            obj.SetActive(true);
        }
    }
    
    private void OnDisable()
    {
        foreach (var obj in ControlledObjects)
        {
            obj.SetActive(false);
        }
    }
}
