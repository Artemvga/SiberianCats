using UnityEngine;
using UnityEngine.InputSystem;
using InputSystemProject;
using Player;
using DG.Tweening;
using Items;

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

    private void Start()
    {
        _photoUI.SetActive(false);
        _playerMovement = FindObjectOfType<PlayerMovement>();
        _playerVirtualCam = FindObjectOfType<Unity.Cinemachine.CinemachineCamera>();
    }

    private void OnEnable()
    {
        _photoModeAction.action.performed += OnPhotoMode;
        _interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        _photoModeAction.action.performed -= OnPhotoMode;
        _interactAction.action.performed -= OnInteract;
    }

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

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!IsActive) return;
        if (_takingPhoto) return;
        StartCoroutine(TakePhotoRoutine());
    }

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