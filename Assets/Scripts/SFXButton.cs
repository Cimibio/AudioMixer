using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class SFXButton : MonoBehaviour
{
    private Button _button;
    private AudioSource _audioSource;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _audioSource = GetComponent<AudioSource>();

        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(PlaySFX);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(PlaySFX);
    }

    private void PlaySFX()
    {
        if (_audioSource.clip != null)
        {
            _audioSource.Play();
            Debug.Log($"SFX проигран: {_audioSource.clip.name}");
        }
        else
        {
            Debug.LogWarning($"SFXButton на {gameObject.name}: AudioSource или AudioClip не назначен!");
        }
    }
}