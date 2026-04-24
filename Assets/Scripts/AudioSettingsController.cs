using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Аудиоисточники")]
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private AudioSource sfxSource1;
    [SerializeField] private AudioSource sfxSource2;
    [SerializeField] private AudioSource sfxSource3;

    [Header("Элементы UI")]
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider masterSlider;

    private void Start()
    {
        Debug.Log("=== AudioSettingsController запущен ===");

        // Запуск фоновой музыки
        if (bgMusicSource != null && bgMusicSource.clip != null)
        {
            bgMusicSource.loop = true;
            bgMusicSource.playOnAwake = false;
            bgMusicSource.Play();
            Debug.Log("Фоновая музыка запущена");
        }

        // Привязка кнопок
        if (button1 != null) button1.onClick.AddListener(() => PlaySFX(sfxSource1));
        if (button2 != null) button2.onClick.AddListener(() => PlaySFX(sfxSource2));
        if (button3 != null) button3.onClick.AddListener(() => PlaySFX(sfxSource3));

        // Тоггл музыки
        if (musicToggle != null)
        {
            musicToggle.isOn = true;
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }

        // Настройка слайдеров
        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0.0001f;
            sfxSlider.maxValue = 1f;
            sfxSlider.value = 1f;
            sfxSlider.onValueChanged.AddListener(UpdateAllVolumes);
        }

        if (musicSlider != null)
        {
            musicSlider.minValue = 0.0001f;
            musicSlider.maxValue = 1f;
            musicSlider.value = 1f;
            musicSlider.onValueChanged.AddListener(UpdateAllVolumes);
        }

        if (masterSlider != null)
        {
            masterSlider.minValue = 0.0001f;
            masterSlider.maxValue = 1f;
            masterSlider.value = 1f;
            masterSlider.onValueChanged.AddListener(UpdateAllVolumes);
        }

        // Применяем начальные значения
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

    private void OnMusicToggleChanged(bool isOn)
    {
        if (bgMusicSource != null)
        {
            bgMusicSource.mute = !isOn;
            Debug.Log($"Музыка mute = {!isOn}");
        }
    }

    private void UpdateAllVolumes(float _ = 0)
    {
        if (audioMixer == null) return;

        float sfx = sfxSlider != null ? sfxSlider.value : 1f;
        float music = musicSlider != null ? musicSlider.value : 1f;
        float master = masterSlider != null ? masterSlider.value : 1f;

        // Итоговая громкость
        float sfxLevel = sfx * master;
        float musicLevel = music * master;

        // Конвертация в децибелы
        float sfxDB = LinearToDecibel(sfxLevel);
        float musicDB = LinearToDecibel(musicLevel);

        // Применяем через миксер
        audioMixer.SetFloat("SFXVolume", sfxDB);
        audioMixer.SetFloat("MusicVolume", musicDB);

        Debug.Log($"Громкость: SFX={sfxDB:F1}dB, Music={musicDB:F1}dB");
    }

    private float LinearToDecibel(float linear)
    {
        if (linear <= 0f) return -80f;
        return Mathf.Log10(linear) * 20f;
    }
}