using UnityEngine;
using UnityEngine.Events;

// -----------------------------------------------------------------------------
// Назначение файла: GameManager.cs
// Путь: Assets/Scripts/Game/GameManager.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `GameManager` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private int _currentScore = 0;
    public int CurrentScore => _currentScore;

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    /// <summary>
    /// Выполняет операцию `Init` в рамках обязанностей текущего компонента.
    /// </summary>
    public static void Init()
    {
        if (Instance != null) return;
        GameObject go = new GameObject("GameManager");
        Instance = go.AddComponent<GameManager>();
        DontDestroyOnLoad(go);
    }

    /// <summary>
    /// Инициализирует объект при создании компонента Unity.
    /// </summary>
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

    /// <summary>
    /// Выполняет операцию `AddScore` в рамках обязанностей текущего компонента.
    /// </summary>
    public void AddScore(int amount)
    {
        _currentScore += amount;
        OnScoreChanged?.Invoke(_currentScore); // ✅ Важно!
    }

    /// <summary>
    /// Выполняет операцию `SetScore` в рамках обязанностей текущего компонента.
    /// </summary>
    public void SetScore(int value)
    {
        _currentScore = value;
        OnScoreChanged?.Invoke(_currentScore);
    }

    /// <summary>
    /// Выполняет операцию `ResetScore` в рамках обязанностей текущего компонента.
    /// </summary>
    public void ResetScore()
    {
        _currentScore = 0;
        OnScoreChanged?.Invoke(_currentScore);
    }
}