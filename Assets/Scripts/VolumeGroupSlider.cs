using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(Slider))]
public class VolumeGroupSlider : MonoBehaviour
{
    [SerializeField] private string _exposedParameterName = "Volume";
    [SerializeField] private float _minDB = -80f;

    private Slider _slider;
    private AudioMixer _audioMixer;
    //private float _masterVolume = 1f;
    //private bool _isInitialized;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.minValue = 0f;
        _slider.maxValue = 1f;
        _slider.value = 1f;

        _slider.onValueChanged.RemoveAllListeners();
    }

    public void Initialize(AudioMixer mixer)
    {
        _audioMixer = mixer;
        //_isInitialized = true;

        _slider.onValueChanged.AddListener(UpdateVolume);
        UpdateVolume(_slider.value); 
    }

    private void OnDestroy()
    {
        if (_slider != null)
            _slider.onValueChanged.RemoveListener(UpdateVolume);
    }

    private void UpdateVolume(float value)
    {
        if (_audioMixer == null) 
            return;

        float db = LinearToDecibel(_slider.value);
        _audioMixer.SetFloat(_exposedParameterName, db);
    }

    public void SetInteractable(bool interactable)
    {
        if (_slider != null)
            _slider.interactable = interactable;
    }

    private float LinearToDecibel(float linear)
    {
        if (linear <= 0f) 
            return _minDB;

        return Mathf.Log10(linear) * 20f;
    }
}