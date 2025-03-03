using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleMusic : MonoBehaviour
{
    private static ModuleMusic S;
    
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private List<AudioClip> modules = new List<AudioClip>();

    private bool musicPlaying = false;
    private float swapTime;
    private float pauseTime;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        musicPlaying = false;

        ModuleMusic.START_MUSIC();
    }

    private void Update()
    {
        if (musicPlaying)
        {
            if (Time.time > swapTime)
            {
                musicSource.clip = S.modules[Random.Range(0, S.modules.Count)];
                swapTime = Time.time + S.musicSource.clip.length;
                S.musicSource.Play();
            }
        }
    }

    public static void START_MUSIC()
    {
        S.musicPlaying = true;

        S.musicSource.clip = S.modules[Random.Range(0, S.modules.Count)];
        S.swapTime = Time.time + S.musicSource.clip.length;
        S.musicSource.Play();
    }

    public static void PLAY_MUSIC()
    {
        S.swapTime += Time.time - S.pauseTime;
        S.musicPlaying = true;
        S.musicSource.UnPause();
    }

    public static void PAUSE_MUSIC()
    {
        S.musicPlaying = false;
        S.musicSource.Pause();
        S.pauseTime = Time.time;
    }
}
