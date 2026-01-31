using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class ActionsHistorylController : BaseUIElement
{
    [SerializeField]
    private ActionDataPanelController _dataPanelPrefab;
    
    [Space]
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private Transform _window;
    [SerializeField]
    private Transform _panelsContainer;
    [SerializeField]
    private Image _fader;
    [SerializeField] 
    private Button _closeBtn;


    private List<ActionDataPanelController> actionDataPanelControllers = new List<ActionDataPanelController>();

    public override void Open()
    {
        _fader.color = new Color(_fader.color.r, _fader.color.g, _fader.color.b, 0f);
        _window.localScale = Vector3.zero;
        _canvasGroup.alpha = 0f;

        gameObject.SetActive(true);

        _fader.DOFade(0.65f, 0.3f).SetEase(Ease.OutCubic);
        _window.DOScale(1f, 0.3f).SetEase(Ease.OutCubic);
        _canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutCubic);

        _closeBtn.onClick.AddListener(Close);
    }

    public override void Close() 
    {
        _fader.DOFade(0f, 0.3f).SetEase(Ease.InCubic);
        _window.DOScale(0f, 0.3f).SetEase(Ease.InCubic);
        _canvasGroup.DOFade(0f, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            ClearPanels();
            _closeBtn.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
        });
    }

    public void SetUpData(List<ActionData> actionsHistory)
    {
        ClearPanels();

        foreach (ActionData actionData in actionsHistory)
        {
            ActionDataPanelController panel = Instantiate<ActionDataPanelController>(_dataPanelPrefab, _panelsContainer);
            panel.SetUp(actionData.action, actionData.response.verdict, actionData.response.textResponse, actionData.response.updatedVitals);
            actionDataPanelControllers.Add(panel);
        }
    }

    private void ClearPanels()
    {
        foreach (var panel in actionDataPanelControllers)
        {
            Destroy(panel.gameObject);
        }
        actionDataPanelControllers.Clear();
    }
}
