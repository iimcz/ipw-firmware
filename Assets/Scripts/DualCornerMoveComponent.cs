using Sirenix.OdinInspector;
using UnityEngine;

public class DualCornerMoveComponent : MonoBehaviour
{
    public int ActiveDisplay;

    [Required, SceneObjectsOnly]
    public CornerMoveComponent CornerMove1;

    [Required, SceneObjectsOnly]
    public CornerMoveComponent CornerMove2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ActiveDisplay++;
            ActiveDisplay %= 2;

            if (ActiveDisplay == 0)
            {
                CornerMove1.enabled = true;
                CornerMove2.enabled = false;
            }
            else
            {
                CornerMove1.enabled = false;
                CornerMove2.enabled = true;
            }
        }
    }
}
