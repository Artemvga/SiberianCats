using UnityEngine;
using TMPro;

// -----------------------------------------------------------------------------
// Назначение файла: ScoreUI.cs
// Путь: Assets/Scripts/Game/ScoreUI.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `ScoreUI` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _scoreText;
    
    private int _currentScore = 0;

    /// <summary>
    /// Запускает начальную настройку после инициализации сцены.
    /// </summary>
    private void Start()
    {
        // Проверка на наличие текстового поля
        if (_scoreText == null)
        {
            Debug.LogWarning("ScoreUI: _scoreText не назначен в Inspector!");
        }
    }

    /// <summary>
    /// Выполняет логику, которая должна обновляться каждый кадр.
    /// </summary>
    private void Update()
    {
        // Проверяем, существует ли GameManager
        if (GameManager.Instance == null) return;
        
        // Проверяем, изменилось ли значение счёта
        if (GameManager.Instance.CurrentScore != _currentScore)
        {
            _currentScore = GameManager.Instance.CurrentScore;
            UpdateScoreDisplay();
        }
    }

    /// <summary>
    /// Выполняет операцию `UpdateScoreDisplay` в рамках обязанностей текущего компонента.
    /// </summary>
    private void UpdateScoreDisplay()
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Счёт: {_currentScore.ToString()}";
        }
    }
}