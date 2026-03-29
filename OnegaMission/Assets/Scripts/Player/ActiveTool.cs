using System.Collections.Generic;
using Items;
using UnityEngine;
using InputSystemProject;
using Player;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// Назначение файла: ActiveTool.cs
// Путь: Assets/Scripts/Player/ActiveTool.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `ActiveTool` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class ActiveTool : MonoBehaviour
{
    public static ActiveTool Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform _handTransform;
    [SerializeField] private GameObject _defaultHandModel;
    [SerializeField] private TabletUI _tabletUI; // ссылка на планшет (для автоматического закрытия)

    private Dictionary<ToolType, GameObject> _toolModels = new Dictionary<ToolType, GameObject>();
    private ToolType _currentTool = ToolType.None;
    private GameObject _currentModel;

    /// <summary>
    /// Инициализирует объект при создании компонента Unity.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Запускает начальную настройку после инициализации сцены.
    /// </summary>
    private void Start()
    {
        if (_defaultHandModel != null) _defaultHandModel.SetActive(true);
    }

    /// <summary>
    /// Срабатывает при активации компонента.
    /// </summary>
    private void OnEnable()
    {
        var playerMap = InputManager.Instance.actions.Player;
        playerMap.SelectTool1.performed += OnSelectTool1;
        playerMap.SelectTool2.performed += OnSelectTool2;
        playerMap.SelectTool3.performed += OnSelectTool3;
    }

    /// <summary>
    /// Срабатывает при деактивации компонента.
    /// </summary>
    private void OnDisable()
    {
        if (InputManager.Instance == null) return;
        var playerMap = InputManager.Instance.actions.Player;
        playerMap.SelectTool1.performed -= OnSelectTool1;
        playerMap.SelectTool2.performed -= OnSelectTool2;
        playerMap.SelectTool3.performed -= OnSelectTool3;
    }

    /// <summary>
    /// Выполняет операцию `RegisterTool` в рамках обязанностей текущего компонента.
    /// </summary>
    public void RegisterTool(ToolType type, GameObject model)
    {
        if (_toolModels.ContainsKey(type))
        {
            Destroy(_toolModels[type]);
            _toolModels[type] = model;
        }
        else
        {
            _toolModels.Add(type, model);
        }
        model.transform.SetParent(_handTransform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.SetActive(false);
    }

    private void OnSelectTool1(InputAction.CallbackContext _) => SelectTool(ToolType.Tablet);
    private void OnSelectTool2(InputAction.CallbackContext _) => SelectTool(ToolType.Scissors);
    private void OnSelectTool3(InputAction.CallbackContext _) => SelectTool(ToolType.PhotoCamera);

    /// <summary>
    /// Выполняет операцию `SelectTool` в рамках обязанностей текущего компонента.
    /// </summary>
    private void SelectTool(ToolType type)
    {
        if (!PlayerTools.Instance.HasTool(type))
        {
            Debug.Log($"Нет инструмента {type}");
            return;
        }

        if (type == _currentTool)
        {
            UnequipCurrentTool();
            return;
        }

        EquipTool(type);
    }

    /// <summary>
    /// Выполняет операцию `EquipTool` в рамках обязанностей текущего компонента.
    /// </summary>
    private void EquipTool(ToolType type)
    {
        // Если экипируем не планшет, а планшет был открыт, закрываем его
        if (type != ToolType.Tablet && _tabletUI != null)
            _tabletUI.CloseIfOpen();

        if (_currentModel != null)
            _currentModel.SetActive(false);
        if (_defaultHandModel != null)
            _defaultHandModel.SetActive(true);

        _currentTool = type;

        if (_toolModels.TryGetValue(type, out GameObject model))
        {
            model.SetActive(true);
            _currentModel = model;
            if (_defaultHandModel != null) _defaultHandModel.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Модель для {type} не зарегистрирована");
        }
    }

    /// <summary>
    /// Выполняет операцию `UnequipCurrentTool` в рамках обязанностей текущего компонента.
    /// </summary>
    private void UnequipCurrentTool()
    {
        // Если снимаем планшет, закрываем его
        if (_currentTool == ToolType.Tablet && _tabletUI != null)
            _tabletUI.CloseIfOpen();

        if (_currentModel != null)
            _currentModel.SetActive(false);
        if (_defaultHandModel != null)
            _defaultHandModel.SetActive(true);
        _currentTool = ToolType.None;
        _currentModel = null;
    }

    public ToolType GetCurrentToolType() => _currentTool;
    public GameObject GetCurrentModel() => _currentModel;
}