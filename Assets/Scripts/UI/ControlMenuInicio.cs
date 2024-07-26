using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlMenuInicio : MonoBehaviour
{
    public static ControlMenuInicio controlMenuInicio;     // Referencia instancia est�tica del GameManager para acceder desde otros scripts

    [Header("Paneles")]
    [SerializeField] private GameObject panelInicio;               // Referencia al panel menu
    [SerializeField] private GameObject panelReino;
    [SerializeField] private GameObject panelConfiguracion;
    [SerializeField] private GameObject panelAyuda;

    [Header("Imagenes")]
    [SerializeField] private Image[] imagenBotonDesafio; // Referencias a las im�genes de los botones

    [Header("Texto")]
    [SerializeField] private TextMeshProUGUI[] textoBotonDesafio; // Referencias a los textos de los botones

    public Sprite spriteSuma; // Sprite para el primer bot�n
    public Sprite spriteResta; // Sprite para el primer bot�n
    public Sprite spriteMultiplicacion; // Sprite para el primer bot�n
    public Sprite spriteDivision; // Sprite para el primer bot�n
    public Sprite spriteCandado; // Sprite para los botones bloqueados

    private int[] conocimientoTotalNivel;
    private int nivel;

    private bool repiteNivel = false;
    private bool terminaNivel = false;
    



    private void Start()
    {

        panelInicio.SetActive(true);
        panelReino.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);

        conocimientoTotalNivel = new int[8]; // Aseg�rate de inicializar la matriz con el tama�o adecuado

    }

    private void Update()
    {
        // Obt�n el nivel actual del GameManager
        nivel = GameManager.gameManager.GetNivel();
        //Debug.Log("nivel en menu " + nivel);

        repiteNivel = GameManager.gameManager.GetRepiteNivel();
        terminaNivel = GameManager.gameManager.GetTerminaNivel();

        if (repiteNivel || terminaNivel)
        {
            MostrarPanelReino();
            ActualizarEstadoBotones();
        }

    }

    private void MostrarPanelReino()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(true);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);
    }

    private void ActualizarEstadoBotones()
    {

        // Sprites para cada tipo de operaci�n
        Sprite[] spritesOperaciones = { spriteSuma, spriteSuma, spriteResta, spriteResta, spriteMultiplicacion, spriteMultiplicacion, spriteDivision, spriteDivision };
        Debug.Log("nivel en menu " + nivel);
        // Actualiza el estado de todos los botones
        for (int i = 0; i < imagenBotonDesafio.Length; i++)
        {
            if (i < nivel) // Si el nivel ha sido completado
            {
                // Actualizar el conocimiento y el sprite del bot�n seg�n el nivel
                conocimientoTotalNivel[i] = GameManager.gameManager.GetconocimientoTotalNivel(i);
                textoBotonDesafio[i].text = conocimientoTotalNivel[i].ToString();
                imagenBotonDesafio[i].sprite = spritesOperaciones[i];
            }
            else if (i == nivel) // El siguiente nivel a jugar
            {
                // El siguiente nivel a jugar, desbloquear el sprite correspondiente
                textoBotonDesafio[i].text = "";
                imagenBotonDesafio[i].sprite = spritesOperaciones[i];
            }
            else
            {
                // Niveles bloqueados, mantener el sprite de candado
                textoBotonDesafio[i].text = "";
                imagenBotonDesafio[i].sprite = spriteCandado;
            }
        }
    }


    // Gestiona el bot�n Jugar
    public void BotonIniciaReino()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(true);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);

        repiteNivel = false;
        terminaNivel = false;
    }

    // Gestiona el cierre de la aplicaci�n
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
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);
    }

    public void BotonVolverMenuInicio()
    {
        panelInicio.SetActive(true);
        panelReino.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(false);
    }

    // Gestiona el bot�n Ayuda
    public void BotonAyuda()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelAyuda.SetActive(true);

    }


    // Gestiona el bot�n Configuraci�n
    public void BotonConfiguracion()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(false);
        panelConfiguracion.SetActive(true);
        panelAyuda.SetActive(false);
    }

    public void BotonDesafio_1()
    {
        // Reinicia las variables del juego
        GameManager.gameManager.ReiniciaJuego();

        // Carga la escena 1
        SceneManager.LoadScene(1);
    }
}

    
