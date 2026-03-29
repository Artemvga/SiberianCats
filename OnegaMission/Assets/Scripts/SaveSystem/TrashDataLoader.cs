using System.Collections.Generic;
using UnityEngine;
using Items;

// -----------------------------------------------------------------------------
// Назначение файла: TrashDataLoader.cs
// Путь: Assets/Scripts/SaveSystem/TrashDataLoader.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `TrashDataLoader` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public static class TrashDataLoader
{
    private static Dictionary<string, TrashData> _dataMap;
    private static Dictionary<string, ItemTypeSO> _typeMap;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    /// <summary>
    /// Выполняет операцию `LoadData` в рамках обязанностей текущего компонента.
    /// </summary>
    private static void LoadData()
    {
        _dataMap = new Dictionary<string, TrashData>();
        _typeMap = new Dictionary<string, ItemTypeSO>();

        // Загружаем JSON
        TextAsset jsonFile = Resources.Load<TextAsset>("DataTrash/TrashData");
        if (jsonFile == null)
        {
            Debug.LogError("JSON файл TrashData не найден в Resources/DataTrash");
            return;
        }

        TrashDataList wrapper = JsonUtility.FromJson<TrashDataList>(jsonFile.text);
        foreach (var item in wrapper.items)
        {
            _dataMap[item.id] = item;
        }

        // Загружаем ItemTypeSO (из папки Resources/ItemTypes)
        ItemTypeSO[] types = Resources.LoadAll<ItemTypeSO>("ItemTypes");
        foreach (var type in types)
        {
            _typeMap[type.TypeName] = type;
        }
    }

    /// <summary>
    /// Выполняет операцию `GetDataById` в рамках обязанностей текущего компонента.
    /// </summary>
    public static TrashData GetDataById(string id)
    {
        _dataMap.TryGetValue(id, out TrashData data);
        return data;
    }

    /// <summary>
    /// Выполняет операцию `GetItemTypeSO` в рамках обязанностей текущего компонента.
    /// </summary>
    public static ItemTypeSO GetItemTypeSO(string typeName)
    {
        _typeMap.TryGetValue(typeName, out ItemTypeSO so);
        return so;
    }
}