using System.Collections;
using TMPro;
using UnityEngine;

public class NodeNavigatorUIComponent : MonoBehaviour
{
    public TextMeshProUGUI CannotContinueText;

    public void CannotContinue()
    {
        IEnumerator Delay()
        {
            CannotContinueText.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(3);
            CannotContinueText.gameObject.SetActive(false);
            yield return null;
        }

        StartCoroutine(Delay());
    }
}
