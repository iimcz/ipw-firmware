using UnityEngine;

public class InverseEnabler : MonoBehaviour
{
    public void SetActive(bool active)
    {
        gameObject.SetActive(!active);
    }
}
