using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour
{
    public static NoteUI Instance { get; private set; }
    public static bool IsActive => _instance != null && _instance._panel.activeSelf;

    [Header("UI Elements")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _authorText;
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private Button _closeButton;

    private static NoteUI _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        Instance = this;
        _panel.SetActive(false);
        _closeButton.onClick.AddListener(Hide);
    }

    public void Show(NoteData note)
    {
        _titleText.text = note.title;
        _authorText.text = note.author;
        _contentText.text = note.text;
        _panel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (PlayerController.Instance != null)
            PlayerController.Instance.enabled = false;
    }

    public void Hide()
    {
        _panel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (PlayerController.Instance != null)
            PlayerController.Instance.enabled = true;
    }
}