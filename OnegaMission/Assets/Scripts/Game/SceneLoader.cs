using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Loading Screen")]
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Text _progressText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
        _loadingPanel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

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