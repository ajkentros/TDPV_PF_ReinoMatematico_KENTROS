
using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlPanelResumenNivel : MonoBehaviour
{
    [SerializeField] private GameObject panelResumenNivel;
    [SerializeField] private GameObject boton;
    [SerializeField] private TextMeshProUGUI mensajeNivel;
    [SerializeField] private TextMeshProUGUI textConocimientoObtenido;
    [SerializeField] private TextMeshProUGUI conocimientoObtenido;
    [SerializeField] private TextMeshProUGUI textConocimientoMaxNivel;
    [SerializeField] private TextMeshProUGUI conocimientoMaxNivel;


    //private Image panelImage;
    private bool repiteNivel;
    private bool terminaNivel;

    private int nivel;



    // Start is called before the first frame update
    void Start()
    {
        panelResumenNivel.SetActive(false);
        

    }

    private void Update()
    {
        

        ActualizaPanelResumenNivel();

    }

    private void ActualizaPanelResumenNivel()
    {
        repiteNivel = GameManager.gameManager.GetRepiteNivel();
        terminaNivel = GameManager.gameManager.GetTerminaNivel();

        ActualizaPanelResumenNivelRepiteNivel();

        ActualizaPanelResumenNivelTerminaNivel();
    }

    private void ActualizaPanelResumenNivelTerminaNivel()
    {
        if (terminaNivel)
        {
            panelResumenNivel.SetActive(true);
            mensajeNivel.gameObject.SetActive(true);
            panelResumenNivel.GetComponent<Image>().color = Color.green;

            // Cambiar el texto del mensaje

            mensajeNivel.text = "Ganaste el nivel";

            nivel = GameManager.gameManager.GetNivel();
            conocimientoObtenido.text = GameManager.gameManager.GetconocimientoTotalNivel(nivel).ToString();
            conocimientoMaxNivel.text = GameManager.gameManager.GetConocimientoMaximoNivel().ToString();

            boton.SetActive(true);


        }
    }

    private void ActualizaPanelResumenNivelRepiteNivel()
    {
        if (repiteNivel)
        {
            panelResumenNivel.SetActive(true);
            mensajeNivel.gameObject.SetActive(true);
            // Cambiar el texto del mensaje
            mensajeNivel.text = "Tendrás que repetir el nivel";
            textConocimientoObtenido.gameObject.SetActive(false);
            conocimientoObtenido.gameObject.SetActive(false);
            textConocimientoMaxNivel.gameObject.SetActive(false);
            conocimientoMaxNivel.gameObject.SetActive(false);

            boton.SetActive(true);



        }
    }

    public void BotonInicioReino()
    {
        SceneManager.LoadScene(0);
        
    }

}
