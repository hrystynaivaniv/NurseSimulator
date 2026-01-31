using TMPro;
using UnityEngine;

public class ActionDataPanelController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _actionTxt;
    [SerializeField]
    private TextMeshProUGUI _verdictTxt;
    [SerializeField]
    private TextMeshProUGUI _descriptionTxt;
    [SerializeField]
    private TextMeshProUGUI _vitalsTxt;


    public void SetUp(string actionTxt, string verdictTxt, string descriptionTxt, VitalsData vitals)
    {
        _actionTxt.text = actionTxt;
        _verdictTxt.text = verdictTxt;
        _descriptionTxt.text = descriptionTxt;
        _vitalsTxt.text = vitals.ToString();
    }
}
