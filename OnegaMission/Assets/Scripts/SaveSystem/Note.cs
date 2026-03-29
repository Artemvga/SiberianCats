using Items;
using UnityEngine;
using Player;

public class Note : MonoBehaviour, IInteractable
{
    [Header("Note Data")]
    [SerializeField] private string _noteId;
    [SerializeField] private TextAsset _notesJson;

    private NoteData _cachedNote;

    public string ItemName => _cachedNote?.title ?? "Записка";
    public string ItemType => "Записка";
    public string Description => "Старый листок бумаги";
    public string Lore => "";
    public bool ShouldShowRequirement => true;

    private void Awake()
    {
        LoadNoteData();
    }

    private void LoadNoteData()
    {
        if (_notesJson == null)
        {
            Debug.LogError("JSON с записками не назначен!");
            return;
        }

        var wrapper = JsonUtility.FromJson<NoteDataList>(_notesJson.text);
        foreach (var note in wrapper.notes)
        {
            if (note.id == _noteId)
            {
                _cachedNote = note;
                break;
            }
        }

        if (_cachedNote == null)
            Debug.LogError($"Записка с id {_noteId} не найдена в JSON");
    }

    public void OnFocus() { }
    public void OnDefocus() { }

    public bool CanInteract(PlayerTools tools) => true;

    public void Interact()
    {
        if (_cachedNote != null)
            NoteUI.Instance.Show(_cachedNote);
        else
            Debug.LogWarning($"Записка {_noteId} не загружена");
    }

    public string GetInteractionMessage()
    {
        return "Нажмите E, чтобы прочитать записку";
    }
}