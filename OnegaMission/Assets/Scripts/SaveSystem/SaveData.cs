using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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