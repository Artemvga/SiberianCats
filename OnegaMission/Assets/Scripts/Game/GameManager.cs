using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private int _currentScore = 0;
    public int CurrentScore => _currentScore;

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        if (Instance != null) return;
        GameObject go = new GameObject("GameManager");
        Instance = go.AddComponent<GameManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int amount)
    {
        _currentScore += amount;
        OnScoreChanged?.Invoke(_currentScore);
    }

    public void SetScore(int value)
    {
        _currentScore = value;
        OnScoreChanged?.Invoke(_currentScore);
    }

    public void ResetScore()
    {
        _currentScore = 0;
        OnScoreChanged?.Invoke(_currentScore);
    }
}