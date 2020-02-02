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
        playsOnAwake = new Dictionary<AudioSource, bool>();
    }

    private static AudioSource AudioInside;
    private static AudioSource AudioOutside;

    private static Dictionary<AudioSource, float> startVolumes;
    private static Dictionary<AudioSource, bool> playsOnAwake;

    public static void PlayOutside(AudioClip audioClip, float pitchJitter = 0f, float minVolume = 0f, float maxVolume = 1f) {
        Play(AudioOutside, audioClip, pitchJitter, minVolume, maxVolume);
    }
    public static void PlayInside(AudioClip audioClip, float pitchJitter = 0f, float minVolume = 0f, float maxVolume = 1f) {
        Play(AudioInside, audioClip, pitchJitter, minVolume, maxVolume);
    }       
    private static void Play(AudioSource source, AudioClip audioClip, float pitchJitter = 0f, float minVolume = 1f, float maxVolume = 1f) {
        float oldPitch = source.pitch;
        if (pitchJitter > 0f)
            source.pitch = Random.Range(-pitchJitter, pitchJitter);

        source.PlayOneShot(audioClip, minVolume < 1f && minVolume >= 0f ? Random.Range(minVolume, maxVolume) : maxVolume);
        source.pitch = oldPitch;
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime) {
        Debug.Log($"fade OUT {audioSource?.clip?.name}");

        float startVolume = audioSource.volume;
        if (!startVolumes.ContainsKey(audioSource))
            startVolumes[audioSource] = startVolume;
        if (!playsOnAwake.ContainsKey(audioSource))
            playsOnAwake[audioSource] = audioSource.playOnAwake;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolumes[audioSource] * Time.deltaTime / FadeTime;
            yield return null;
        }

        if (!playsOnAwake[audioSource])
            audioSource.Stop();
    }
    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime) {
        Debug.Log($"fade IN {audioSource?.clip?.name}");

        if (!startVolumes.ContainsKey(audioSource))
            startVolumes[audioSource] = audioSource.volume;
        if (!playsOnAwake.ContainsKey(audioSource))
            playsOnAwake[audioSource] = audioSource.playOnAwake;

        if (!playsOnAwake[audioSource])
            audioSource.Play();

        audioSource.volume = 0f;
        while (audioSource.volume < startVolumes[audioSource]) {
            audioSource.volume += startVolumes[audioSource] * Time.deltaTime / FadeTime;
            yield return null;
        }
    }
}