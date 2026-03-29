// -----------------------------------------------------------------------------
// Назначение файла: TrashData.cs
// Путь: Assets/Scripts/Data/TrashData.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

[System.Serializable]
/// <summary>
/// Реализует компонент `TrashData` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class TrashData
{
    public string id;
    public string name;
    public string type;
    public string description;
    public string lore;
}

[System.Serializable]
/// <summary>
/// Реализует компонент `TrashDataList` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class TrashDataList
{
    public TrashData[] items;
}