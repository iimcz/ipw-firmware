using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DualSwitchComponent : MonoBehaviour
{
    public int ActiveDisplay;

    public List<MonoBehaviour> GroupA;
    public List<MonoBehaviour> GroupB;
    
    [Required, SceneObjectsOnly]
    public RectTransform GroupAActiveMarker;
    
    [Required, SceneObjectsOnly]
    public RectTransform GroupBActiveMarker;

    private void OnEnable()
    {
        ActivateGroups();
    }

    private void OnDisable()
    {
        foreach (var b in GroupA) b.enabled = false;
        foreach (var b in GroupB) b.enabled = false;
    }

    private void ActivateGroups()
    {
        if (ActiveDisplay == 0)
        {
            GroupAActiveMarker.gameObject.SetActive(true);
            GroupBActiveMarker.gameObject.SetActive(false);
                
            foreach (var b in GroupA) b.enabled = true;
            foreach (var b in GroupB) b.enabled = false;
        }
        else
        {
            GroupAActiveMarker.gameObject.SetActive(false);
            GroupBActiveMarker.gameObject.SetActive(true);
                
            foreach (var b in GroupA) b.enabled = false;
            foreach (var b in GroupB) b.enabled = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ActiveDisplay++;
            ActiveDisplay %= 2;
            
            ActivateGroups();
        }
    }
}
