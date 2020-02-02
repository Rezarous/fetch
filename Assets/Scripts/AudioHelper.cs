using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHelper : MonoBehaviour
{
    void Start() {
        Initialize();
    }

    private static void Initialize() {
        AudioInside = GameObject.FindGameObjectWithTag("AudioPlayerInside").GetComponent<AudioSource>();
        AudioOutside = GameObject.FindGameObjectWithTag("AudioPlayerOutside").GetComponent<AudioSource>();
        startVolumes = new Dictionary<AudioSource, float>();
    }

    private static AudioSource AudioInside;
    private static AudioSource AudioOutside;

    private static Dictionary<AudioSource, float> startVolumes;

    public static void PlayOutside(AudioClip audioClip, float pitchJitter = 0f, float minVolume = 0f) {
        Play(AudioOutside, audioClip, pitchJitter, minVolume);
    }
    public static void PlayInside(AudioClip audioClip, float pitchJitter = 0f, float minVolume = 0f) {
        Play(AudioInside, audioClip, pitchJitter, minVolume);
    }
    private static void Play(AudioSource source, AudioClip audioClip, float pitchJitter = 0f, float minVolume = 1f) {
        float oldPitch = source.pitch;
        if (pitchJitter > 0f)
            source.pitch = Random.Range(-pitchJitter, pitchJitter);

        source.PlayOneShot(audioClip, minVolume < 1f && minVolume >= 0f ? Random.Range(minVolume, 1f) : 1f);
        source.pitch = oldPitch;
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;
        if (!startVolumes.ContainsKey(audioSource))
            startVolumes[audioSource] = startVolume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
    }
    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime) {
        float startVolume = startVolumes.ContainsKey(audioSource) ? startVolumes[audioSource] : audioSource.volume;

        audioSource.Play();
        audioSource.volume = 0f;
        while (audioSource.volume < startVolume) {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
    }
}