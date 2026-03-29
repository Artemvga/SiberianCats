using System.Collections.Generic;
using Items;
using UnityEngine;
using InputSystemProject;
using Player;
using UnityEngine.InputSystem;

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (_defaultHandModel != null) _defaultHandModel.SetActive(true);
    }

    private void OnEnable()
    {
        var playerMap = InputManager.Instance.actions.Player;
        playerMap.SelectTool1.performed += OnSelectTool1;
        playerMap.SelectTool2.performed += OnSelectTool2;
        playerMap.SelectTool3.performed += OnSelectTool3;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;
        var playerMap = InputManager.Instance.actions.Player;
        playerMap.SelectTool1.performed -= OnSelectTool1;
        playerMap.SelectTool2.performed -= OnSelectTool2;
        playerMap.SelectTool3.performed -= OnSelectTool3;
    }

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