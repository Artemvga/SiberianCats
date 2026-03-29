using UnityEngine;
using UnityEngine.InputSystem;
using InputSystemProject;
using Items;
using Player;
using Unity.Cinemachine;
using UnityEngine.Events;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// Назначение файла: SortingTable.cs
// Путь: Assets/Scripts/Sorting/SortingTable.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `SortingTable` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class SortingTable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _tableCameraPosition;
    [SerializeField] private SortingManager _sortingManager;
    [SerializeField] private GameObject _enterPrompt;
    [SerializeField] private GameObject _exitPrompt;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CinemachineCamera _playerVirtualCamera;
    [SerializeField] private Camera _mainCamera;

    [Header("Events")]
    public UnityEvent OnTableEnter;
    public UnityEvent OnTableExit;

    private bool _isPlayerInRange = false;
    private bool _isSortingActive = false;
    private Transform _originalCameraParent;
    private Vector3 _originalCameraPos;
    private Quaternion _originalCameraRot;

    // Для отключения рендеринга игрока и инструментов
    private List<Renderer> _playerRenderers = new List<Renderer>();
    private List<bool> _playerRendererStates = new List<bool>();
    private GameObject _currentToolModel;
    private List<Renderer> _toolRenderers = new List<Renderer>();
    private List<bool> _toolRendererStates = new List<bool>();

    /// <summary>
    /// Запускает начальную настройку после инициализации сцены.
    /// </summary>
    private void Start()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
        if (_enterPrompt != null) _enterPrompt.SetActive(false);
        if (_exitPrompt != null) _exitPrompt.SetActive(false);
    }

    /// <summary>
    /// Срабатывает при активации компонента.
    /// </summary>
    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.actions.Player.Interact.performed += OnInteractPerformed;
            InputManager.Instance.actions.UI.Cancel.performed += OnCancelPerformed;
        }
    }

    /// <summary>
    /// Срабатывает при деактивации компонента.
    /// </summary>
    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.actions.Player.Interact.performed -= OnInteractPerformed;
            InputManager.Instance.actions.UI.Cancel.performed -= OnCancelPerformed;
        }
    }

    /// <summary>
    /// Выполняет операцию `OnTriggerEnter` в рамках обязанностей текущего компонента.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            if (_enterPrompt != null && !_isSortingActive) _enterPrompt.SetActive(true);
        }
    }

    /// <summary>
    /// Выполняет операцию `OnTriggerExit` в рамках обязанностей текущего компонента.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = false;
            if (_enterPrompt != null) _enterPrompt.SetActive(false);
            if (_exitPrompt != null) _exitPrompt.SetActive(false);
        }
    }

    /// <summary>
    /// Выполняет операцию `OnInteractPerformed` в рамках обязанностей текущего компонента.
    /// </summary>
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        TryStartSorting();
    }

    /// <summary>
    /// Выполняет операцию `TryStartSorting` в рамках обязанностей текущего компонента.
    /// </summary>
    public void TryStartSorting()
    {
        if (!_isPlayerInRange || _isSortingActive) return;
        if (!PlayerTools.Instance.HasTool(ToolType.Tablet) ||
            ActiveTool.Instance.GetCurrentToolType() != ToolType.Tablet)
        {
            Debug.Log("Планшет должен быть экипирован в руке");
            return;
        }
        StartSorting();
    }

    /// <summary>
    /// Выполняет операцию `StartSorting` в рамках обязанностей текущего компонента.
    /// </summary>
    private void StartSorting()
    {
        _isSortingActive = true;
        OnTableEnter?.Invoke();

        // Закрываем планшет, если он открыт
        if (TabletUI.IsOpen)
        {
            TabletUI tabletUI = FindObjectOfType<TabletUI>();
            if (tabletUI != null) tabletUI.CloseIfOpen();
        }

        // Скрываем подсказку входа и показываем подсказку выхода
        if (_enterPrompt != null) _enterPrompt.SetActive(false);
        if (_exitPrompt != null) _exitPrompt.SetActive(true);

        // Отключаем движение игрока
        if (_playerMovement != null) _playerMovement.enabled = false;
        InputManager.Instance.ChangeInputMap(InputType.UI);

        // Отключаем виртуальную камеру
        if (_playerVirtualCamera != null) _playerVirtualCamera.enabled = false;

        // Сохраняем и отключаем рендер игрока
        SaveAndDisablePlayerRenderers();

        // Сохраняем и отключаем текущую модель инструмента
        SaveAndDisableToolModel();

        // Перемещаем камеру
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

    /// <summary>
    /// Выполняет операцию `StopSorting` в рамках обязанностей текущего компонента.
    /// </summary>
    public void StopSorting()
    {
        if (!_isSortingActive) return;

        _isSortingActive = false;
        OnTableExit?.Invoke();

        // Восстанавливаем рендер игрока
        RestorePlayerRenderers();

        // Восстанавливаем модель инструмента
        RestoreToolModel();

        // Возвращаем камеру
        _mainCamera.transform.SetParent(_originalCameraParent);
        _mainCamera.transform.position = _originalCameraPos;
        _mainCamera.transform.rotation = _originalCameraRot;

        if (_playerVirtualCamera != null) _playerVirtualCamera.enabled = true;
        if (_playerMovement != null) _playerMovement.enabled = true;

        InputManager.Instance.ChangeInputMap(InputType.Player);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Скрываем подсказку выхода
        if (_exitPrompt != null) _exitPrompt.SetActive(false);
        // Если игрок всё ещё в зоне, показываем подсказку входа
        if (_enterPrompt != null && _isPlayerInRange) _enterPrompt.SetActive(true);

        _sortingManager.StopSorting();
    }

    /// <summary>
    /// Выполняет операцию `OnCancelPerformed` в рамках обязанностей текущего компонента.
    /// </summary>
    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (_isSortingActive) StopSorting();
    }

    #region Visibility Helpers

    /// <summary>
    /// Выполняет операцию `SaveAndDisablePlayerRenderers` в рамках обязанностей текущего компонента.
    /// </summary>
    private void SaveAndDisablePlayerRenderers()
    {
        if (_playerMovement == null) return;

        // Находим все Renderer на объекте игрока и его дочерних объектах
        var renderers = _playerMovement.GetComponentsInChildren<Renderer>(true);
        _playerRenderers.Clear();
        _playerRendererStates.Clear();

        foreach (var r in renderers)
        {
            // Пропускаем камеры (если вдруг камера дочерняя)
            if (r.GetComponent<Camera>() != null) continue;

            _playerRenderers.Add(r);
            _playerRendererStates.Add(r.enabled);
            r.enabled = false;
        }
    }

    /// <summary>
    /// Выполняет операцию `RestorePlayerRenderers` в рамках обязанностей текущего компонента.
    /// </summary>
    private void RestorePlayerRenderers()
    {
        for (int i = 0; i < _playerRenderers.Count; i++)
        {
            if (_playerRenderers[i] != null)
                _playerRenderers[i].enabled = _playerRendererStates[i];
        }
        _playerRenderers.Clear();
        _playerRendererStates.Clear();
    }

    /// <summary>
    /// Выполняет операцию `SaveAndDisableToolModel` в рамках обязанностей текущего компонента.
    /// </summary>
    private void SaveAndDisableToolModel()
    {
        if (ActiveTool.Instance == null) return;

        _currentToolModel = ActiveTool.Instance.GetCurrentModel();
        if (_currentToolModel == null) return;

        var renderers = _currentToolModel.GetComponentsInChildren<Renderer>(true);
        _toolRenderers.Clear();
        _toolRendererStates.Clear();

        foreach (var r in renderers)
        {
            _toolRenderers.Add(r);
            _toolRendererStates.Add(r.enabled);
            r.enabled = false;
        }
    }

    /// <summary>
    /// Выполняет операцию `RestoreToolModel` в рамках обязанностей текущего компонента.
    /// </summary>
    private void RestoreToolModel()
    {
        for (int i = 0; i < _toolRenderers.Count; i++)
        {
            if (_toolRenderers[i] != null)
                _toolRenderers[i].enabled = _toolRendererStates[i];
        }
        _toolRenderers.Clear();
        _toolRendererStates.Clear();
        _currentToolModel = null;
    }

    #endregion
}