using System.Collections.Generic;
using UnityEngine;
using Items;

public class TrashSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _spawnCount = 20;
    [SerializeField] private TextAsset _trashJson;

    [Header("Item Type Mapping")]
    [SerializeField] private ItemTypeSO[] _itemTypes;

    [Header("Prefabs Folder")]
    [SerializeField] private string _prefabsFolder = "TrashPrefabs";

    private Dictionary<string, ItemTypeSO> _typeMap = new Dictionary<string, ItemTypeSO>();
    private List<TrashData> _availableTrash = new List<TrashData>();

    private void Awake()
    {
        BuildTypeMap();
        LoadTrashData();
        SpawnTrash();
    }

    private void BuildTypeMap()
    {
        foreach (var type in _itemTypes)
        {
            _typeMap[type.TypeName] = type;
        }
    }

    private void LoadTrashData()
    {
        if (_trashJson == null)
        {
            Debug.LogError("JSON файл не назначен!");
            return;
        }

        TrashDataList wrapper = JsonUtility.FromJson<TrashDataList>(_trashJson.text);
        _availableTrash = new List<TrashData>(wrapper.items);
        Debug.Log($"Загружено {_availableTrash.Count} записей мусора.");
    }

    private void SpawnTrash()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogError("Нет точек спавна!");
            return;
        }

        if (_availableTrash.Count == 0)
        {
            Debug.LogError("Нет данных о мусоре для спавна!");
            return;
        }

        // Спавним столько, сколько указано в _spawnCount, но не больше чем точек спавна
        int count = Mathf.Min(_spawnCount, _spawnPoints.Length);
        if (count <= 0) return;

        // Перемешиваем точки спавна для вариативности
        List<Transform> shuffledPoints = new List<Transform>(_spawnPoints);
        Shuffle(shuffledPoints);

        for (int i = 0; i < count; i++)
        {
            Transform point = shuffledPoints[i];
            
            // 🔥 Выбираем случайный предмет из доступных (с возможностью повторений)
            TrashData data = _availableTrash[Random.Range(0, _availableTrash.Count)];

            string prefabPath = $"{_prefabsFolder}/{data.id}";
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Префаб не найден: {prefabPath}. Пропускаем.");
                continue;
            }

            GameObject trashGO = Instantiate(prefab, point.position, point.rotation);
            TrashItem trashItem = trashGO.GetComponent<TrashItem>();
            if (trashItem == null)
                trashItem = trashGO.AddComponent<TrashItem>();

            ItemTypeSO typeSO = GetTypeSO(data.type);
            trashItem.SetData(data.id, data.name, typeSO, data.description, data.lore);
        }

        Debug.Log($"Спавнено {count} предметов мусора (из {_availableTrash.Count} уникальных типов).");
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private ItemTypeSO GetTypeSO(string typeName)
    {
        if (_typeMap.TryGetValue(typeName, out ItemTypeSO so))
            return so;
        Debug.LogWarning($"Не найден ItemTypeSO для типа {typeName}");
        return null;
    }
}