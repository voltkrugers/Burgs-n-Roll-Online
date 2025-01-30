using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    
    public static OptionUI Instance { get; private set; }
    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button secondInteractButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI secondInteractText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Transform pressToRebindKeyTransform;

    private void Awake()
    {
        Instance = this;
        soundEffectButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
        moveUpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Up);});
        moveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Down);});
        moveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Left);});
        moveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Right);});
        interactButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact);});
        secondInteractButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.SecondInteract);});
        pauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause);});
        
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGameUnpaused+= KitchenGameManager_OnGameUnpaused;
        UpdateVisual();
        HidePressToRebindKey();
        Hide();
    }

    private void KitchenGameManager_OnGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume()* 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume()* 10f);

        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        secondInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.SecondInteract);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }
    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding,()=>
        {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}
