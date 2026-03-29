[System.Serializable]
public class TrashData
{
    public string id;
    public string name;
    public string type;
    public string description;
    public string lore;
}

[System.Serializable]
public class TrashDataList
{
    public TrashData[] items;
}