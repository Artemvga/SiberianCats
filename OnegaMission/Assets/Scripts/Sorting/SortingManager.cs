using System.Collections.Generic;
using Items;
using Player;
using UnityEngine;
using UnityEngine.Events;

public class SortingManager : MonoBehaviour
{
    public static SortingManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Camera _sortingCamera;
    [SerializeField] private LayerMask _binLayer;

    [Header("Score Settings")]
    [SerializeField] private int _pointsPerCorrect = 10;
    [SerializeField] private int _pointsPerWrong = -5;

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<int> OnItemsLeftChanged;
    public UnityEvent OnCorrectSort;
    public UnityEvent OnWrongSort;
    public UnityEvent OnSortingStarted;
    public UnityEvent OnSortingEnded;

    private int _currentScore;
    private List<InteractableBase> _remainingItems;
    private InteractableBase _currentItem;
    private SortingTable _currentTable;
    private bool _isSortingActive = false;

    private void Awake() => Instance = this;

    private void Update()
    {
        if (!_isSortingActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            Camera cam = _sortingCamera != null ? _sortingCamera : Camera.main;
            if (cam == null) return;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _binLayer))
            {
                Bin bin = hit.collider.GetComponent<Bin>();
                if (bin != null) CheckSort(bin.AcceptedType);
            }
        }
    }

    public void StartSorting(SortingTable table)
    {
        _currentTable = table;
        _currentScore = 0;
        _isSortingActive = true;
        OnScoreChanged?.Invoke(_currentScore);

        // Забираем ВСЕ предметы из инвентаря (включая деактивированные)
        _remainingItems = Inventory.Instance.TakeAllItems();
        Debug.Log($"Sorting started, items in inventory: {_remainingItems.Count}");
        OnItemsLeftChanged?.Invoke(_remainingItems.Count);
        OnSortingStarted?.Invoke();

        SpawnNextItem();
    }

    public void StopSorting()
    {
        _isSortingActive = false;

        // Возвращаем текущий предмет, если он есть
        if (_currentItem != null)
        {
            _currentItem.gameObject.SetActive(false);
            _remainingItems.Insert(0, _currentItem);
            _currentItem = null;
        }

        // Возвращаем все оставшиеся предметы в инвентарь
        if (_remainingItems != null && _remainingItems.Count > 0)
        {
            Inventory.Instance.AddItems(_remainingItems);
            Debug.Log($"Returned {_remainingItems.Count} items to inventory");
        }

        _remainingItems = null;
        OnSortingEnded?.Invoke();
    }

    private void SpawnNextItem()
    {
        if (_remainingItems == null || _remainingItems.Count == 0)
        {
            Debug.Log("No items left, stopping sorting");
            _currentTable.StopSorting();
            return;
        }

        _currentItem = _remainingItems[0];
        _remainingItems.RemoveAt(0);

        // Проверяем, что предмет ещё существует
        if (_currentItem == null || _currentItem.gameObject == null)
        {
            Debug.LogWarning("Item is null, skipping");
            SpawnNextItem();
            return;
        }

        // Активируем и размещаем предмет
        _currentItem.gameObject.SetActive(true);
        _currentItem.transform.position = _spawnPoint.position;
        _currentItem.transform.rotation = _spawnPoint.rotation;

        // Отключаем подсветку и физику
        var outlinable = _currentItem.GetComponent<EPOOutline.Outlinable>();
        if (outlinable != null) outlinable.enabled = false;

        var rb = _currentItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        OnItemsLeftChanged?.Invoke(_remainingItems.Count);
        Debug.Log($"Spawned item: {_currentItem.ItemName}, remaining: {_remainingItems.Count}");
    }

    public void CheckSort(ItemTypeSO binType)
    {
        if (_currentItem == null) return;

        var trash = _currentItem as TrashItem;
        if (trash == null) return;

        if (trash.ItemTypeSO == binType)
        {
            _currentScore += _pointsPerCorrect;
            OnScoreChanged?.Invoke(_currentScore);
            GameManager.Instance?.AddScore(_pointsPerCorrect);
            OnCorrectSort?.Invoke();

            // Уничтожаем предмет, он больше не нужен
            Destroy(_currentItem.gameObject);
            _currentItem = null;
            SpawnNextItem();
        }
        else
        {
            _currentScore += _pointsPerWrong;
            OnScoreChanged?.Invoke(_currentScore);
            GameManager.Instance?.AddScore(_pointsPerWrong);
            OnWrongSort?.Invoke();
            // предмет остаётся на столе
        }
    }
}