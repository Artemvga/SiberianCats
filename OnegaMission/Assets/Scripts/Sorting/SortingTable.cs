using UnityEngine;
using UnityEngine.InputSystem;
using InputSystemProject;
using Items;
using Player;
using Unity.Cinemachine;
using UnityEngine.Events;

/// <summary>
/// Компонент стола сортировки.
/// При входе в зону показывает подсказку для входа.
/// При активации – подсказку для выхода.
/// </summary>
public class SortingTable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _tableCameraPosition;        // позиция камеры при сортировке
    [SerializeField] private SortingManager _sortingManager;        // менеджер сортировки
    [SerializeField] private GameObject _enterPrompt;              // подсказка "Нажми E для сортировки"
    [SerializeField] private GameObject _exitPrompt;               // подсказка "Нажми ESC для выхода"
    [SerializeField] private PlayerMovement _playerMovement;       // скрипт движения игрока
    [SerializeField] private CinemachineCamera _playerVirtualCamera; // виртуальная камера игрока
    [SerializeField] private Camera _mainCamera;                   // основная камера

    [Header("Events")]
    public UnityEvent OnTableEnter;    // при входе в режим сортировки
    public UnityEvent OnTableExit;     // при выходе из режима

    private bool _isPlayerInRange = false;
    private bool _isSortingActive = false;
    private Transform _originalCameraParent;
    private Vector3 _originalCameraPos;
    private Quaternion _originalCameraRot;

    private void Start()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
        if (_enterPrompt != null) _enterPrompt.SetActive(false);
        if (_exitPrompt != null) _exitPrompt.SetActive(false);
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.actions.Player.Interact.performed += OnInteractPerformed;
            InputManager.Instance.actions.UI.Cancel.performed += OnCancelPerformed;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.actions.Player.Interact.performed -= OnInteractPerformed;
            InputManager.Instance.actions.UI.Cancel.performed -= OnCancelPerformed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            if (_enterPrompt != null && !_isSortingActive) _enterPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = false;
            if (_enterPrompt != null) _enterPrompt.SetActive(false);
            if (_exitPrompt != null) _exitPrompt.SetActive(false);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        TryStartSorting();
    }

    public void TryStartSorting()
    {
        if (!_isPlayerInRange || _isSortingActive) return;
        if (!PlayerTools.Instance.HasTool(ToolType.Tablet))
        {
            Debug.Log("Для сортировки нужен планшет");
            return;
        }
        StartSorting();
    }

    private void StartSorting()
    {
        _isSortingActive = true;
        OnTableEnter?.Invoke();

        // Скрываем подсказку входа и показываем подсказку выхода
        if (_enterPrompt != null) _enterPrompt.SetActive(false);
        if (_exitPrompt != null) _exitPrompt.SetActive(true);

        if (_playerMovement != null) _playerMovement.enabled = false;
        InputManager.Instance.ChangeInputMap(InputType.UI);

        if (_playerVirtualCamera != null) _playerVirtualCamera.enabled = false;

        _originalCameraParent = _mainCamera.transform.parent;
        _originalCameraPos = _mainCamera.transform.position;
        _originalCameraRot = _mainCamera.transform.rotation;

        _mainCamera.transform.SetParent(null);
        _mainCamera.transform.position = _tableCameraPosition.position;
        _mainCamera.transform.rotation = _tableCameraPosition.rotation;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _sortingManager.StartSorting(this);
    }

    public void StopSorting()
    {
        if (!_isSortingActive) return;

        _isSortingActive = false;
        OnTableExit?.Invoke();

        // Скрываем подсказку выхода
        if (_exitPrompt != null) _exitPrompt.SetActive(false);
        // Если игрок всё ещё в зоне, показываем подсказку входа
        if (_enterPrompt != null && _isPlayerInRange) _enterPrompt.SetActive(true);

        _mainCamera.transform.SetParent(_originalCameraParent);
        _mainCamera.transform.position = _originalCameraPos;
        _mainCamera.transform.rotation = _originalCameraRot;

        if (_playerVirtualCamera != null) _playerVirtualCamera.enabled = true;
        if (_playerMovement != null) _playerMovement.enabled = true;

        InputManager.Instance.ChangeInputMap(InputType.Player);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _sortingManager.StopSorting();
    }

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (_isSortingActive) StopSorting();
    }
}