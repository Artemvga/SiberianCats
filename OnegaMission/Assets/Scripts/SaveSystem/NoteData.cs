[System.Serializable]
public class NoteData
{
    public string id;
    public string title;
    public string author;
    public string text;
}

[System.Serializable]
public class NoteDataList
{
    public NoteData[] notes;
}