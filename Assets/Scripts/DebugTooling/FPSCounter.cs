using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{

    public TextMeshProUGUI display_Text;
    public bool isGPU = false;

    [SerializeField] private float timer = 1;
    private float currentTimer, avgFramerate;

    public void Update()
    {
        if (isGPU) gpuFPS();
        else cpuFPS();
    }

    private void gpuFPS()
    {
        //Change smoothDeltaTime to deltaTime or fixedDeltaTime to see the difference
        float timelapse = Time.smoothDeltaTime;
        currentTimer = currentTimer <= 0 ? timer : currentTimer -= timelapse;

        if (currentTimer <= 0) avgFramerate = (int)(1f / timelapse);
        display_Text.text = "GPU FPS: " + avgFramerate.ToString("F1");
    }

    private void cpuFPS()
    {
        // Calculate frames per second
        currentTimer = currentTimer <= 0 ? timer : currentTimer -= Time.deltaTime;

        if (currentTimer <= 0) avgFramerate = (int)(1f / Time.deltaTime);
        display_Text.text = "CPU FPS: " + avgFramerate.ToString("F1");
    }

}
