using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _musicSource;

    [Header("Элементы UI")]
    [SerializeField] private Toggle _masterMuteToggle;
    [SerializeField] private List<VolumeGroupSlider> _volumeGroups;

    private float _minVolume = 0f;
    private float _maxVolume = 1f;

    private void Start()
    {
        InitializeVolumeGroups();

        if (_masterMuteToggle != null)
        {
            _masterMuteToggle.isOn = false;
            _masterMuteToggle.onValueChanged.AddListener(ToggleMute);
        }

        PlayMusic();
    }

    private void InitializeVolumeGroups()
    {
        foreach (var group in _volumeGroups)        
            if (group != null)            
                group.Initialize(_audioMixer);
    }

    private void ToggleMute(bool isOn)
    {
        AudioListener.volume = isOn ? _minVolume : _maxVolume;

        foreach (var group in _volumeGroups)
            group?.SetInteractable(!isOn);

        Debug.Log($"Master Mute: {isOn}");
    }

    private void PlayMusic()
    {
        if (_musicSource != null && _musicSource.clip != null)
        {
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.Play();
            Debug.Log("Background music started");
        }
        else
        {
            Debug.LogWarning("MusicSource or AudioClip is not assigned!");
        }
    }
}