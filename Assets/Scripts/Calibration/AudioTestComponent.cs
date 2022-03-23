using System.Collections;
using UnityEngine;

public class AudioTestComponent : MonoBehaviour
{
    public bool Finished = false;
    
    [SerializeField] private AudioSource _audio;
    
    public IEnumerator StartTest()
    {
        Finished = false;
        
        for (int i = 0; i < 6; i++)
        {
            _audio.panStereo = i >= 3 ? 1 : -1;
            _audio.Play();
            
            if (Application.isEditor) yield return new WaitForSeconds(0.1f);
            else yield return new WaitForSeconds(1.75f);
        }

        Finished = true;
    }
}