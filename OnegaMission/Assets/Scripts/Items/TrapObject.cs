using UnityEngine;
using DG.Tweening;
using InputSystemProject;
using Items;
using Player;
using Unity.Cinemachine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TrapObject : MonoBehaviour, IInteractable
{
    [Header("Requirements")]
    [SerializeField] private ToolType _requiredTool = ToolType.Scissors;
    [SerializeField] private ItemTypeSO _rewardTrashType;

    [Header("Minigame")]
    [SerializeField] private GameObject _minigamePanel;
    [SerializeField] private int _requiredClicks = 5;
    [SerializeField] private TMP_Text _motivationText; // Текст для мотивирующих фраз
    [SerializeField] private string[] _motivationMessages = new string[]
    {
        "Ещё немного!",
        "Так держать!",
        "Почти готово!",
        "Отлично!",
        "Последнее усилие!"
    };

    [Header("Player Control")]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CinemachineCamera _playerVirtualCamera;

    [Header("Animation")]
    [SerializeField] private float _flyDuration = 2f;
    [SerializeField] private Vector3 _flyOffset = new Vector3(0, 5, 0);
    [SerializeField] private Ease _flyEase = Ease.OutQuad;
    [SerializeField] private Transform _flyTargetPoint; // Точка назначения
    [SerializeField] private float _rotationDuration = 0.5f; // Длительность начального поворота
    [SerializeField] private float _flightRotationAngle = 90f; // Вращение во время полёта

    [Header("Trash Object")]
    [SerializeField] private Transform _trashChildObject; // Ссылка на объект мусора на модели (ребенок)

    [Header("Info")]
    [SerializeField] private string _itemName = "Запутанный объект";
    [SerializeField] private string _itemType = "Ловушка";
    [SerializeField] [TextArea] private string _description = "Кто-то запутался в мусоре. Нужны ножницы!";
    [SerializeField] [TextArea] private string _lore = " ";

    [Header("Events")]
    public UnityEvent OnMinigameStart;
    public UnityEvent OnMinigameComplete;
    public UnityEvent OnFlyAway;

    private bool _isInteractable = true;
    private int _remainingClicks;
    private bool _isMinigameActive = false;
    private int _clickCount;

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
        _clickCount = 0;

        if (_playerMovement != null) _playerMovement.enabled = false;
        if (_playerVirtualCamera != null) _playerVirtualCamera.enabled = false;

        InputManager.Instance.ChangeInputMap(InputType.UI);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (_minigamePanel != null)
        {
            _minigamePanel.SetActive(true);
            var buttons = _minigamePanel.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnButtonClick(btn));
                btn.interactable = true;
            }
        }

        if (_motivationText != null)
            _motivationText.text = "Начинаем!";

        OnMinigameStart?.Invoke();
    }

    private void OnButtonClick(Button button)
    {
        if (!_isMinigameActive) return;
        if (!button.interactable) return;

        button.interactable = false;
        _remainingClicks--;
        _clickCount++;

        UpdateMotivationText();

        if (_remainingClicks <= 0)
        {
            CompleteMinigame();
        }
    }

    private void UpdateMotivationText()
    {
        if (_motivationText == null) return;

        int messageIndex = Mathf.Min(_clickCount - 1, _motivationMessages.Length - 1);
        
        if (messageIndex >= 0 && messageIndex < _motivationMessages.Length)
        {
            _motivationText.text = _motivationMessages[messageIndex];
            
            // Анимация текста
            _motivationText.transform.DOKill();
            _motivationText.transform.DOScale(1.2f, 0.1f).OnComplete(() =>
            {
                _motivationText.transform.DOScale(1f, 0.1f);
            });
        }

        if (_remainingClicks <= 0)
        {
            _motivationText.text = "Готово! 🎉";
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

        // Определяем цель полета
        Vector3 targetPosition = _flyTargetPoint != null ? _flyTargetPoint.position : transform.position + _flyOffset;

        // Создаем последовательность анимаций
        Sequence flySequence = DOTween.Sequence();

        // 1. СНАЧАЛА ПОВОРОТ к точке назначения (лицом к цели)
        if (_flyTargetPoint != null)
        {
            Vector3 direction = targetPosition - transform.position;
            if (direction.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // Append добавляет действие в конец очереди (ждет завершения предыдущего)
                flySequence.Append(transform.DORotate(targetRotation.eulerAngles, _rotationDuration));
            }
        }

        // 2. ПОТОМ ДВИЖЕНИЕ к точке + вращение на 90 градусов во время полёта
        flySequence.Append(transform.DOMove(targetPosition, _flyDuration).SetEase(_flyEase));
        flySequence.Join(transform.DORotate(transform.eulerAngles + new Vector3(0, _flightRotationAngle, 0), _flyDuration).SetEase(_flyEase));

        // Завершение
        flySequence.OnComplete(() =>
        {
            OnFlyAway?.Invoke();
            Destroy(gameObject);
        });
    }

    private void AddTrashToInventory()
    {
        TrashItem trash = null;

        // Если назначен дочерний объект на сцене
        if (_trashChildObject != null)
        {
            // Отцепляем от родителя (ловушки), сохраняя мировые координаты
            _trashChildObject.SetParent(null);
            
            // Получаем компонент TrashItem
            trash = _trashChildObject.GetComponent<TrashItem>();
            if (trash == null)
                trash = _trashChildObject.gameObject.AddComponent<TrashItem>();

            // Деактивируем объект (он теперь в инвентаре)
            _trashChildObject.gameObject.SetActive(false);
        }
        // Если дочерний объект не назначен, пробуем найти его среди детей
        else
        {
            Transform childTrash = transform.Find("Trash");
            if (childTrash != null)
            {
                childTrash.SetParent(null);
                trash = childTrash.GetComponent<TrashItem>();
                if (trash == null)
                    trash = childTrash.gameObject.AddComponent<TrashItem>();
                childTrash.gameObject.SetActive(false);
            }
        }

        // Добавляем в инвентарь
        if (trash != null)
        {
            Inventory.Instance.AddItem(trash);
        }
        else
        {
            Debug.LogWarning("TrapObject: Объект мусора не найден!");
        }
    }
}