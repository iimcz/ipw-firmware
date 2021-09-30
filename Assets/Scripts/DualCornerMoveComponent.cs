using Sirenix.OdinInspector;
using UnityEngine;

public class DualCornerMoveComponent : MonoBehaviour
{
    public int ActiveDisplay;

    [Required, SceneObjectsOnly, BoxGroup("Corner move")]
    public CornerMoveComponent CornerMove1;

    [Required, SceneObjectsOnly, BoxGroup("Corner move")]
    public CornerMoveComponent CornerMove2;

    [Required, SceneObjectsOnly, BoxGroup("Color change")]
    public ColorChangeComponent ColorChange1;

    [Required, SceneObjectsOnly, BoxGroup("Color change")]
    public ColorChangeComponent ColorChange2;

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

                ColorChange1.enabled = true;
                ColorChange2.enabled = false;
            }
            else
            {
                CornerMove1.enabled = false;
                CornerMove2.enabled = true;

                ColorChange1.enabled = false;
                ColorChange2.enabled = true;
            }
        }
    }
}
