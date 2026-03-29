using UnityEngine;
using DG.Tweening;
using InputSystemProject;
using Items;
using Player;
using Unity.Cinemachine;
using UnityEngine.Events;

public class TrapObject : MonoBehaviour, IInteractable
{
    [Header("Requirements")]
    [SerializeField] private ToolType _requiredTool = ToolType.Scissors;
    [SerializeField] private ItemTypeSO _rewardTrashType;
    [SerializeField] private GameObject _trashPrefab;

    [Header("Minigame")]
    [SerializeField] private GameObject _minigamePanel;
    [SerializeField] private int _requiredClicks = 5;

    [Header("Player Control")]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CinemachineCamera _playerVirtualCamera;

    [Header("Animation")]
    [SerializeField] private float _flyDuration = 2f;
    [SerializeField] private Vector3 _flyOffset = new Vector3(0, 5, 0);
    [SerializeField] private Ease _flyEase = Ease.OutQuad;

    [Header("Info")]
    [SerializeField] private string _itemName = "Запутанный объект";
    [SerializeField] private string _itemType = "Ловушка";
    [SerializeField] [TextArea] private string _description = "Кто-то запутался в мусоре. Нужны ножницы!";
    [SerializeField] [TextArea] private string _lore = "";

    [Header("Events")]
    public UnityEvent OnMinigameStart;
    public UnityEvent OnMinigameComplete;
    public UnityEvent OnFlyAway;

    private bool _isInteractable = true;
    private int _remainingClicks;
    private bool _isMinigameActive = false;

    public string ItemName => _itemName;
    public string ItemType => _itemType;
    public string Description => _description;
    public string Lore => _lore;
    public bool ShouldShowRequirement => true;

    public void OnFocus()
    {
        var outlinable = GetComponent<EPOOutline.Outlinable>();
        if (outlinable != null) outlinable.enabled = true;
    }

    public void OnDefocus()
    {
        var outlinable = GetComponent<EPOOutline.Outlinable>();
        if (outlinable != null) outlinable.enabled = false;
    }

    public bool CanInteract(PlayerTools tools)
    {
        return _isInteractable && ActiveTool.Instance.GetCurrentToolType() == ToolType.Scissors;
    }

    public void Interact()
    {
        if (!_isInteractable) return;
        _isInteractable = false;
        StartMinigame();
    }

    public string GetInteractionMessage()
    {
        if (ActiveTool.Instance.GetCurrentToolType() == ToolType.Scissors)
            return "Нажмите E, чтобы освободить";
        else
            return "Возьмите ножницы в руки (2)";
    }


    private void StartMinigame()
    {
        // Снимаем фокус с объекта, чтобы UI описания исчез
        if (PlayerInteraction.Instance != null)
            PlayerInteraction.Instance.ClearFocus();

        _isMinigameActive = true;
        _remainingClicks = _requiredClicks;

        if (_playerMovement != null) _playerMovement.enabled = false;
        if (_playerVirtualCamera != null) _playerVirtualCamera.enabled = false;

        InputManager.Instance.ChangeInputMap(InputType.UI);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (_minigamePanel != null)
        {
            _minigamePanel.SetActive(true);
            var buttons = _minigamePanel.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            foreach (var btn in buttons)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnButtonClick(btn));
                btn.interactable = true;
            }
        }

        OnMinigameStart?.Invoke();
    }

    private void OnButtonClick(UnityEngine.UI.Button button)
    {
        if (!_isMinigameActive) return;
        if (!button.interactable) return;

        button.interactable = false;
        _remainingClicks--;

        if (_remainingClicks <= 0)
        {
            CompleteMinigame();
        }
    }

    private void CompleteMinigame()
    {
        _isMinigameActive = false;

        InputManager.Instance.ChangeInputMap(InputType.Player);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (_playerMovement != null) _playerMovement.enabled = true;
        if (_playerVirtualCamera != null) _playerVirtualCamera.enabled = true;

        if (_minigamePanel != null)
            _minigamePanel.SetActive(false);

        OnMinigameComplete?.Invoke();

        AddTrashToInventory();

        transform.DOMove(transform.position + _flyOffset, _flyDuration)
            .SetEase(_flyEase)
            .OnComplete(() =>
            {
                OnFlyAway?.Invoke();
                Destroy(gameObject);
            });
    }

    private void AddTrashToInventory()
    {
        GameObject trashGO = Instantiate(_trashPrefab, transform.position, Quaternion.identity);
        TrashItem trash = trashGO.GetComponent<TrashItem>();
        if (trash == null)
            trash = trashGO.AddComponent<TrashItem>();
        trash.gameObject.SetActive(false);
        Inventory.Instance.AddItem(trash);
    }
}