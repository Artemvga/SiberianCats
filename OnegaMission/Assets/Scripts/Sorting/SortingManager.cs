using System.Collections.Generic;
using Items;
using Player;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Менеджер процесса сортировки.
/// Забирает предметы из инвентаря, спавнит на столе,
/// обрабатывает клики по контейнерам, начисляет очки.
/// </summary>
public class SortingManager : MonoBehaviour
{
    public static SortingManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform _spawnPoint;          // точка появления предмета на столе
    [SerializeField] private Camera _sortingCamera;          // камера для лучей (если пусто – Camera.main)
    [SerializeField] private LayerMask _binLayer;            // слой контейнеров

    [Header("Score Settings")]
    [SerializeField] private int _pointsPerCorrect = 10;
    [SerializeField] private int _pointsPerWrong = -5;

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;       // локальный счёт сессии
    public UnityEvent<int> OnItemsLeftChanged;   // оставшиеся предметы
    public UnityEvent OnCorrectSort;             // правильная сортировка
    public UnityEvent OnWrongSort;               // неправильная
    public UnityEvent OnSortingStarted;          // старт
    public UnityEvent OnSortingEnded;            // конец

    private int _currentScore;                       // счёт текущей сессии
    private List<InteractableBase> _remainingItems;  // предметы, ожидающие сортировки
    private InteractableBase _currentItem;           // текущий предмет на столе
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

        _remainingItems = Inventory.Instance.TakeAllItems();
        OnItemsLeftChanged?.Invoke(_remainingItems.Count);
        OnSortingStarted?.Invoke();

        SpawnNextItem();
    }

    public void StopSorting()
    {
        _isSortingActive = false;

        if (_currentItem != null)
        {
            _currentItem.gameObject.SetActive(false);
            _remainingItems.Insert(0, _currentItem);
            _currentItem = null;
        }

        if (_remainingItems != null && _remainingItems.Count > 0)
            Inventory.Instance.AddItems(_remainingItems);

        _remainingItems = null;
        OnSortingEnded?.Invoke();
    }

    private void SpawnNextItem()
    {
        if (_remainingItems == null || _remainingItems.Count == 0)
        {
            _currentTable.StopSorting();
            return;
        }

        _currentItem = _remainingItems[0];
        _remainingItems.RemoveAt(0);

        _currentItem.gameObject.SetActive(true);
        _currentItem.transform.position = _spawnPoint.position;
        _currentItem.transform.rotation = _spawnPoint.rotation;

        // Отключаем подсветку и физику
        var outlinable = _currentItem.GetComponent<EPOOutline.Outlinable>();
        if (outlinable != null) outlinable.enabled = false;

        var rb = _currentItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        OnItemsLeftChanged?.Invoke(_remainingItems.Count);
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