using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown _qualityDropdown;
    [SerializeField] private Toggle _fullscreenToggle;

    [Header("Audio")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

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

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
    }

    public void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt("Fullscreen", isFull ? 1 : 0);
    }

    public void SetMasterVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.MasterVolume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.MusicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSfxVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SfxVolume = value;
        PlayerPrefs.SetFloat("SfxVolume", value);
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}