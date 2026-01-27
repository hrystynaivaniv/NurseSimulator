using UnityEngine;
using TMPro;

public class LoadingTextPulse : MonoBehaviour
{
    private TMP_Text loadingText;
    public float speed = 2.0f;

    void Start()
    {
        loadingText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (loadingText != null)
        {
            float alpha = Mathf.PingPong(Time.time * speed, 1.0f);
            loadingText.alpha = alpha;
        }
    }
}