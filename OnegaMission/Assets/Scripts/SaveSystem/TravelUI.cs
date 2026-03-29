using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------
// Назначение файла: TravelUI.cs
// Путь: Assets/Scripts/SaveSystem/TravelUI.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `TravelUI` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class TravelUI : MonoBehaviour
{
    public static TravelUI Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _bayButton;
    [SerializeField] private Button _baseButton;
    [SerializeField] private Button _shoreButton;
    [SerializeField] private Button _sealButton;
    [SerializeField] private Button _closeButton;

    /// <summary>
    /// Инициализирует объект при создании компонента Unity.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        _panel.SetActive(false);
    }

    /// <summary>
    /// Запускает начальную настройку после инициализации сцены.
    /// </summary>
    private void Start()
    {
        _bayButton.onClick.AddListener(() => LoadScene("Bay"));
        _baseButton.onClick.AddListener(() => LoadScene("Base"));
        _shoreButton.onClick.AddListener(() => LoadScene("Shore"));
        _sealButton.onClick.AddListener(() => LoadScene("SealColony"));
        _closeButton.onClick.AddListener(Hide);
    }

    /// <summary>
    /// Выполняет операцию `Show` в рамках обязанностей текущего компонента.
    /// </summary>
    public void Show()
    {
        _panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        if (PlayerController.Instance != null)
            PlayerController.Instance.enabled = false;
    }

    /// <summary>
    /// Выполняет операцию `Hide` в рамках обязанностей текущего компонента.
    /// </summary>
    public void Hide()
    {
        _panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        if (PlayerController.Instance != null)
            PlayerController.Instance.enabled = true;
    }

    /// <summary>
    /// Выполняет операцию `LoadScene` в рамках обязанностей текущего компонента.
    /// </summary>
    private void LoadScene(string sceneName)
    {
        Hide();
        SceneLoader.Instance.LoadScene(sceneName);
    }
}