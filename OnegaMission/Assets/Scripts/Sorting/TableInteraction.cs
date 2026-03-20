using EPOOutline;
using Items;
using Player;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Компонент для стола, реализующий IInteractable.
/// Позволяет активировать стол нажатием E, если у игрока есть планшет.
/// </summary>
public class TableInteraction : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private SortingTable _sortingTable;
    [SerializeField] private string _itemName = "Стол сортировки";
    [SerializeField] private string _itemType = "Мебель";
    [SerializeField] [TextArea] private string _description = "Стол для сортировки мусора";
    [SerializeField] [TextArea] private string _lore = "";

    [Header("Events")]
    public UnityEvent OnFocusEvent;
    public UnityEvent OnDefocusEvent;
    public UnityEvent OnInteractEvent;

    private Outlinable _outlinable;

    public string ItemName => _itemName;
    public string ItemType => _itemType;
    public string Description => _description;
    public string Lore => _lore;
    public bool ShouldShowRequirement => true;

    private void Start()
    {
        _outlinable = GetComponent<Outlinable>();
        if (_outlinable == null)
            _outlinable = gameObject.AddComponent<Outlinable>();

        _outlinable.RenderStyle = RenderStyle.FrontBack;
        _outlinable.FrontParameters.Color = Color.cyan;
        _outlinable.BackParameters.Color = Color.blue;
        _outlinable.OutlineParameters.DilateShift = 2f;
        _outlinable.enabled = false;
    }

    public void OnFocus()
    {
        if (_outlinable != null) _outlinable.enabled = true;
        OnFocusEvent?.Invoke();
    }

    public void OnDefocus()
    {
        if (_outlinable != null) _outlinable.enabled = false;
        OnDefocusEvent?.Invoke();
    }

    public bool CanInteract(PlayerTools tools) => tools.HasTool(ToolType.Tablet);

    public void Interact()
    {
        _sortingTable.TryStartSorting();
        OnInteractEvent?.Invoke();
    }
}