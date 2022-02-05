using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameCounter : MonoBehaviour
{
    private Text displayFrames;

    private void Start()
    {
        displayFrames = GetComponent<Text>();
    }

    private void Update()
    {
        float current = (int)(1f / Time.unscaledDeltaTime);
        displayFrames.text = "FPS: "  + current.ToString();

    }
}
