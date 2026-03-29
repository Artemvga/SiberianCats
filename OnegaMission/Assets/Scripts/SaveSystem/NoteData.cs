// -----------------------------------------------------------------------------
// Назначение файла: NoteData.cs
// Путь: Assets/Scripts/SaveSystem/NoteData.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

[System.Serializable]
/// <summary>
/// Реализует компонент `NoteData` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class NoteData
{
    public string id;
    public string title;
    public string author;
    public string text;
}

[System.Serializable]
/// <summary>
/// Реализует компонент `NoteDataList` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class NoteDataList
{
    public NoteData[] notes;
}