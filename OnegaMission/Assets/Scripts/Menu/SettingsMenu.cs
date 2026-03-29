using UnityEngine;
using UnityEngine.UI;
using TMPro;

// -----------------------------------------------------------------------------
// Назначение файла: SettingsMenu.cs
// Путь: Assets/Scripts/Menu/SettingsMenu.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `SettingsMenu` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown _qualityDropdown;
    [SerializeField] private Toggle _fullscreenToggle;

    [Header("Audio")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    /// <summary>
    /// Запускает начальную настройку после инициализации сцены.
    /// </summary>
    private void Start()
    {
        // Загружаем настройки из PlayerPrefs
        _qualityDropdown.value = QualitySettings.GetQualityLevel();
        _fullscreenToggle.isOn = Screen.fullScreen;

        if (SoundManager.Instance != null)
        {
            _masterVolumeSlider.value = SoundManager.Instance.MasterVolume;
            _musicVolumeSlider.value = SoundManager.Instance.MusicVolume;
            _sfxVolumeSlider.value = SoundManager.Instance.SfxVolume;
        }

        _qualityDropdown.onValueChanged.AddListener(SetQuality);
        _fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        _masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    /// <summary>
    /// Выполняет операцию `SetQuality` в рамках обязанностей текущего компонента.
    /// </summary>
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
    }

    /// <summary>
    /// Выполняет операцию `SetFullscreen` в рамках обязанностей текущего компонента.
    /// </summary>
    public void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt("Fullscreen", isFull ? 1 : 0);
    }

    /// <summary>
    /// Выполняет операцию `SetMasterVolume` в рамках обязанностей текущего компонента.
    /// </summary>
    public void SetMasterVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.MasterVolume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    /// <summary>
    /// Выполняет операцию `SetMusicVolume` в рамках обязанностей текущего компонента.
    /// </summary>
    public void SetMusicVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.MusicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    /// <summary>
    /// Выполняет операцию `SetSfxVolume` в рамках обязанностей текущего компонента.
    /// </summary>
    public void SetSfxVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SfxVolume = value;
        PlayerPrefs.SetFloat("SfxVolume", value);
    }

    /// <summary>
    /// Освобождает ресурсы перед уничтожением объекта.
    /// </summary>
    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}