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
    [SerializeField] private TMP_Text _trashListText;
    [SerializeField] private TMP_Text _toolsListText;
    [SerializeField] private TMP_Text _inventoryStatusText;

    [Header("Events")]
    public UnityEvent OnTabletOpened;
    public UnityEvent OnTabletClosed;

    private bool _isOpen = false;
    public static bool IsOpen { get; private set; }

    private void Awake()
    {
        _tabletPanel.SetActive(false);
        IsOpen = false;
    }
    
    private void Update()
    {
        if (_isOpen)
        {
            UpdateContent(); 
        }
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
        if (!PlayerTools.Instance.HasTool(ToolType.Tablet))
        {
            Debug.Log("У вас нет планшета!");
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

    private void OnInventoryChanged()
    {
        if (_isOpen) UpdateContent();
    }

    private void OnToolsChanged()
    {
        if (_isOpen) UpdateContent();
    }

    // Если события всё равно не работают, раскомментируйте этот метод:
    // private void Update()
    // {
    //     if (_isOpen) UpdateContent();
    // }

    private void UpdateContent()
    {
        _inventoryStatusText.text = $"Мусор: {Inventory.Instance.ItemsCount}/{Inventory.Instance.MaxSlots}";

        StringBuilder trashSb = new StringBuilder();
        foreach (var item in Inventory.Instance.GetItems())
        {
            trashSb.AppendLine($"• {item.ItemName} ({item.ItemType})");
        }
        _trashListText.text = trashSb.Length > 0 ? trashSb.ToString() : "Нет мусора";

        StringBuilder toolsSb = new StringBuilder();
        foreach (var tool in PlayerTools.Instance.GetAllTools())
        {
            toolsSb.AppendLine($"• {tool.ItemName} ({tool.ToolType})");
        }
        _toolsListText.text = toolsSb.Length > 0 ? toolsSb.ToString() : "Нет инструментов";
    }
}