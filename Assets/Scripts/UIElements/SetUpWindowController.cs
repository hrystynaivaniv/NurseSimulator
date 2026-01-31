using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetUpWindowController : BaseUIElement
{
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private Transform _topPanel;
    [SerializeField]
    private Transform _mainPanel;

    [SerializeField]
    private TMP_Dropdown _hospitalDropdown;
    [SerializeField] 
    private TMP_Dropdown _modeDropdown;
    [SerializeField] 
    private TMP_Dropdown _interactionDropdown;

    [Space]
    [SerializeField]
    private Button _generateBtn;

    public event Action<string, string, string> OnGenerateBtnClickedEvent;

    private Dictionary<string, string> _hospitalMapping = new Dictionary<string, string>
    {
        { "Emergency Room", "ER" },
        { "Intensive Care Unit", "ICU" },
        { "Cardiology Department", "CARDIO" },
        { "Infectious Diseases Ward", "INFECT" },
        { "Pediatric Ward", "PEDS" },
        { "Surgery Ward", "SURGERY" }
    };

    private List<string> _modes = new List<string>
    {
        "training",
        "exam",
        "consultant"
    };

    private List<string> _interactionModes = new List<string>
    {
        "standard",
        "freeform"
    };

    private void Awake()
    {
        List<TMP_Dropdown.OptionData> hospitalOptions = new List<TMP_Dropdown.OptionData>();
        List<TMP_Dropdown.OptionData> modeOptions = new List<TMP_Dropdown.OptionData>();
        List<TMP_Dropdown.OptionData> interactionOptions = new List<TMP_Dropdown.OptionData>();

        foreach (var env in _hospitalMapping)
        {
            hospitalOptions.Add(new TMP_Dropdown.OptionData(env.Key));
        }

        foreach (var mode in _modes)
        {
            modeOptions.Add(new TMP_Dropdown.OptionData(mode));
        }

        foreach (var inter in _interactionModes)
        {
            interactionOptions.Add(new TMP_Dropdown.OptionData(inter));
        }

        _hospitalDropdown.AddOptions(hospitalOptions);
        _modeDropdown.AddOptions(modeOptions);
        _interactionDropdown.AddOptions(interactionOptions);

        _generateBtn.onClick.AddListener(OnGenerateBtnClicked);
    }

    private void OnGenerateBtnClicked()
    {
        string selectedDisplayText = _hospitalDropdown.options[_hospitalDropdown.value].text;

        string hospitalKey = _hospitalMapping.ContainsKey(selectedDisplayText)
            ? _hospitalMapping[selectedDisplayText]
            : selectedDisplayText;

        string mode = _modeDropdown.options[_modeDropdown.value].text.ToLower();
        string interaction = _interactionDropdown.options[_interactionDropdown.value].text.ToLower();

        OnGenerateBtnClickedEvent?.Invoke(hospitalKey, mode, interaction);

        Close();
    }

    public override void Close()
    {
        _topPanel.DOLocalMoveY(250f, 0.3f).SetEase(Ease.InCubic);
        _mainPanel.DOLocalMoveY(-200f, 0.3f).SetEase(Ease.InCubic);
        _canvasGroup.DOFade(0f, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
