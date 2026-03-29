using System.Text;
using InputSystemProject;
using Items;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class TabletUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _tabletPanel;
    [SerializeField] private TMP_Text _trashCountText;       // теперь один текстовый элемент для количества
    [SerializeField] private TMP_Text _toolsListText;
    [SerializeField] private TMP_Text _inventoryStatusText;

    [Header("Events")]
    public UnityEvent OnTabletOpened;
    public UnityEvent OnTabletClosed;

    private bool _isOpen = false;
    public static bool IsOpen { get; private set; }

    public static TabletUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _tabletPanel.SetActive(false);
        IsOpen = false;
    }

    private void Update()
    {
        if (_isOpen) UpdateContent();
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.actions.Player.Research.performed += OnResearchPerformed;

        if (Inventory.Instance != null)
            Inventory.Instance.OnInventoryChanged += OnInventoryChanged;
        if (PlayerTools.Instance != null)
            PlayerTools.Instance.OnToolsChanged += OnToolsChanged;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.actions.Player.Research.performed -= OnResearchPerformed;

        if (Inventory.Instance != null)
            Inventory.Instance.OnInventoryChanged -= OnInventoryChanged;
        if (PlayerTools.Instance != null)
            PlayerTools.Instance.OnToolsChanged -= OnToolsChanged;
    }

    private void OnResearchPerformed(InputAction.CallbackContext context) => ToggleTablet();

    public void ToggleTablet()
    {
        if (!PlayerTools.Instance.HasTool(ToolType.Tablet) ||
            ActiveTool.Instance.GetCurrentToolType() != ToolType.Tablet)
        {
            Debug.Log("Планшет не экипирован или отсутствует в инвентаре");
            return;
        }

        _isOpen = !_isOpen;
        IsOpen = _isOpen;
        _tabletPanel.SetActive(_isOpen);

        if (_isOpen)
        {
            UpdateContent();
            OnTabletOpened?.Invoke();
        }
        else
        {
            OnTabletClosed?.Invoke();
        }
    }

    /// <summary>
    /// Закрывает планшет, если он открыт.
    /// </summary>
    public void CloseIfOpen()
    {
        if (_isOpen)
            ToggleTablet();
    }

    private void OnInventoryChanged()
    {
        if (_isOpen) UpdateContent();
    }

    private void OnToolsChanged()
    {
        if (_isOpen) UpdateContent();
    }

    private void UpdateContent()
    {
        // Отображаем количество мусора вместо списка
        _trashCountText.text = $"{Inventory.Instance.ItemsCount} / {Inventory.Instance.MaxSlots}";
        _inventoryStatusText.text = "Мусор в инвентаре";

        // Список инструментов (оставляем как было)
        StringBuilder toolsSb = new StringBuilder();
        foreach (var tool in PlayerTools.Instance.GetAllTools())
        {
            toolsSb.AppendLine($"• {tool.ItemName} ({tool.ToolType})");
        }
        _toolsListText.text = toolsSb.Length > 0 ? toolsSb.ToString() : "Нет инструментов";
    }
}