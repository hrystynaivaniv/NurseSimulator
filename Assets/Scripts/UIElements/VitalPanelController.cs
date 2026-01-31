using DG.Tweening;
using TMPro;
using UnityEngine;

public class VitalPanelController : BaseUIElement
{
    [Header("UI Text Fields")]
    [SerializeField]
    private TextMeshProUGUI _painText;
    [SerializeField]
    private TextMeshProUGUI _tempText;
    [SerializeField]
    private TextMeshProUGUI _hrText;
    [SerializeField]
    private TextMeshProUGUI _bpText;
    [SerializeField] 
    private TextMeshProUGUI _spo2Text;
    [SerializeField]
    private TextMeshProUGUI _rrText;

    [Space]
    [SerializeField]
    private Transform _heartIcon;

    private VitalsData _oldVitalsData;
    private float _minTemp = 35.0f;
    private float _normalTemp = 36.6f;
    private float _highTemp = 37.5f;
    private float _maxTemp = 40.0f;
    private float _bpm = 60f;

    private Sequence _tweenSeq;

    public void UpdateValues(VitalsData vitals)
    {
        if (_oldVitalsData == null)
        {
            _oldVitalsData = new VitalsData(vitals);

            if (_painText != null)
            {
                _painText.text = vitals.pain.ToString() + "/10";
                _painText.color = GetPainColor(vitals.pain);
            }
            if (_tempText != null)
            {
                _tempText.text = vitals.temp.ToString("F1") + "°C";
                _tempText.color = GetTemperatureColor(vitals.temp);
            }
            if (_hrText != null)
            {
                _hrText.text = vitals.hr.ToString();
                _bpm = Mathf.Clamp(vitals.hr, 30f, 200f);
                PlayHeartbeat();
            }
            if (_bpText != null) _bpText.text = vitals.bp;
            if (_spo2Text != null) _spo2Text.text = vitals.spo2.ToString() + "%";
            if (_rrText != null) _rrText.text = vitals.rr.ToString();

            return;
        }
    }

    public Color GetTemperatureColor(float temp)
    {
        if (temp < _normalTemp)
            return Color.Lerp(Color.cyan, Color.green, Mathf.InverseLerp(_minTemp, _normalTemp, temp));
        else if (temp < _highTemp)
            return Color.Lerp(Color.green, Color.yellow, Mathf.InverseLerp(_normalTemp, _highTemp, temp));
        else
            return Color.Lerp(Color.yellow, Color.red, Mathf.InverseLerp(_highTemp, _maxTemp, temp));
    }

    public Color GetPainColor(int pain)
    {
        if (pain < 5)
            return Color.Lerp(Color.green, Color.yellow, Mathf.InverseLerp(0, 5, pain));
        else
            return Color.Lerp(Color.yellow, Color.red, Mathf.InverseLerp(5, 10, pain));
    }

    void PlayHeartbeat()
    {
        _tweenSeq?.Kill();

        _tweenSeq = DOTween.Sequence();

        float beatTime = 60f / _bpm;

        _tweenSeq.Append(_heartIcon.DOScale(1.25f, beatTime * 0.15f));
        _tweenSeq.Append(_heartIcon.DOScale(1.1f, beatTime * 0.1f));
        _tweenSeq.Append(_heartIcon.DOScale(1f, beatTime * 0.2f));
        _tweenSeq.AppendInterval(beatTime * 0.55f);

        _tweenSeq.SetLoops(-1);
    }
}
