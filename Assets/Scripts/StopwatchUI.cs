using UnityEngine;
using TMPro;

public class StopwatchUI : MonoBehaviour
{
    public TextMeshProUGUI stopwatchText;
    private float totalElapsedTime = 0f;
    private bool isRunning = false;

    void Update()
    {
        if (isRunning)
        {
            totalElapsedTime += Time.deltaTime;
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(totalElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(totalElapsedTime % 60f);
        stopwatchText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartStopwatch() => isRunning = true;
    public void StopStopwatch() => isRunning = false;

    public int GetTotalSeconds() => Mathf.FloorToInt(totalElapsedTime);
}