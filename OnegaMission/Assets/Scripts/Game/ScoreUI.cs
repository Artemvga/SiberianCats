using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _scoreText;
    
    private int _currentScore = 0;

    private void Start()
    {
        // Проверка на наличие текстового поля
        if (_scoreText == null)
        {
            Debug.LogWarning("ScoreUI: _scoreText не назначен в Inspector!");
        }
    }

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

    private void UpdateScoreDisplay()
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Счёт: {_currentScore.ToString()}";
        }
    }
}