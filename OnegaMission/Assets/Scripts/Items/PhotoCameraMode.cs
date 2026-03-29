using UnityEngine;
using UnityEngine.InputSystem;
using InputSystemProject;
using Player;
using DG.Tweening;
using Items;

// -----------------------------------------------------------------------------
// Назначение файла: PhotoCameraMode.cs
// Путь: Assets/Scripts/Items/PhotoCameraMode.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `PhotoCameraMode` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class PhotoCameraMode : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _photoUI;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private int _photoWidth = 1920;
    [SerializeField] private int _photoHeight = 1080;
    [SerializeField] private Transform _photoCameraPosition;
    [SerializeField] private float _transitionDuration = 0.3f;

    [Header("Input")]
    [SerializeField] private InputActionReference _photoModeAction; // P
    [SerializeField] private InputActionReference _interactAction;   // E

    public static bool IsActive { get; private set; }

    private bool _takingPhoto = false;
    private Vector3 _originalCameraPos;
    private Quaternion _originalCameraRot;
    private PlayerMovement _playerMovement;
    private Unity.Cinemachine.CinemachineCamera _playerVirtualCam;

    /// <summary>
    /// Запускает начальную настройку после инициализации сцены.
    /// </summary>
    private void Start()
    {
        _photoUI.SetActive(false);
        _playerMovement = FindObjectOfType<PlayerMovement>();
        _playerVirtualCam = FindObjectOfType<Unity.Cinemachine.CinemachineCamera>();
    }

    /// <summary>
    /// Срабатывает при активации компонента.
    /// </summary>
    private void OnEnable()
    {
        _photoModeAction.action.performed += OnPhotoMode;
        _interactAction.action.performed += OnInteract;
    }

    /// <summary>
    /// Срабатывает при деактивации компонента.
    /// </summary>
    private void OnDisable()
    {
        _photoModeAction.action.performed -= OnPhotoMode;
        _interactAction.action.performed -= OnInteract;
    }

    /// <summary>
    /// Выполняет операцию `OnPhotoMode` в рамках обязанностей текущего компонента.
    /// </summary>
    private void OnPhotoMode(InputAction.CallbackContext ctx)
    {
        // Фоторежим доступен только если в руке фотоаппарат
        if (!PlayerTools.Instance.HasTool(ToolType.PhotoCamera) ||
            ActiveTool.Instance.GetCurrentToolType() != ToolType.PhotoCamera)
        {
            return;
        }

        if (!IsActive)
            EnterPhotoMode();
        else
            ExitPhotoMode();
    }

    /// <summary>
    /// Выполняет операцию `OnInteract` в рамках обязанностей текущего компонента.
    /// </summary>
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!IsActive) return;
        if (_takingPhoto) return;
        StartCoroutine(TakePhotoRoutine());
    }

    /// <summary>
    /// Выполняет операцию `EnterPhotoMode` в рамках обязанностей текущего компонента.
    /// </summary>
    private void EnterPhotoMode()
    {
        IsActive = true;

        _originalCameraPos = _mainCamera.transform.position;
        _originalCameraRot = _mainCamera.transform.rotation;

        InputManager.Instance.ChangeInputMap(InputType.UI);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (_playerMovement != null) _playerMovement.enabled = false;
        if (_playerVirtualCam != null) _playerVirtualCam.enabled = false;

        _mainCamera.transform.DOMove(_photoCameraPosition.position, _transitionDuration);
        _mainCamera.transform.DORotate(_photoCameraPosition.rotation.eulerAngles, _transitionDuration);

        _photoUI.SetActive(true);
    }

    /// <summary>
    /// Выполняет операцию `ExitPhotoMode` в рамках обязанностей текущего компонента.
    /// </summary>
    private void ExitPhotoMode()
    {
        IsActive = false;

        _mainCamera.transform.DOMove(_originalCameraPos, _transitionDuration);
        _mainCamera.transform.DORotate(_originalCameraRot.eulerAngles, _transitionDuration);

        InputManager.Instance.ChangeInputMap(InputType.Player);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (_playerMovement != null) _playerMovement.enabled = true;
        if (_playerVirtualCam != null) _playerVirtualCam.enabled = true;

        _photoUI.SetActive(false);
    }

    /// <summary>
    /// Выполняет операцию `TakePhotoRoutine` в рамках обязанностей текущего компонента.
    /// </summary>
    private System.Collections.IEnumerator TakePhotoRoutine()
    {
        _takingPhoto = true;
        _photoUI.SetActive(false);
        yield return null;
        PhotoCapture.TakeScreenshot(_mainCamera, _photoWidth, _photoHeight);
        _photoUI.SetActive(true);
        _takingPhoto = false;
    }
}