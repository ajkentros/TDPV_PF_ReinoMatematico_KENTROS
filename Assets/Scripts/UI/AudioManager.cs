using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager; // Instancia singleton

    [Header("M�sica de Fondo")]
    public AudioClip[] audioClipMusicaFondo;
    public AudioSource audioSourceMusicaFondo;

    [Header("Efectos Sonoros")]
    public AudioClip[] audioClipSonidos;
    public AudioSource audioSourceSonidos;

    private void Awake()
    {
        // Implementar Singleton para asegurarse de que solo hay una instancia de AudioManager
        if (audioManager == null)
        {
            audioManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Reproducir m�sica de fondo
    public void PlayMusicaFondo(int index)
    {
       
        if (index >= 0 && index < audioClipMusicaFondo.Length)
        {
            audioSourceMusicaFondo.clip = audioClipMusicaFondo[index];
            audioSourceMusicaFondo.loop = true;
            audioSourceMusicaFondo.Play();
        }
    }

    // Detener la m�sica de fondo
    public void StopMusicaFondo(int index)
    {
        
        if (index >= 0 && index < audioClipMusicaFondo.Length)
        {
            audioSourceMusicaFondo.clip = audioClipMusicaFondo[index];
            audioSourceMusicaFondo.Stop();
        }

    }

    // Reproducir un efecto de sonido por �ndice
    public void PlaySonidos(int index)
    {
        if (index >= 0 && index < audioClipSonidos.Length)
        {
            audioSourceSonidos.clip = audioClipSonidos[index];
            audioSourceSonidos.Play();
        }
    }

    // Detener un efecto de sonido por �ndice
    public void StopSonidos()
    {

        if (audioSourceSonidos.isPlaying)
        {
            audioSourceSonidos.Stop();
        }
    }

    public void StopSonido(int index)
    {

        if (index >= 0 && index < audioClipSonidos.Length)
        {
            audioSourceSonidos.clip = audioClipSonidos[index];
            audioSourceSonidos.Stop();
        }
    }

    public void SetLoopSonidos(int index, bool shouldLoop)
    {
        if (index >= 0 && index < audioClipSonidos.Length)
        {
            audioSourceSonidos.clip = audioClipSonidos[index];
            audioSourceSonidos.loop = shouldLoop;
        }
    }
}
