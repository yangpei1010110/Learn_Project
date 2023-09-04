using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FpsDetect : MonoBehaviour
{
    private TextMeshProUGUI textMeshProUGUI;
    Queue<float>            FrameDeltaTimes = new Queue<float>();
    private int             SampleCount     = 10;

    private void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        while (FrameDeltaTimes.Count > SampleCount)
        {
            FrameDeltaTimes.Dequeue();
        }

        FrameDeltaTimes.Enqueue(Time.deltaTime);
        textMeshProUGUI.SetText($"FPS:{1f / FrameDeltaTimes.Average():0.0}");
    }
}