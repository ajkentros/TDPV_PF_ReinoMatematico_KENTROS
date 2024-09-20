using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ControladorBoxCalculo : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelCalculo;       // PanelCalculo en el Canvas
    [SerializeField] private GameObject panelOpciones;      // PanelCalculo en el Canvas
    [SerializeField] private TextMeshProUGUI mensaje;       // PanelCalculo en el Canvas
    [SerializeField] private Slider sliderTemporizador;     // PanelCalculo en el Canvas

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
    [SerializeField] private float tiempoParaResolver = 7f;

    private List<int> indicesDisponibles; // Lista de índices disponibles
    private int indiceActual; // Índice actual en la lista barajada
    private readonly int conocimientoCalculoCorrecto = 1;
    private readonly int conocimientoCalculoIncorrecto = -1;
    private Sprite resultadoEsperado; // Para almacenar la imagen de resultado esperada
    private bool hayColisionConBoxCalculo;

    private void OnEnable()
    {
        temporizador.OnTiempoTerminado += ManejaTiempoAgotado; // Suscribirse al evento del temporizador
    }

    private void OnDisable()
    {
        temporizador.OnTiempoTerminado -= ManejaTiempoAgotado; // Desuscribirse del evento del temporizador
    }

    private void Start()
    {
        if (panelCalculo != null) panelCalculo.SetActive(false); // Asegúrate de que el Panel está desactivado al inicio
        if (panelOpciones != null) panelOpciones.SetActive(false);

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerControl.SetMovimientoPlayer(false);
            hayColisionConBoxCalculo = true;

            AudioManager.audioManager.StopMusicaFondo(1);
            AudioManager.audioManager.PlaySonidos(0);

            if (indiceActual < indicesDisponibles.Count)
            {
                temporizador.IniciarTemporizador(tiempoParaResolver);

                LimpiarImagenes();
                DesuscribirEventos();

                SetZonaActivaIfSinNumeroFalse(imageOperando_1);
                SetZonaActivaIfSinNumeroFalse(imageOperando_2);
                SetZonaActivaIfSinNumeroFalse(imageOperando_3);

                int indice = indicesDisponibles[indiceActual];
                indiceActual++;

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

                    SetZonaActivaIfSinNumeroTrue(imageOperando_1);
                    SetZonaActivaIfSinNumeroTrue(imageOperando_2);
                    SetZonaActivaIfSinNumeroTrue(imageOperando_3);

                    if (panelCalculo != null) panelCalculo.SetActive(true);
                    if (panelOpciones != null) panelOpciones.SetActive(true);
                    if (mensaje != null) mensaje.text = "arrastra el número que falta";
                }
            }
            else
            {
                Debug.LogWarning("No hay más datos de cálculo disponibles");
            }

            
        }
    }

    private void ManejaTiempoAgotado()
    {
        if (hayColisionConBoxCalculo)
        {
            mensaje.text = "Tiempo agotado - Perdiste";
            GameManager.gameManager.SetConocimiento(conocimientoCalculoIncorrecto);
            StartCoroutine(DesactivarPanelesYLimpiar());
        }
    }

    private void Shuffle(List<int> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i + 1);
            (lista[rnd], lista[i]) = (lista[i], lista[rnd]);
        }
    }

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

    private void SetZonaActivaIfSinNumeroTrue(Image image)
    {
        if (image != null && image.sprite != null && image.sprite.name == "SinNumero")
        {
            if (image.TryGetComponent<Soltar>(out var soltar))
            {
                soltar.SetZonaActiva(true);
                soltar.OnDropEvent += VerificarResultado;
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
                soltar.OnDropEvent -= VerificarResultado;
            }
        }
    }

    private void VerificarResultado(Sprite imagenSoltada)
    {
        if (imagenSoltada == resultadoEsperado)
        {
            if (mensaje != null) mensaje.text = "Correcto";
            GameManager.gameManager.SetConocimiento(conocimientoCalculoCorrecto);
        }
        else
        {
            if (mensaje != null) mensaje.text = "Perdiste";
            GameManager.gameManager.SetConocimiento(conocimientoCalculoIncorrecto);
        }

        temporizador.DetenerTemporizador();
        DesuscribirEventos();
        StartCoroutine(DesactivarPanelesYLimpiar());
    }

    private void DesuscribirEventos()
    {
        DesuscribirEventoDeImagen(imageOperando_1);
        DesuscribirEventoDeImagen(imageOperando_2);
        DesuscribirEventoDeImagen(imageOperando_3);
        DesuscribirEventoDeImagen(imageOpcion_1);
        DesuscribirEventoDeImagen(imageOpcion_2);
        DesuscribirEventoDeImagen(imageOpcion_3);
    }

    private void DesuscribirEventoDeImagen(Image image)
    {
        if (image != null && image.TryGetComponent<Soltar>(out var soltar))
        {
            soltar.OnDropEvent -= VerificarResultado;
        }
    }

    private IEnumerator DesactivarPanelesYLimpiar()
    {
        yield return new WaitForSeconds(2f);

        if (panelCalculo != null) panelCalculo.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (mensaje != null) mensaje.text = "";

        hayColisionConBoxCalculo = false;
        LimpiarImagenes();
        Destroy(gameObject); // Destruir el objeto BoxCalculo
        playerControl.SetMovimientoPlayer(true);

    }

    private void LimpiarImagenes()
    {
        imageOperando_1.sprite = null;
        imageOperando_2.sprite = null;
        imageOperando_3.sprite = null;
        imageOpcion_1.sprite = null;
        imageOpcion_2.sprite = null;
        imageOpcion_3.sprite = null;
    }
}

