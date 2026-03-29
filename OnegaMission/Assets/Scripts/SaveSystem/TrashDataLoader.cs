using System.Collections.Generic;
using UnityEngine;
using Items;

public static class TrashDataLoader
{
    private static Dictionary<string, TrashData> _dataMap;
    private static Dictionary<string, ItemTypeSO> _typeMap;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
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

    public static TrashData GetDataById(string id)
    {
        _dataMap.TryGetValue(id, out TrashData data);
        return data;
    }

    public static ItemTypeSO GetItemTypeSO(string typeName)
    {
        _typeMap.TryGetValue(typeName, out ItemTypeSO so);
        return so;
    }
}