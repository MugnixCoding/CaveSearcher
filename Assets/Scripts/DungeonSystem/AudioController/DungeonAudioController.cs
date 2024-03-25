using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DungeonAudioController : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    [Range(0,1)]
    private  float volume = 0.2f;
    [SerializeField]
    private AudioClip ambient;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }
    public void PlayAmbient()
    {
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.clip = ambient;
        audioSource.Play();
    }
}
