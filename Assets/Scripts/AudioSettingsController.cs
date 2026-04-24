using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Фоновая музыка")]
    [SerializeField] private AudioSource _musicSource;

    [Header("Элементы UI")]
    [SerializeField] private Toggle _masterMuteToggle;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _masterVolumeSlider;

    private bool _isMuted = false;

    private void Start()
    {
        if (_masterMuteToggle != null)
        {
            _masterMuteToggle.isOn = false;
            _masterMuteToggle.onValueChanged.AddListener(HandleMasterMuteChange);
        }

        PlayMusic();

        SetupSlider(_sfxVolumeSlider);
        SetupSlider(_musicVolumeSlider);
        SetupSlider(_masterVolumeSlider);

        UpdateAllVolumes(1f);
    }

    private void PlayMusic()
    {
        if (_musicSource != null && _musicSource.clip != null)
        {
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.Play();
            Debug.Log("Фоновая музыка запущена");
        }
        else
        {
            Debug.LogWarning("MusicSource или AudioClip не назначен!");
        }
    }

    private void SetupSlider(Slider slider)
    {
        if (slider != null)
        {
            slider.minValue = 0.0001f;
            slider.maxValue = 1f;
            slider.value = 1f;
            slider.onValueChanged.AddListener(UpdateAllVolumes);
        }
    }

    private void HandleMasterMuteChange(bool isOn)
    {
        _isMuted = isOn;

        if (_audioMixer != null)
        {
            if (_isMuted)
            {
                _audioMixer.SetFloat("MasterVolume", -80f);

                SetSlidersInteractable(false);
                Debug.Log("🔇 Весь звук выключен (Master Volume = -80 dB)");
            }
            else
            {
                _audioMixer.SetFloat("MasterVolume", 0f);

                SetSlidersInteractable(true);
                UpdateAllVolumes(0);
                Debug.Log("🔊 Звук включен (Master Volume = 0 dB)");
            }
        }
    }

    private void SetSlidersInteractable(bool interactable)
    {
        if (_sfxVolumeSlider != null)
            _sfxVolumeSlider.interactable = interactable;

        if (_musicVolumeSlider != null)
            _musicVolumeSlider.interactable = interactable;

        if (_masterVolumeSlider != null)
            _masterVolumeSlider.interactable = interactable;
    }

    private void UpdateAllVolumes(float _ = 0)
    {
        if (_audioMixer == null)
            return;

        if (_isMuted)
            return;

        float sfx = _sfxVolumeSlider != null ? _sfxVolumeSlider.value : 1f;
        float music = _musicVolumeSlider != null ? _musicVolumeSlider.value : 1f;
        float master = _masterVolumeSlider != null ? _masterVolumeSlider.value : 1f;

        float sfxLevel = sfx * master;
        float musicLevel = music * master;

        float sfxDB = LinearToDecibel(sfxLevel);
        float musicDB = LinearToDecibel(musicLevel);

        _audioMixer.SetFloat("SFXVolume", sfxDB);
        _audioMixer.SetFloat("MusicVolume", musicDB);

        Debug.Log($"Громкость: SFX={sfxDB:F1}dB, Music={musicDB:F1}dB");
    }

    private float LinearToDecibel(float linear)
    {
        if (linear <= 0f)
            return -80f;

        return Mathf.Log10(linear) * 20f;
    }
}