
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager; // Instancia singleton

    [Header("Música de Fondo")]
    public AudioClip[] audioClipMusicaFondo;        // Referencia a los clips de sonidos
    public AudioSource audioSourceMusicaFondo;      // Referencia a la fuente de sonido

    [Header("Efectos Sonoros")]
    public AudioClip[] audioClipSonidos;            // Referencia a los clips de musica 
    public AudioSource audioSourceSonidos;          // Referencia a la fuente de sonido

    [Header("Botón de Mute/Play")]
    public Sprite botonMuteMusica;            // Asigna la imagen para el estado Mute
    public Sprite botonPlayMusica;            // Asigna la imagen para el estado Play
    public Button buttonMutePlayMusica;      // Referencia al boton Mute/Play

    public Sprite botonMuteSonido;            // Asigna la imagen para el estado Mute
    public Sprite botonPlaySonido;            // Asigna la imagen para el estado Play
    public Button buttonMutePlaySonido;       // Referencia al boton Mute/Play

    [Header("Controles de Volumen")]
    public Slider sliderMusica; // Referencia al slider de música
    public Slider sliderSonidos; // Referencia al slider de sonidos


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
    //
    private void Start()
    {
        // Inicializa el volumen al máximo (1.0f)
        audioSourceMusicaFondo.volume = 0.25f;
        audioSourceSonidos.volume = 0.25f;

        sliderMusica.value = audioSourceMusicaFondo.volume;
        sliderSonidos.value = audioSourceSonidos.volume;


    }

    // Reproduce música de fondo
    public void PlayMusicaFondo(int index)
    {

        if (index >= 0 && index < audioClipMusicaFondo.Length)
        {
            audioSourceMusicaFondo.clip = audioClipMusicaFondo[index];
            audioSourceMusicaFondo.loop = true;
            audioSourceMusicaFondo.Play();
        }
    }

    // Detiene la música de fondo
    public void StopMusicaFondo(int index)
    {

        if (index >= 0 && index < audioClipMusicaFondo.Length)
        {
            audioSourceMusicaFondo.clip = audioClipMusicaFondo[index];
            audioSourceMusicaFondo.Stop();
        }

    }

    // Reproduce un efecto de sonido por índice
    public void PlaySonidos(int index)
    {
        if (index >= 0 && index < audioClipSonidos.Length)
        {
            audioSourceSonidos.clip = audioClipSonidos[index];
            audioSourceSonidos.Play();
        }
    }

    // Detiene un efecto de sonido por índice
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

    // Gestiona ajuste de volumen de musica
    public void SetVolumenMusica()
    {
        // Asigna a volumne el valor que toma el slider
        float volumen = sliderMusica.value;

        // Asigna al volumen del audio source con el valor de volumen
        audioSourceMusicaFondo.volume = volumen;

        if (volumen == 0f)
        {
            buttonMutePlayMusica.image.sprite = botonMuteMusica;
        }
        else
        {
            buttonMutePlayMusica.image.sprite = botonPlayMusica;
        }
    }

    // Gestiona ajuste de volumen de sonidos
    public void SetVolumenSonidos()
    {
        // Asigna a volumne el valor que toma el slider
        float volumen = sliderSonidos.value;

        // Asigna al volumen del audio source con el valor de volumen
        audioSourceSonidos.volume = volumen;

        if (volumen == 0f)
        {
            buttonMutePlaySonido.image.sprite = botonMuteSonido;
        }
        else
        {
            buttonMutePlaySonido.image.sprite = botonPlaySonido;
        }
    }

    // Gestiona muteo de musica
    public void MuteMusica()
    {
        // Camcia el estado de muteo
        audioSourceMusicaFondo.mute = !audioSourceMusicaFondo.mute;

        // Cambia la imagen del botón según el estado de muteo
        if (audioSourceMusicaFondo.mute)
        {
            buttonMutePlayMusica.image.sprite = botonMuteMusica;
        }
        else
        {
            buttonMutePlayMusica.image.sprite = botonPlayMusica;
        }

    }

    

    // Gestiona muteo de sonidos
    public void MuteSonidos()
    {
        // Camcia el estado de muteo
        audioSourceSonidos.mute = !audioSourceSonidos.mute;

        // Cambia la imagen del botón según el estado de muteo
        if (audioSourceSonidos.mute)
        {
            buttonMutePlaySonido.image.sprite = botonMuteSonido;
        }
        else
        {
            buttonMutePlaySonido.image.sprite = botonPlaySonido;
        }
    }
}
