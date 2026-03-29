using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// -----------------------------------------------------------------------------
// Назначение файла: SceneLoader.cs
// Путь: Assets/Scripts/Game/SceneLoader.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `SceneLoader` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Loading Screen")]
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Text _progressText;

    /// <summary>
    /// Инициализирует объект при создании компонента Unity.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
        _loadingPanel.SetActive(false);
    }

    /// <summary>
    /// Выполняет операцию `LoadScene` в рамках обязанностей текущего компонента.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Выполняет операцию `LoadSceneAsync` в рамках обязанностей текущего компонента.
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        _loadingPanel.SetActive(true);
        _progressBar.value = 0;
        _progressText.text = "0%";

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            _progressBar.value = progress;
            _progressText.text = (progress * 100).ToString("F0") + "%";

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        _loadingPanel.SetActive(false);

        // После загрузки сцены восстанавливаем позицию игрока из сохранения
        if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.transform.position = SaveManager.Instance.CurrentSave.playerPosition;
                player.transform.rotation = SaveManager.Instance.CurrentSave.playerRotation;
            }
        }
    }
}