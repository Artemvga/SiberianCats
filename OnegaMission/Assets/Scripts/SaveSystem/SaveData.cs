using System;
using System.Collections.Generic;
using UnityEngine;

// -----------------------------------------------------------------------------
// Назначение файла: SaveData.cs
// Путь: Assets/Scripts/SaveSystem/SaveData.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

[Serializable]
/// <summary>
/// Реализует компонент `SaveData` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class SaveData
{
    public string sceneName;
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public int score;

    // Для мусора – сохраняем JSON-идентификаторы
    public List<string> trashIDs = new List<string>();

    // Для инструментов – сохраняем строки ToolType
    public List<string> toolTypes = new List<string>();

    public string lastSavePoint;
}