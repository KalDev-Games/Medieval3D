using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAduioController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private List<AudioClip> clipList;
    [SerializeField]
    private List<AudioSource> sourceList;

    private void LateUpdate()
    {
        for (int i = 0; i < sourceList.Count; i++)
        {
            if (!sourceList[i].isPlaying)
            {
                int track = Random.Range(0,clipList.Count * 5);

                if (track < clipList.Count)
                {
                    sourceList[i].clip = clipList[track];
                    sourceList[i].Play();
                }
                
            }
        }
    }
}
