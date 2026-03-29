using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Items;
using Player;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private string _saveFileName = "save.json";
    [SerializeField] private bool _autoLoadOnStart = false;

    private string _savePath;
    private SaveData _currentSave = new SaveData();

    public SaveData CurrentSave => _currentSave;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, _saveFileName);
    }

    private void Start()
    {
        if (_autoLoadOnStart && File.Exists(_savePath))
            LoadGame();
        else
            NewGame();
    }

    public void SaveGame(string savePointName = "")
    {
        _currentSave.sceneName = SceneManager.GetActiveScene().name;
        if (PlayerController.Instance != null)
        {
            _currentSave.playerPosition = PlayerController.Instance.transform.position;
            _currentSave.playerRotation = PlayerController.Instance.transform.rotation;
        }
        _currentSave.score = GameManager.Instance.CurrentScore;

        // Сохраняем мусор (TrashItem)
        _currentSave.trashIDs.Clear();
        foreach (var item in Inventory.Instance.GetItems())
        {
            TrashItem trash = item as TrashItem;
            if (trash != null)
            {
                _currentSave.trashIDs.Add(trash.Id);
            }
        }

        // Сохраняем инструменты (ToolItem)
        _currentSave.toolTypes.Clear();
        foreach (var tool in PlayerTools.Instance.GetAllTools())
        {
            _currentSave.toolTypes.Add(tool.ToolType.ToString());
        }

        if (!string.IsNullOrEmpty(savePointName))
            _currentSave.lastSavePoint = savePointName;

        string json = JsonUtility.ToJson(_currentSave, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"Игра сохранена в {_savePath}");
    }

    public void LoadGame()
    {
        if (!File.Exists(_savePath))
        {
            Debug.Log("Сохранение не найдено, запускаем новую игру");
            NewGame();
            return;
        }

        string json = File.ReadAllText(_savePath);
        _currentSave = JsonUtility.FromJson<SaveData>(json);

        // Очищаем текущие инвентари перед загрузкой
        Inventory.Instance.ClearInventory();
        PlayerTools.Instance.ClearTools();

        // Восстанавливаем инструменты
        foreach (string toolTypeStr in _currentSave.toolTypes)
        {
            if (System.Enum.TryParse(toolTypeStr, out ToolType toolType))
            {
                // Создаём инструмент по типу (нужен фабричный метод)
                ToolItem restoredTool = CreateToolByType(toolType);
                if (restoredTool != null)
                {
                    PlayerTools.Instance.AddTool(restoredTool);
                }
            }
        }

        // Восстанавливаем мусор
        foreach (string id in _currentSave.trashIDs)
        {
            TrashItem restoredTrash = CreateTrashById(id);
            if (restoredTrash != null)
            {
                Inventory.Instance.AddItem(restoredTrash);
            }
        }

        // Загружаем сцену
        SceneManager.LoadScene(_currentSave.sceneName);
    }

    public void NewGame()
    {
        _currentSave = new SaveData();
        _currentSave.sceneName = "Base";
        _currentSave.playerPosition = Vector3.zero;
        _currentSave.playerRotation = Quaternion.identity;
        _currentSave.score = 0;
        _currentSave.trashIDs.Clear();
        _currentSave.toolTypes.Clear();

        GameManager.Instance.ResetScore();
        Inventory.Instance.ClearInventory();
        PlayerTools.Instance.ClearTools();

        SceneManager.LoadScene("Base");
    }

    public bool HasSave() => File.Exists(_savePath);

    // Фабричный метод для создания инструмента по типу
    private ToolItem CreateToolByType(ToolType type)
    {
        string prefabName = type switch
        {
            ToolType.Tablet => "Tablet",
            ToolType.Scissors => "Scissors",
            ToolType.PhotoCamera => "PhotoCamera",
            _ => null
        };
        if (string.IsNullOrEmpty(prefabName)) return null;

        GameObject prefab = Resources.Load<GameObject>($"Tools/{prefabName}");
        if (prefab == null)
        {
            Debug.LogWarning($"Префаб для инструмента {type} не найден в Resources/Tools");
            return null;
        }

        GameObject instance = Instantiate(prefab);
        ToolItem tool = instance.GetComponent<ToolItem>();
        if (tool == null) tool = instance.AddComponent<ToolItem>();
        instance.SetActive(false); // инструмент будет храниться в PlayerTools без отображения в сцене
        return tool;
    }

    // Фабричный метод для создания мусора по ID
    private TrashItem CreateTrashById(string id)
    {
        GameObject prefab = Resources.Load<GameObject>($"TrashPrefabs/{id}");
        if (prefab == null)
        {
            Debug.LogWarning($"Префаб мусора с id {id} не найден в Resources/TrashPrefabs");
            return null;
        }

        GameObject instance = Instantiate(prefab);
        TrashItem trash = instance.GetComponent<TrashItem>();
        if (trash == null) trash = instance.AddComponent<TrashItem>();

        // Теперь нужно заполнить данные из JSON по id
        // Для этого можно загрузить JSON и найти запись с таким id.
        // Вместо повторного чтения файла можно иметь статическую таблицу, загруженную при старте.
        // Для простоты оставим логику загрузки данных в отдельном методе, который можно вызвать.
        // Реализуем метод LoadTrashDataFromId, который использует TrashDataLoader.
        TrashData data = TrashDataLoader.GetDataById(id);
        if (data != null)
        {
            ItemTypeSO typeSO = GetItemTypeSO(data.type);
            trash.SetData(data.id, data.name, typeSO, data.description, data.lore);
        }
        else
        {
            Debug.LogWarning($"Данные для мусора {id} не найдены в JSON");
        }

        instance.SetActive(false);
        return trash;
    }

    private ItemTypeSO GetItemTypeSO(string typeName)
    {
        // Здесь нужна глобальная таблица, как в TrashSpawner.
        // Можно сделать статический словарь в отдельном классе.
        return TrashDataLoader.GetItemTypeSO(typeName);
    }
}