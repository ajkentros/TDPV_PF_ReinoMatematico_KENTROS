using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlMenuInicio : MonoBehaviour
{
    public static ControlMenuInicio controlMenuInicio;     // Referencia instancia estática del GameManager para acceder desde otros scripts


    [SerializeField] private GameObject panelInicio;               // Referencia al panel menu
    [SerializeField] private GameObject panelReino;
    [SerializeField] private GameObject panelConfiguracíon;
    [SerializeField] private GameObject panelAyuda;

    private bool panelReinoActivo = false;

    //private void Awake()
    //{
    //    if (controlMenuInicio == null)
    //    {
    //        controlMenuInicio = this;

    //        // Evita que el objeto GameManager se destruya al cargar una nueva escena.
    //        DontDestroyOnLoad(gameObject);
            
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    private void Start()
    {

        panelInicio.SetActive(true);
        panelReino.SetActive(false);
        panelConfiguracíon.SetActive(false);
        panelAyuda.SetActive(false);
    }

    private void Update()
    {
        panelReinoActivo = GameManager.gameManager.GetRepiteNivel();

        if (panelReinoActivo)
        {
            panelInicio.SetActive(false);
            panelReino.SetActive(true);
            panelConfiguracíon.SetActive(false);
            panelAyuda.SetActive(false);
        }
        
    }
    // Gestiona el botón Jugar
    public void BotonIniciaReino()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(true);
        panelConfiguracíon.SetActive(false);
        panelAyuda.SetActive(false);

        panelReinoActivo = false;
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
        panelConfiguracíon.SetActive(false);
        panelAyuda.SetActive(false);
    }

    public void BotonVolverMenuInicio()
    {
        panelInicio.SetActive(true);
        panelReino.SetActive(false);
        panelConfiguracíon.SetActive(false);
        panelAyuda.SetActive(false);
    }

    // Gestiona el botón Ayuda
    public void BotonAyuda()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(false);
        panelConfiguracíon.SetActive(false);
        panelAyuda.SetActive(true);

    }


    // Gestiona el botón Configuración
    public void BotonConfiguracion()
    {
        panelInicio.SetActive(false);
        panelReino.SetActive(false);
        panelConfiguracíon.SetActive(true);
        panelAyuda.SetActive(false);
    }

    public void BotonDesafio_1()
    {
        // Reinicia las variables del juego
        GameManager.gameManager.ReiniciaJuego();

        // Carga la escena 1
        SceneManager.LoadScene(1);
    }

    public void SetPanelReinoActivo(bool _panelReinoActivo)
    {
        panelReinoActivo = _panelReinoActivo;
    }
}
