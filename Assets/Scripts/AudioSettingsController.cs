using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Аудиоисточники")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource1;
    [SerializeField] private AudioSource _sfxSource2;
    [SerializeField] private AudioSource _sfxSource3;

    [Header("Элементы UI")]
    [SerializeField] private Button _button1;
    [SerializeField] private Button _button2;
    [SerializeField] private Button _button3;
    [SerializeField] private Toggle _masterMuteToggle;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _masterVolumeSlider;

    private bool _isMuted = false;

    private void Start()
    {
        Debug.Log("=== AudioSettingsController запущен ===");

        if (_musicSource != null && _musicSource.clip != null)
        {
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.Play();
            Debug.Log("Фоновая музыка запущена");
        }

        if (_button1 != null) 
            _button1.onClick.AddListener(() => PlaySFX(_sfxSource1));

        if (_button2 != null) 
            _button2.onClick.AddListener(() => PlaySFX(_sfxSource2));

        if (_button3 != null) 
            _button3.onClick.AddListener(() => PlaySFX(_sfxSource3));

        if (_masterMuteToggle != null)
        {
            _masterMuteToggle.isOn = false;
            _masterMuteToggle.onValueChanged.AddListener(HandleMasterMuteChange);
        }

        if (_sfxVolumeSlider != null)
        {
            _sfxVolumeSlider.minValue = 0.0001f;
            _sfxVolumeSlider.maxValue = 1f;
            _sfxVolumeSlider.value = 1f;
            _sfxVolumeSlider.onValueChanged.AddListener(UpdateAllVolumes);
        }

        if (_musicVolumeSlider != null)
        {
            _musicVolumeSlider.minValue = 0.0001f;
            _musicVolumeSlider.maxValue = 1f;
            _musicVolumeSlider.value = 1f;
            _musicVolumeSlider.onValueChanged.AddListener(UpdateAllVolumes);
        }

        if (_masterVolumeSlider != null)
        {
            _masterVolumeSlider.minValue = 0.0001f;
            _masterVolumeSlider.maxValue = 1f;
            _masterVolumeSlider.value = 1f;
            _masterVolumeSlider.onValueChanged.AddListener(UpdateAllVolumes);
        }

        UpdateAllVolumes(1f);
    }

    private void PlaySFX(AudioSource source)
    {
        if (source != null && source.clip != null)
        {
            source.Play();
            Debug.Log($"SFX проигран: {source.clip.name}");
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

                if (_sfxVolumeSlider != null) 
                    _sfxVolumeSlider.interactable = false;

                if (_musicVolumeSlider != null) 
                    _musicVolumeSlider.interactable = false;

                if (_masterVolumeSlider != null) 
                    _masterVolumeSlider.interactable = false;

                Debug.Log("🔇 Весь звук выключен (Master Volume = -80 dB)");
            }
            else
            {
                _audioMixer.SetFloat("MasterVolume", 0f);

                if (_sfxVolumeSlider != null) 
                    _sfxVolumeSlider.interactable = true;

                if (_musicVolumeSlider != null) 
                    _musicVolumeSlider.interactable = true;

                if (_masterVolumeSlider != null) 
                    _masterVolumeSlider.interactable = true;

                UpdateAllVolumes(0);

                Debug.Log("🔊 Звук включен (Master Volume = 0 dB)");
            }
        }
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