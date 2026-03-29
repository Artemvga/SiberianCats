using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _settingsPanel;

    private void Start()
    {
        _mainPanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }

    public void NewGame()
    {
        SaveManager.Instance.NewGame();
    }

    public void Continue()
    {
        if (SaveManager.Instance.HasSave())
            SaveManager.Instance.LoadGame();
        else
            Debug.Log("Нет сохранений");
    }

    public void OpenSettings()
    {
        _mainPanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        _settingsPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}