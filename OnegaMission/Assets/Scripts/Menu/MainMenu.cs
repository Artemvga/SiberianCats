using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------
// Назначение файла: MainMenu.cs
// Путь: Assets/Scripts/Menu/MainMenu.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `MainMenu` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _settingsPanel;

    /// <summary>
    /// Запускает начальную настройку после инициализации сцены.
    /// </summary>
    private void Start()
    {
        _mainPanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Выполняет операцию `NewGame` в рамках обязанностей текущего компонента.
    /// </summary>
    public void NewGame()
    {
        SaveManager.Instance.NewGame();
    }

    /// <summary>
    /// Выполняет операцию `Continue` в рамках обязанностей текущего компонента.
    /// </summary>
    public void Continue()
    {
        if (SaveManager.Instance.HasSave())
            SaveManager.Instance.LoadGame();
        else
            Debug.Log("Нет сохранений");
    }

    /// <summary>
    /// Выполняет операцию `OpenSettings` в рамках обязанностей текущего компонента.
    /// </summary>
    public void OpenSettings()
    {
        _mainPanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Выполняет операцию `CloseSettings` в рамках обязанностей текущего компонента.
    /// </summary>
    public void CloseSettings()
    {
        _settingsPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }

    /// <summary>
    /// Выполняет операцию `QuitGame` в рамках обязанностей текущего компонента.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}