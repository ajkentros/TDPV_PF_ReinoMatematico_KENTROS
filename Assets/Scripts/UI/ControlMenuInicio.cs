using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlMenuInicio : MonoBehaviour
{
    public static ControlMenuInicio controlMenuInicio;     // Referencia instancia estática para acceder desde otros scripts

    [Header("Paneles")]
    [SerializeField] private GameObject panelInicio;                // Referencia al panel que tiene la presentación
    [SerializeField] private GameObject panelReino;
    [SerializeField] private GameObject panelFinal;
    [SerializeField] private GameObject panelAyuda;                 // Referencia al planel con la ayuda
    [SerializeField] private GameObject panelConfiguracionAudio;    // Referencia al planel con la ayuda

    [Header("Botones")]
    [SerializeField] private Button[] botonDesafio;         // Referencias a los botones
    [SerializeField] private Button botonIniciarReino;      // Referencia al boton Iniciar Reino

    [Header("Imagenes Botones")]
    [SerializeField] private Image[] imagenBotonDesafio; // Referencias a las imágenes de los botones

    [Header("Texto Botones")]
    [SerializeField] private TextMeshProUGUI[] textoBotonDesafio; // Referencias a los textos de los botones

    [Header("Texto Conocimiento")]
    [SerializeField] private TextMeshProUGUI textoConocimientoSumas;                // Referencias al texto que muestra el conocimiento total de las sumas
    [SerializeField] private TextMeshProUGUI textoConocimientoRestas;               // Referencias al texto que muestra el conocimiento total de las restas
    [SerializeField] private TextMeshProUGUI textoConocimientoMultiplicaciones;     // Referencias al texto que muestra el conocimiento total de las multiplicaciones
    [SerializeField] private TextMeshProUGUI textoConocimientoDivisiones;           // Referencias al texto que muestra el conocimiento total de las divisiones
    [SerializeField] private TextMeshProUGUI textoConocimientoFinal;                // Referencias al texto que muestra el conocimiento final

    [Header("Sprite")]
    [SerializeField] private Sprite spriteSuma;                     // Sprite para el primer botón
    [SerializeField] private Sprite spriteResta;                    // Sprite para el primer botón
    [SerializeField] private Sprite spriteMultiplicacion;           // Sprite para el primer botón
    [SerializeField] private Sprite spriteDivision;                 // Sprite para el primer botón
    [SerializeField] private Sprite spriteCandado;                  // Sprite para los botones bloqueados
    [SerializeField] private Sprite spriteButtonIniciarReino;  // Sprite para el botón en niveles superiores a 0
    [SerializeField] private Sprite nuevoSpriteButtonIniciarReino;  // Sprite para el botón en niveles superiores a 0

    private int[] conocimientoTotalNivel;
    private int nivel;

    private bool repiteNivel = false;
    private bool terminaNivel = false;
    private bool terminaJuego = false;


    private void Start()
    {  
        IniciarStart();
       
    }

    private void IniciarStart()
    {
        int totalNiveles = GameManager.gameManager.GetTotalNiveles();
        conocimientoTotalNivel = new int[totalNiveles];

        // Obtiene banderas del juego
        ActualizaBanderas();
        
        if(!terminaJuego && !repiteNivel && !terminaNivel)
        {
            for (int i = 1; i < botonDesafio.Length; i++)
            {
                // Instancia los botones todos desactivados
                botonDesafio[i].interactable = false;
            }
        }

        if (terminaJuego)
        {
            MostrarPanelFinal();
        }

        CambiarImagenButtonIniciarReino();
        
       }

    private void CambiarImagenButtonIniciarReino()
    {
        // Accede al componente Image del botón
        Image imagenButtonIniciarReino = botonIniciarReino.GetComponent<Image>();

        if (imagenButtonIniciarReino != null)
        {
            if (nivel >= 1 && nuevoSpriteButtonIniciarReino != null)  // Si el nivel es mayor o igual a 1, cambia la imagen
            {
                imagenButtonIniciarReino.sprite = nuevoSpriteButtonIniciarReino;
            }
            else if (spriteButtonIniciarReino != null)  // Si el nivel es 0, restaura la imagen original
            {
                imagenButtonIniciarReino.sprite = spriteButtonIniciarReino;
            }
        }
    }

    // Gestiona el panel Reino
    private void MostrarPanelReino()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(true);
        panelFinal.SetActive(false);
        panelAyuda.SetActive(false);
        panelConfiguracionAudio.SetActive(false);

        // Obtiene banderas del juego
        ActualizaBanderas();

        if (nivel >= 1)
        {
            ActualizarEstadoBotones();
        }

    }

    //
    private void ActualizaBanderas()
    {
        nivel = GameManager.gameManager.GetNivel(); // Debug.Log("ControlMenuInicio - IniciaStart " + "Nivel = " + nivel);
        repiteNivel = GameManager.gameManager.GetRepiteNivel();
        terminaNivel = GameManager.gameManager.GetTerminaNivel();
        terminaJuego = GameManager.gameManager.GetTerminaJuego(); // Debug.Log("ControlMenuInicio - IniciaStart " + "terminaJuego = " + terminaJuego);
    }


    // Gestiona la actualización de los botones en el panel Reino
    private void ActualizarEstadoBotones()
    {

        // Sprites para cada tipo de operación
        Sprite[] spritesOperaciones = { spriteSuma, spriteSuma, spriteSuma, spriteSuma, 
                                        spriteResta, spriteResta, spriteResta, spriteResta,
                                        spriteMultiplicacion, spriteMultiplicacion, spriteMultiplicacion, spriteMultiplicacion,
                                        spriteDivision, spriteDivision, spriteDivision, spriteDivision };

        //Debug.Log("ControlMenuInicio - ActualizarEstadoBotones " + "nivel = " + nivel);
        
        // Actualiza el estado de todos los botones
        for (int i = 0; i < botonDesafio.Length; i++)
        {
            if (i < nivel) // Si el nivel ha sido completado
            {
                // Actualizar el conocimiento y el sprite del botón según el nivel
                conocimientoTotalNivel[i] = GameManager.gameManager.GetconocimientoTotalNivel(i);
                textoBotonDesafio[i].text = conocimientoTotalNivel[i].ToString();
                imagenBotonDesafio[i].sprite = spritesOperaciones[i];
                botonDesafio[i].interactable = false;

            }
            else if (i == nivel) // El siguiente nivel a jugar
            {
                // El siguiente nivel a jugar, desbloquear el sprite correspondiente
                textoBotonDesafio[i].text = "";
                imagenBotonDesafio[i].sprite = spritesOperaciones[i];
                botonDesafio[i].interactable = true;
            }
            else
            {
                // Niveles bloqueados, mantener el sprite de candado
                textoBotonDesafio[i].text = "";
                imagenBotonDesafio[i].sprite = spriteCandado;
                botonDesafio[i].interactable = false;
            }
        }
    }

    // Gestiona el panel final de juego
    private void MostrarPanelFinal()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(false);
        panelFinal.SetActive(true);
        panelAyuda.SetActive(false);
        panelConfiguracionAudio.SetActive(false);

        ActualizarConocimientoFinal();

    }
    
    // Gestiona el inicio de variables conocimiento
    private void ActualizarConocimientoFinal()
    {
        // Obtiene los valores del GameManager y los muestra en la UI
        int conocimientoSumas = GameManager.gameManager.GetConocimientoSumas();
        textoConocimientoSumas.text = conocimientoSumas.ToString();     //Debug.Log("conocimiento de + = " + conocimientoSumas);

        int conocimientoRestas = GameManager.gameManager.GetConocimientoRestas();
        textoConocimientoRestas.text = conocimientoRestas.ToString(); //Debug.Log("conocimiento de - = " + conocimientoRestas);

        int conocimientoMultiplicaciones = GameManager.gameManager.GetConocimientoMultiplicaciones();
        textoConocimientoMultiplicaciones.text = conocimientoMultiplicaciones.ToString(); //Debug.Log("conocimiento de * = " + conocimientoMultiplicaciones);

        int conocimientoDivisiones = GameManager.gameManager.GetConocimientoDivisiones();
        textoConocimientoDivisiones.text = conocimientoDivisiones.ToString(); //Debug.Log("conocimiento de / = " + conocimientoDivisiones);

        int ConocimientoFinal = GameManager.gameManager.GetConocimientoTotal();
        textoConocimientoFinal.text = ConocimientoFinal.ToString(); //Debug.Log("conocimiento Final = " + ConocimientoFinal);
    }

    // Gestiona el botón Jugar
    public void BotonIniciaReino()
    {
        if (!terminaJuego)
        {
            // Habilita paneles
            MostrarPanelReino();
            //IniciarStart();
        }
        else
        {
            MostrarPanelFinal();
        }

        

    }

    public void BotonReIniciaReino()
    {

        // Eliminar el valor del nivel almacenado en PlayerPrefs
        PersistenciaManager.Instance.DeleteAll();
        
        PersistenciaManager.Instance.DeleteConocimientoTotalNivel(nivel);

        GameManager.gameManager.CargarDatosPersistenciaManager();

        GameManager.gameManager.ReiniciaJuego();

        // Actualiza MenuInicio según los nuevos valores
        IniciarStart();
    }

    // Gestiona el cierre de la aplicación
    public void BotonSalirReino()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        GameManager.gameManager.GuardarProgreso();
#else
        Application.Quit();
#endif
    }

    public void BotonVolverReino()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(true);
        panelFinal.SetActive(false);
        panelAyuda.SetActive(false);
        panelConfiguracionAudio.SetActive(false);
    }

    public void BotonVolverMenuInicio()
    {
        panelInicio.SetActive(true);
        panelReino.SetActive(false);
        panelFinal.SetActive(false);
        panelAyuda.SetActive(false);
        panelConfiguracionAudio.SetActive(false);
    }

    // Gestiona el botón Ayuda
    public void BotonAyuda()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(false);
        panelFinal.SetActive(false);
        panelAyuda.SetActive(true);
        panelConfiguracionAudio.SetActive(false);

    }

    // Gestiona el botón Configuración
    public void BotonConfiguracion()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(false);
        panelFinal.SetActive(false);
        panelAyuda.SetActive(false);

        panelConfiguracionAudio.SetActive(true);
    }

    public void BotonDesafio(int escena)
    {
       
        // Reinicia las variables del juego
        GameManager.gameManager.JuegaNivel(escena);

        // Carga la escena 1
        SceneManager.LoadScene(escena);
    }

    public void BotonMenuInicio()
    {
        if (terminaJuego == false)
        {

            // Reinicia las variables del juego
            GameManager.gameManager.VolverMenuInicio();
            
        }
        BotonVolverMenuInicio();
    }

   
}

    
