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
    [SerializeField] private GameObject panelConfiguracion;         // Referencia al planel con la configuraciòn de audio y música
    [SerializeField] private GameObject panelAyuda;                 // Referencia al planel con la ayuda

    [Header("Botones")]
    [SerializeField] private Button[] botonDesafio; // Referencias a los botones

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

    
    public Sprite spriteSuma; // Sprite para el primer botón
    public Sprite spriteResta; // Sprite para el primer botón
    public Sprite spriteMultiplicacion; // Sprite para el primer botón
    public Sprite spriteDivision; // Sprite para el primer botón
    public Sprite spriteCandado; // Sprite para los botones bloqueados

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

        nivel = GameManager.gameManager.GetNivel();
        repiteNivel = GameManager.gameManager.GetRepiteNivel();
        terminaNivel = GameManager.gameManager.GetTerminaNivel();
        terminaJuego = GameManager.gameManager.GetTerminaJuego();

        int totalNiveles = GameManager.gameManager.GetTotalNiveles();
        conocimientoTotalNivel = new int[totalNiveles];

        for (int i = 1; i < botonDesafio.Length; i++)
        {
            // Instancia los botones todos desactivados
            botonDesafio[i].interactable = false;
        }
    }

    private void Update()
    {
        // Obtiene banderas del juego
        ActualizaBanderas();

        if ((repiteNivel || terminaNivel) && !terminaJuego)
        {
            MostrarPanelReino();
            ActualizarEstadoBotones();
        }

        if (terminaJuego)
        {
            MostrarPanelFinal();
        }

    }

    private void ActualizaBanderas()
    {
        nivel = GameManager.gameManager.GetNivel();
        repiteNivel = GameManager.gameManager.GetRepiteNivel();
        terminaNivel = GameManager.gameManager.GetTerminaNivel();
        terminaJuego = GameManager.gameManager.GetTerminaJuego();
    }


     
    // Gestiona el panel Reino
    private void MostrarPanelReino()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(true);
        panelFinal.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);
    }


    // Gestiona la actualización de los botones en el panel Reino
    private void ActualizarEstadoBotones()
    {

        // Sprites para cada tipo de operación
        Sprite[] spritesOperaciones = { spriteSuma, spriteSuma, spriteSuma, spriteSuma, 
                                        spriteResta, spriteResta, spriteResta, spriteResta,
                                        spriteMultiplicacion, spriteMultiplicacion, spriteMultiplicacion, spriteMultiplicacion,
                                        spriteDivision, spriteDivision, spriteDivision, spriteDivision };

        Debug.Log("ControlMenuInicio - nivel:" + nivel);
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
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);

        ActualizarConocimientoFinal();

    }
    
    // Gestiona el inicio de variables conocimiento
    private void ActualizarConocimientoFinal()
    {
        // Obtiene los valores del GameManager y los muestra en la UI
        int conocimientoSumas = GameManager.gameManager.GetConocimientoSumas();
        textoConocimientoSumas.text = conocimientoSumas.ToString();     Debug.Log("conocimiento de sumas = " + conocimientoSumas);

        int conocimientoRestas = GameManager.gameManager.GetConocimientoRestas();
        textoConocimientoRestas.text = conocimientoRestas.ToString();

        int conocimientoMultiplicaciones = GameManager.gameManager.GetConocimientoMultiplicaciones();
        textoConocimientoMultiplicaciones.text = conocimientoMultiplicaciones.ToString();

        int conocimientoDivisiones = GameManager.gameManager.GetConocimientoDivisiones();
        textoConocimientoDivisiones.text = conocimientoDivisiones.ToString();

        int ConocimientoFinal = GameManager.gameManager.GetConocimientoTotal();
        textoConocimientoFinal.text = ConocimientoFinal.ToString();
    }

    // Gestiona el botón Jugar
    public void BotonIniciaReino()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(true);
        panelFinal.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);

        repiteNivel = false;
        terminaNivel = false;
       
    }

    // Gestiona el cierre de la aplicación
    public void BotonSalirReino()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void BotonVolverReino()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(true);
        panelFinal.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);
    }

    public void BotonVolverMenuInicio()
    {
        panelInicio.SetActive(true);
        panelReino.SetActive(false);
        panelFinal.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);
    }

    // Gestiona el botón Ayuda
    public void BotonAyuda()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(false);
        panelFinal.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(true);

    }


    // Gestiona el botón Configuración
    public void BotonConfiguracion()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(false);
        panelFinal.SetActive(false);
        panelConfiguracion.SetActive(true);
        panelAyuda.SetActive(false);
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
        // Reinicia las variables del juego
        GameManager.gameManager.ReiniciaJuego();

        // Carga la escena 0
        SceneManager.LoadScene(0);
    }

}

    
