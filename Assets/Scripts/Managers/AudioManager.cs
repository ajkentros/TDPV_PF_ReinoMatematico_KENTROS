
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
    private Button buttonMutePlayMusica;      // Referencia al boton Mute/Play

    public Sprite botonMuteSonido;            // Asigna la imagen para el estado Mute
    public Sprite botonPlaySonido;            // Asigna la imagen para el estado Play
    private Button buttonMutePlaySonido;       // Referencia al boton Mute/Play

    [Header("Controles de Volumen")]
    private Slider sliderMusica; // Referencia al slider de música
    private Slider sliderSonido; // Referencia al slider de sonidos


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
            return;
        }

    }
    //
    private void Start()
    {
        // Carga la configuración de sonido después de asignar referencias UI
        CargaConfiguracionSonido();
    }

    // Método que se invoca desde el panel de configuración cuando este se activa.
    public void AsignaReferenciasPanelConfiguracionAudio()
    {
        /// Encuentra el panel dentro del Canvas (ajusta el nombre si es necesario)
        GameObject panelConfiguracionAudio = GameObject.Find("MenuInicio").transform.Find("PanelConfiguracionAudio").gameObject;

        if (panelConfiguracionAudio != null)
        {
            // Asigna los botones y sliders solo si el panel está activo
            buttonMutePlayMusica = panelConfiguracionAudio.transform.Find("ButtonMusica_Mute").GetComponent<Button>();
            buttonMutePlaySonido = panelConfiguracionAudio.transform.Find("ButtonSonido_Mute").GetComponent<Button>();

            sliderMusica = panelConfiguracionAudio.transform.Find("SliderMusica").GetComponent<Slider>();
            sliderSonido = panelConfiguracionAudio.transform.Find("SliderSonido").GetComponent<Slider>();

            // Verificar que los botones y sliders existen antes de asignar eventos
            if (buttonMutePlayMusica != null && buttonMutePlaySonido != null && sliderMusica != null && sliderSonido != null)
            {
                // Eliminar los listeners previos para evitar duplicados
                buttonMutePlayMusica.onClick.RemoveAllListeners();
                buttonMutePlaySonido.onClick.RemoveAllListeners();
                sliderMusica.onValueChanged.RemoveAllListeners();
                sliderSonido.onValueChanged.RemoveAllListeners();

                // Asignar los métodos a los eventos de los botones y sliders
                buttonMutePlayMusica.onClick.AddListener(MuteMusica);
                buttonMutePlaySonido.onClick.AddListener(MuteSonidos);

                sliderMusica.onValueChanged.AddListener(delegate { SetVolumenMusica(); });
                sliderSonido.onValueChanged.AddListener(delegate { SetVolumenSonidos(); });

                Debug.Log("Eventos de los botones y sliders asignados correctamente.");
            }
        }
        else
        {
            Debug.LogError("No se pudo encontrar el PanelConfiguracionAudio o los objetos UI dentro de él.");
        }
    }

    public void CargaConfiguracionSonido()
    {
        // Verificar si los controles existen antes de actualizarlos
        if (sliderMusica != null && buttonMutePlayMusica != null)
        {
            bool isMusicaMuted = PersistenciaManager.Instance.LoadMuteMusicaConfig();
            float volumenMusica = PersistenciaManager.Instance.LoadVolumenMusicaConfig();

            audioSourceMusicaFondo.mute = isMusicaMuted;
            audioSourceMusicaFondo.volume = volumenMusica;

            sliderMusica.value = volumenMusica;
            buttonMutePlayMusica.image.sprite = isMusicaMuted ? botonMuteMusica : botonPlayMusica;
        }

        if (sliderSonido != null && buttonMutePlaySonido != null)
        {
            bool isSonidoMuted = PersistenciaManager.Instance.LoadMuteSonidoConfig();
            float volumenSonido = PersistenciaManager.Instance.LoadVolumenSonidoConfig();

            audioSourceSonidos.mute = isSonidoMuted;
            audioSourceSonidos.volume = volumenSonido;

            sliderSonido.value = volumenSonido;
            buttonMutePlaySonido.image.sprite = isSonidoMuted ? botonMuteSonido : botonPlaySonido;
        }
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

        // Cambia el sprite del botón si se mutea la música
        buttonMutePlayMusica.image.sprite = volumen == 0f ? botonMuteMusica : botonPlayMusica;

        // Guardar configuración de música en PersistenciaManager
        PersistenciaManager.Instance.SaveMusicaConfig(audioSourceMusicaFondo.mute, volumen);
        PersistenciaManager.Instance.Save();
    }

    // Gestiona ajuste de volumen de sonidos
    public void SetVolumenSonidos()
    {
        // Asigna a volumne el valor que toma el slider
        float volumen = sliderSonido.value;

        // Asigna al volumen del audio source con el valor de volumen
        audioSourceSonidos.volume = volumen;

        // Cambia el sprite del botón si se mutea el sonido
        buttonMutePlaySonido.image.sprite = volumen == 0f ? botonMuteSonido : botonPlaySonido;

        // Guardar configuración de sonido
        PersistenciaManager.Instance.SaveSonidoConfig(audioSourceSonidos.mute, volumen);
        PersistenciaManager.Instance.Save();
    }

    // Gestiona muteo de musica
    public void MuteMusica()
    {
        // Camcia el estado de muteo
        audioSourceMusicaFondo.mute = !audioSourceMusicaFondo.mute;

        // Cambia la imagen del botón según el estado de muteo
        buttonMutePlayMusica.image.sprite = audioSourceMusicaFondo.mute ? botonMuteMusica : botonPlayMusica;

        // Guardar configuración de música
        PersistenciaManager.Instance.SaveMusicaConfig(audioSourceMusicaFondo.mute, audioSourceMusicaFondo.volume);
        PersistenciaManager.Instance.Save();
    }

    // Gestiona muteo de sonidos
    public void MuteSonidos()
    {
        // Camcia el estado de muteo
        audioSourceSonidos.mute = !audioSourceSonidos.mute;

        // Cambia la imagen del botón según el estado de muteo
        buttonMutePlaySonido.image.sprite = audioSourceSonidos.mute ? botonMuteSonido : botonPlaySonido;

        // Guardar configuración de sonido
        PersistenciaManager.Instance.SaveSonidoConfig(audioSourceSonidos.mute, audioSourceSonidos.volume);
        PersistenciaManager.Instance.Save();
    }
}
