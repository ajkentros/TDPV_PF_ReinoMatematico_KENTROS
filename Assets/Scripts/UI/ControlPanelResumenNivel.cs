
using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlPanelResumenNivel : MonoBehaviour
{
    [SerializeField] private GameObject panelResumenNivel;
    [SerializeField] private GameObject botonContinua;
    [SerializeField] private TextMeshProUGUI mensajeNivel;
    [SerializeField] private TextMeshProUGUI textConocimientoObtenido;
    [SerializeField] private TextMeshProUGUI conocimientoObtenido;
    [SerializeField] private TextMeshProUGUI textConocimientoMaxNivel;
    [SerializeField] private TextMeshProUGUI conocimientoMaxNivel;


    private int nivel;

    private void OnEnable()
    {
        // Suscribe los métodos a los eventos
        GameManager.gameManager.OnNivelTerminado += ActualizaPanelResumenNivelTerminaNivel;
        GameManager.gameManager.OnNivelRepetido += ActualizaPanelResumenNivelRepiteNivel;
    }

    private void OnDisable()
    {
        GameManager.gameManager.OnNivelTerminado -= ActualizaPanelResumenNivelTerminaNivel;
        GameManager.gameManager.OnNivelRepetido -= ActualizaPanelResumenNivelRepiteNivel;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Desactiva PanelResumen
        panelResumenNivel.SetActive(false);

        

       

    }

    // Gestiona la actualización del PanelResumen cuando el nivel se termina
    private void ActualizaPanelResumenNivelTerminaNivel()
    {
        // Actualiza paneles
        panelResumenNivel.SetActive(true);
        mensajeNivel.gameObject.SetActive(true);
        panelResumenNivel.GetComponent<Image>().color = Color.green;

        // Cambiar el texto del mensaje
        mensajeNivel.text = "Ganaste el nivel";

        // Asigna el valor del nivel jugado
        nivel = GameManager.gameManager.GetNivel();

        // Usa el nivel como índice, restando 1 porque los arrays empiezan en 0, pero asegurando que no es negativo
        int indice = Mathf.Max(nivel - 1, 0);

        // Actualiza textos en pantalla
        conocimientoObtenido.text = GameManager.gameManager.GetconocimientoTotalNivel(indice).ToString();
        conocimientoMaxNivel.text = GameManager.gameManager.GetConocimientoMaximoNivel().ToString();


        // Habilita botón
        botonContinua.SetActive(true);
    }

    // Gestiona la actualización del PanelResumen cuando el nivel se repite
    private void ActualizaPanelResumenNivelRepiteNivel()
    {
        // Actualiza paneles
        panelResumenNivel.SetActive(true);
        mensajeNivel.gameObject.SetActive(true);


        // Cambiar el texto del mensaje
        mensajeNivel.text = "Tendrás que repetir el nivel";

        // Actualiza campos de texto
        textConocimientoObtenido.gameObject.SetActive(false);
        conocimientoObtenido.gameObject.SetActive(false);
        textConocimientoMaxNivel.gameObject.SetActive(false);
        conocimientoMaxNivel.gameObject.SetActive(false);



        // Habilita botón
        botonContinua.SetActive(true);

    }

    // Gestiona boton BotonContinua
    public void BotonContinua()
    {
        
        
        SceneManager.LoadScene(0);

    }

}
