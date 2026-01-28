using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class SessionManager : MonoBehaviour, IUIElement
{
    public CanvasGroup setupCanvas;
    public GameObject gameCanvas;

    [SerializeField]
    private Transform _topPanel;
    [SerializeField]
    private Transform _mainPanel;

    public TMP_Dropdown hospitalDropdown;
    public TMP_Dropdown modeDropdown;
    public TMP_Dropdown interactionDropdown;

    public MockNetworkManager networkManager;

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
        List<TMP_Dropdown.OptionData> hospitalOptions = new List<TMP_Dropdown.OptionData> ();
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

        hospitalDropdown.AddOptions(hospitalOptions);
        modeDropdown.AddOptions(modeOptions);
        interactionDropdown.AddOptions(interactionOptions);
    }

    public void OnGenerateButtonClicked()
    {
        string selectedDisplayText = hospitalDropdown.options[hospitalDropdown.value].text;

        string hospitalKey = _hospitalMapping.ContainsKey(selectedDisplayText)
            ? _hospitalMapping[selectedDisplayText]
            : selectedDisplayText;

        string mode = modeDropdown.options[modeDropdown.value].text.ToLower();
        string interaction = interactionDropdown.options[interactionDropdown.value].text.ToLower();

        networkManager.StartSession(hospitalKey, mode, interaction);

        
        gameCanvas.SetActive(true);
        Close();
    }

    public void Open()
    {
        return;
    }

    public void Close()
    {
        _topPanel.DOLocalMoveY(250f, 0.3f).SetEase(Ease.InCubic);
        _mainPanel.DOLocalMoveY(-200f, 0.3f).SetEase(Ease.InCubic);
        setupCanvas.DOFade(0f, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            setupCanvas.gameObject.SetActive(false);
        });
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}