using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundCue : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundEffects;
    private AudioSource audioSource;
    private int length;

    private void Awake()
    {
        length = soundEffects.Length;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot()
    {
        audioSource.PlayOneShot(soundEffects[Random.Range(0, length)]);
    }

    public void Play()
    {
        audioSource.clip = soundEffects[Random.Range(0, length)];
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
