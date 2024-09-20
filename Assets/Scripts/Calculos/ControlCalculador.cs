
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ControladorCalculos : MonoBehaviour
{
   

    [Header("Paneles")]
    [SerializeField] private GameObject panelCalculo;       // PanelCalculo en el Canvas
    [SerializeField] private GameObject panelOpciones;      // PanelCalculo en el Canvas
    [SerializeField] private TextMeshProUGUI mensaje;       // PanelCalculo en el Canvas
    [SerializeField] private Slider sliderTemporizador;     // PanelCalculo en el Canvas
    
    // Referencias a los componentes de imagen de los paneles
    [Header("Operandos")]
    [SerializeField] private Image imageOperando_1;
    [SerializeField] private Image imageOperando_2;
    [SerializeField] private Image imageOperando_3;

    [Header("Opciones")]
    [SerializeField] private Image imageOpcion_1;
    [SerializeField] private Image imageOpcion_2;
    [SerializeField] private Image imageOpcion_3;

    [Header("CalculoData")]
    [SerializeField] private CalculoData[] calculoDatas;  // Array de ScriptableObjects

    [Header("Player")]
    [SerializeField] private PlayerControl playerControl; // Referencia al script del Player

    [Header("Tablero de tiempo")]
    [SerializeField] private Reloj temporizador;
    [SerializeField] private int tiempoPanelCalculo;
    [SerializeField] private float tiempoParaResolver = 10f;


    private List<int> indicesDisponibles; // Lista de índices disponibles
    private int indiceActual; // Índice actual en la lista barajada
    private readonly int conocimientoCalculoCorrecto = 1;
    private readonly int conocimientoCalculoIncorrecto = -1;
    private Sprite resultadoEsperado; // Para almacenar la imagen de resultado esperada
    private bool hayColisionConBoxCalculo;



    private void OnEnable()
    {
        // Suscribirse al evento cuando el objeto está habilitado
        BoxCalculo.OnPlayerCollide += HayColisionConBoxCalculo;
        temporizador.OnTiempoTerminado += ManejaTiempoAgotado; // Desuscribirse del evento del temporizador

        //Debug.Log("Suscrito al evento OnPlayerCollide");
    }

    private void OnDisable()
    {
        // Desuscribirse del evento cuando el objeto está deshabilitado
        BoxCalculo.OnPlayerCollide -= HayColisionConBoxCalculo;
        temporizador.OnTiempoTerminado -= ManejaTiempoAgotado; // Desuscribirse del evento del temporizador
        //Debug.Log("Desuscrito del evento OnPlayerCollide");
    }

    private void Start()
    {
        if (panelCalculo != null) panelCalculo.SetActive(false); // Asegúrate de que el Panel está desactivado al inicio
        if (panelOpciones != null) panelOpciones.SetActive(false);
        //if (mensaje != null) mensaje.text = "";


        //Inicializar la lista de índices disponibles y barajarla
        indicesDisponibles = new List<int>();
        for (int i = 0; i < calculoDatas.Length; i++)
        {
            indicesDisponibles.Add(i);
        }

        Shuffle(indicesDisponibles);
        indiceActual = 0;
        hayColisionConBoxCalculo = false;
    }

    private void Update()
    {
        if (temporizador != null && sliderTemporizador != null)
        {
            
            float tiempoRestante = temporizador.ObtenerTiempoRestante();
            sliderTemporizador.value = tiempoRestante / tiempoParaResolver; // Actualiza el valor del slider

            // Cambia el color del temporizador
            Color sliderTemporizadorColor;


            if (sliderTemporizador.value >= 0.7f)
            {
                sliderTemporizadorColor = Color.green;
            }
            else if (sliderTemporizador.value >= 0.3f)
            {
                sliderTemporizadorColor = Color.yellow;
            }
            else
            {
                sliderTemporizadorColor = Color.red;
            }

            // Cambia el color del fill del slider
            sliderTemporizador.fillRect.GetComponent<Image>().color = sliderTemporizadorColor;
        }
    }

    private void HayColisionConBoxCalculo()
    {
        // Desactiva el movimiento del player
        playerControl.SetMovimientoPlayer(false);      //Debug.Log("detiene al player");

        AudioManager.audioManager.StopMusicaFondo(1);
        AudioManager.audioManager.PlaySonidos(0);
        

        hayColisionConBoxCalculo = true;

        if (indiceActual < indicesDisponibles.Count)
        {
            temporizador.IniciarTemporizador(tiempoParaResolver);

            
            // Limpiar imágenes antes de asignar los nuevos datos
            LimpiarImagenes();
            DesuscribirEventos();

            SetZonaActivaIfSinNumeroFalse(imageOperando_1);
            SetZonaActivaIfSinNumeroFalse(imageOperando_2);
            SetZonaActivaIfSinNumeroFalse(imageOperando_3);

            // Obtener el índice actual no repetido
            int indice = indicesDisponibles[indiceActual];
            indiceActual++;

            // Asignar los datos del ScriptableObject a los componentes del panel
            CalculoData data = calculoDatas[indice];
            if (data != null)
            {
                if (imageOperando_1 != null)
                {
                    imageOperando_1.sprite = data.Operando_1;
                    AsignarColorDeFondo(imageOperando_1);
                }
                if (imageOperando_2 != null)
                {
                    imageOperando_2.sprite = data.Operando_2;
                    AsignarColorDeFondo(imageOperando_2);
                }
                if (imageOperando_3 != null)
                {
                    imageOperando_3.sprite = data.Operando_3;
                    AsignarColorDeFondo(imageOperando_3);
                }
                if (imageOpcion_1 != null) imageOpcion_1.sprite = data.Opcion_1;
                if (imageOpcion_2 != null) imageOpcion_2.sprite = data.Opcion_2;
                if (imageOpcion_3 != null) imageOpcion_3.sprite = data.Opcion_3;

                resultadoEsperado = data.Resultado; // Almacenar el resultado esperado

                // Activar zonaActiva si el sprite es SinNumero
                SetZonaActivaIfSinNumeroTrue(imageOperando_1);
                SetZonaActivaIfSinNumeroTrue(imageOperando_2);
                SetZonaActivaIfSinNumeroTrue(imageOperando_3);

                // Activar los paneles
                if (panelCalculo != null) panelCalculo.SetActive(true);
                if (panelOpciones != null) panelOpciones.SetActive(true);
                if (mensaje != null) mensaje.text = "arrastra el número que falta";
                //mensaje.gameObject.SetActive(true);

                
            }
        }
        else
        {
            Debug.LogWarning("No hay más datos de cálculo disponibles");
        }
    }

    // Método para manejar el tiempo agotado
    private void ManejaTiempoAgotado()
    {
        if(hayColisionConBoxCalculo)
        {
            mensaje.text = "Tiempo agotado - Perdiste";

            // Reduce el conocimiento en 1
            GameManager.gameManager.SetConocimiento(conocimientoCalculoIncorrecto);

            StartCoroutine(DesactivarPanelesYLimpiar());
        }
        
    }

    // Método para barajar una lista
    private void Shuffle(List<int> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            (lista[rnd], lista[i]) = (lista[i], lista[rnd]);
        }
    }

    // Método para asignar el color de fondo
    private void AsignarColorDeFondo(Image image)
    {
        Color colorSinNumero = new Color32(250, 137, 8, 255); // Color FA8908
        Color colorConNumero = new Color32(237, 255, 100, 255); // Color EDFF64

        if (image.sprite != null && image.sprite.name == "SinNumero")
        {
            image.color = colorSinNumero;
        }
        else
        {
            image.color = colorConNumero;
        }
    }

    // Método para activar zonaActiva si el sprite es SinNumero
    private void SetZonaActivaIfSinNumeroTrue(Image image)
    {
        if (image != null && image.sprite != null && image.sprite.name == "SinNumero")
        {
            if (image.TryGetComponent<Soltar>(out var soltar))
            {
                soltar.SetZonaActiva(true);
                //Debug.Log($"Activada zonaActiva en: {image.name}");
                soltar.OnDropEvent += VerificarResultado; // Suscribirse al evento OnDrop
            }
        }
    }

    private void SetZonaActivaIfSinNumeroFalse(Image image)
    {
        if (image != null && image.sprite != null)
        {
            if (image.TryGetComponent<Soltar>(out var soltar))
            {
                soltar.SetZonaActiva(false);
                //Debug.Log($"Desactiva zonaActiva en: {image.name}");
                soltar.OnDropEvent -= VerificarResultado; // Suscribirse al evento OnDrop
            }
        }
    }

    // Método para verificar el resultado
    private void VerificarResultado(Sprite imagenSoltada)
    {
        //Debug.Log($"imagen soltada: {imagenSoltada.name}");
        if (imagenSoltada == resultadoEsperado)
        {
            if (mensaje != null) mensaje.text = "Correcto";

            // Aumenta el conocimiento en 1
            GameManager.gameManager.SetConocimiento(conocimientoCalculoCorrecto);

        }
        else
        {
            if (mensaje != null) mensaje.text = "Perdiste";

            // Reduce el conocimiento en 1
            GameManager.gameManager.SetConocimiento(conocimientoCalculoIncorrecto);
        }
        temporizador.DetenerTemporizador();
        // Desuscribirse del evento OnDrop para evitar múltiples suscripciones
        DesuscribirEventos();

        // Desactivar los paneles y limpiar las imágenes después de 5 segundos
        StartCoroutine(DesactivarPanelesYLimpiar());
        
    }

    private void DesuscribirEventos()
    {
        // Desuscribir del evento OnDrop
        DesuscribirEventoDeImagen(imageOperando_1);
        DesuscribirEventoDeImagen(imageOperando_2);
        DesuscribirEventoDeImagen(imageOperando_3);
        DesuscribirEventoDeImagen(imageOpcion_1);
        DesuscribirEventoDeImagen(imageOpcion_2);
        DesuscribirEventoDeImagen(imageOpcion_3);

        //Debug.Log("Eventos desuscritos");
    }

    private void DesuscribirEventoDeImagen(Image image)
    {
        if (image != null && image.TryGetComponent<Soltar>(out var soltar))
        {
            soltar.OnDropEvent -= VerificarResultado;
            //Debug.Log($"Evento OnDropEvent desuscrito de {image.name}");
        }
    }

    // Corrutina para desactivar paneles y limpiar imágenes
    private IEnumerator DesactivarPanelesYLimpiar()
    {
        yield return new WaitForSeconds(tiempoPanelCalculo);

        if (panelCalculo != null) panelCalculo.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        mensaje.text = "";

        LimpiarImagenes();
        ResetOpcionesPosition();

        hayColisionConBoxCalculo = false;
        
        //
        AudioManager.audioManager.StopSonido(0);
        AudioManager.audioManager.PlayMusicaFondo(1);

        // Reactivar el movimiento del player
        playerControl.SetMovimientoPlayer(true);
    }

    private void LimpiarImagenes()
    {
        if (imageOperando_1 != null)
        {
            imageOperando_1.sprite = null;
            imageOperando_1.color = Color.white; // Restablecer el color predeterminado

        }
        if (imageOperando_2 != null)
        {
            imageOperando_2.sprite = null;
            imageOperando_2.color = Color.white; // Restablecer el color predeterminado

        }
        if (imageOperando_3 != null)
        {
            imageOperando_3.sprite = null;
            imageOperando_3.color = Color.white; // Restablecer el color predeterminado

        }
        if (imageOpcion_1 != null) imageOpcion_1.sprite = null;
        if (imageOpcion_2 != null) imageOpcion_2.sprite = null;
        if (imageOpcion_3 != null) imageOpcion_3.sprite = null;
        if (resultadoEsperado != null) resultadoEsperado = null;
    }

    private void ResetOpcionesPosition()
    {
        // Resetea las posiciones de las opciones
        imageOpcion_1.GetComponent<Arrastrar>().ResetPosition();
        imageOpcion_2.GetComponent<Arrastrar>().ResetPosition();
        imageOpcion_3.GetComponent<Arrastrar>().ResetPosition();
    }
}
