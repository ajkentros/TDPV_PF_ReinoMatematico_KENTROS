using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ControlCrucigrama : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelCrucigrama;   // PanelCalculo en el Canvas
    [SerializeField] private TextMeshProUGUI mensaje;
    [SerializeField] private Slider sliderCrucigrama;   // PanelCalculo en el Canvas

    [Header("CrucigramaData")]
    [SerializeField] private CrucigramaData[] crucigramaData;  // Array de ScriptableObjects

    [Header("Operandos")]
    [SerializeField] private Image[] imagenOperando; // Array de imágenes de los operandos

    [Header("Input Field Prefab")]
    [SerializeField] private TMP_InputField inputFieldPrefab; // Prefab del InputField

    [Header("Player")]
    [SerializeField] private PlayerControl playerControl; // Referencia al script del Player

    [Header("Tablero de tiempo")]
    [SerializeField] private Reloj temporizador;
    [SerializeField] private int tiempoPanelCrucigrama = 2;
    [SerializeField] private float tiempoParaResolver = 15f;


    private CrucigramaData crucigramaActual; // Referencia al crucigrama actual
    private TMP_InputField currentInputField;
    private List<int> indicesDisponibles; // Lista de índices disponibles
    private int indiceActual; // Índice actual en la lista barajada
    private int intentosRestantes = 3; // Contador de intentos
    private int conocimientoCalculoCorrecto = 1;
    private readonly int conocimientoCalculoIncorrecto = -1;
    private bool allInputFieldsFilled;  // Bandera para verificar si todos los campos de entrada están completos
    private bool shouldValidate;        // Bandera para controlar la validación
    private bool hayColisionConBoxCrucigrama;





    private void OnEnable()
    {
        // Suscribirse al evento cuando el objeto está habilitado
        BoxCrucigrama.OnPlayerCollideBoxCrucigrama += HayColisionConBoxCrucigrama;
        temporizador.OnTiempoTerminado += ManejaTiempoAgotado; // Desuscribirse del evento del temporizador
        //Debug.Log("Suscrito a eventos ");
    }

    private void OnDisable()
    {
        // Desuscribirse del evento cuando el objeto está deshabilitado
        BoxCrucigrama.OnPlayerCollideBoxCrucigrama -= HayColisionConBoxCrucigrama;
        temporizador.OnTiempoTerminado -= ManejaTiempoAgotado; // Desuscribirse del evento del temporizador
        //Debug.Log("DeSuscrito a eventos");
    }


    private void Start()
    {
        if (panelCrucigrama != null) panelCrucigrama.SetActive(false); // Asegúrate de que el Panel está activado al inicio
        if (mensaje != null) mensaje.gameObject.SetActive(false);

        mensaje.text = ""; // Borrar el mensaje al iniciar

        
        // Inicializar la lista de índices disponibles y barajarla
        indicesDisponibles = new List<int>();

        for (int i = 0; i < crucigramaData.Length; i++)
        {
            indicesDisponibles.Add(i);
        }

        Shuffle(indicesDisponibles);
        indiceActual = 0;
        hayColisionConBoxCrucigrama = false;

    }

    private void Update()
    {
        if (shouldValidate == false && currentInputField != null && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            HandleKeyboardInput(currentInputField);
        }

        ControlSliderTemporizador();
    }

    private void ControlSliderTemporizador()
    {
        if (temporizador != null && sliderCrucigrama != null)
        {
            float tiempoRestante = temporizador.ObtenerTiempoRestante();
            sliderCrucigrama.value = tiempoRestante / tiempoParaResolver; // Actualiza el valor del slider

            // Cambia el color del temporizador
            Color sliderTemporizadorColor;


            if (sliderCrucigrama.value >= 0.7f)
            {
                sliderTemporizadorColor = Color.green;
            }
            else if (sliderCrucigrama.value >= 0.3f)
            {
                sliderTemporizadorColor = Color.yellow;
            }
            else
            {
                sliderTemporizadorColor = Color.red;
            }

            // Cambia el color del fill del slider
            sliderCrucigrama.fillRect.GetComponent<Image>().color = sliderTemporizadorColor;
        }
    }


    // Método para barajar una lista
    private void Shuffle(List<int> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i + 1);
            (lista[rnd], lista[i]) = (lista[i], lista[rnd]);
        }
    }


    // Método para activar los paneles
    private void HayColisionConBoxCrucigrama()
    {
        hayColisionConBoxCrucigrama = true;

        if (indiceActual < indicesDisponibles.Count)
        {
            temporizador.IniciarTemporizador(tiempoParaResolver);

            // Obtener el índice actual no repetido
            int indice = indicesDisponibles[indiceActual];
            indiceActual = (indiceActual + 1) % indicesDisponibles.Count;

            // Asignar los datos del ScriptableObject a los componentes del panel
            crucigramaActual = crucigramaData[indice];

            if (crucigramaActual != null)
            {
                // Limpiar InputFields anteriores
                LimpiarInputFields();

                // Cargar imágenes de los operandos desde CrucigramaData
                Sprite[] operandos = {
                    crucigramaActual.Operando_11, crucigramaActual.Operando_12, crucigramaActual.Operando_13,
                    crucigramaActual.Operando_21, crucigramaActual.Operando_22, crucigramaActual.Operando_23,
                    crucigramaActual.Operando_31, crucigramaActual.Operando_32, crucigramaActual.Operando_33
                };

                for (int i = 0; i < imagenOperando.Length; i++)
                {
                    imagenOperando[i].sprite = operandos[i];
                    SetPlaceholderIfNoNumber(imagenOperando[i]);
                }

                

                // Activar los paneles
                if (panelCrucigrama != null) panelCrucigrama.SetActive(true);
                if (mensaje != null) mensaje.text = "Completa el crucigrama";
                mensaje.gameObject.SetActive(true);

                // Desactiva el movimiento del player
                playerControl.SetMovimientoPlayer(false);

                // Reiniciar intentos restantes
                intentosRestantes = 3;

                // Restablecer la bandera de campos completos
                allInputFieldsFilled = false;
                shouldValidate = true; // Permitir validación

            }

        }
        else
        {
            Debug.LogWarning("No hay más crucigramas disponibles");
        }
    }

    // Método para manejar el tiempo agotado
    private void ManejaTiempoAgotado()
    {
        if (hayColisionConBoxCrucigrama)
        {
            mensaje.text = "Tiempo agotado - Perdiste";

            // Decrementa el conocimiento en 1
            GameManager.gameManager.SetConocimiento(conocimientoCalculoIncorrecto);

            StartCoroutine(DesactivarPanelesYLimpiar());
        }
    }

    private void LimpiarInputFields()
    {
        // Encontrar todos los InputFields dentro del panel de crucigrama y eliminarlos
        TMP_InputField[] inputFields = panelCrucigrama.GetComponentsInChildren<TMP_InputField>();
        foreach (TMP_InputField inputField in inputFields)
        {
            if (inputField != null)
            {
                // Detener la corrutina de parpadeo si existe
                if (inputField.TryGetComponent<ParpadeoPlaceholder>(out var parpadeoScript))
                {
                    parpadeoScript.DetenerParpadeo();
                }

                Destroy(inputField.gameObject);
            }
        }
    }

    
    private void SetPlaceholderIfNoNumber(Image image)
    {
        if (image.sprite.name == "SinNumero")
        {
            image.color = Color.red;
            TMP_InputField newInputField = Instantiate(inputFieldPrefab, image.transform);
            TextMeshProUGUI placeholder = newInputField.placeholder.GetComponent<TextMeshProUGUI>();
            placeholder.text = "?";
            placeholder.color = Color.yellow;

            

            // Añadir el componente BlinkPlaceholder al InputField
            ParpadeoPlaceholder parpadeoScript = newInputField.gameObject.AddComponent<ParpadeoPlaceholder>();
            parpadeoScript.IniciarParpadeo(placeholder);

            // Añadir EventTrigger para OnPointerEnter y OnPointerExit
            EventTrigger eventTrigger = newInputField.gameObject.AddComponent<EventTrigger>();
            AddEventTriggerListener(eventTrigger, EventTriggerType.PointerEnter, (eventData) =>
            {
                placeholder.text = "";
                placeholder.color = Color.clear;

                currentInputField = newInputField;
                parpadeoScript.DetenerParpadeo(); // Detener el parpadeo cuando el jugador empiece a escribir

            });
            AddEventTriggerListener(eventTrigger, EventTriggerType.PointerExit, (eventData) => currentInputField = null);

            // Añadir los eventos al InputField
            //newInputField.onEndEdit.AddListener(delegate { ValidateInputs(); });
            //newInputField.onEndEdit.AddListener(delegate { CompruebaTodosInputFieldsCompletos(); });
            newInputField.onEndEdit.AddListener(delegate { HandleInputFieldEndEdit(); });
            newInputField.onValueChanged.AddListener(delegate { ValidateInputField(newInputField); });
        }
    }

   
    private void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, Action<BaseEventData> action)
    {
        EventTrigger.Entry entry = new() { eventID = eventType };
        entry.callback.AddListener((data) => action(data));
        trigger.triggers.Add(entry);
    }

    private void HandleKeyboardInput(TMP_InputField currentInputField)
    {
        TMP_InputField[] inputFields = panelCrucigrama.GetComponentsInChildren<TMP_InputField>();
        int currentIndex = Array.IndexOf(inputFields, currentInputField);

        // Mover al siguiente InputField vacío en orden
        for (int i = 1; i < inputFields.Length; i++)
        {
            int nextIndex = (currentIndex + i) % inputFields.Length;
            if (string.IsNullOrEmpty(inputFields[nextIndex].text))
            {
                inputFields[nextIndex].Select();
                return;
            }
        }

        CompruebaTodosInputFieldsCompletos();
    }

    private void HandleInputFieldEndEdit()
    {
        // Verificar si todos los campos están completos después de la edición
        CompruebaTodosInputFieldsCompletos();
    }

    private void CompruebaTodosInputFieldsCompletos()
    {
        // Solo proceder si la validación está permitida
        if (!shouldValidate) return;

        // Encontrar todos los InputFields dentro del panel de crucigrama
        TMP_InputField[] inputFields = panelCrucigrama.GetComponentsInChildren<TMP_InputField>();

        foreach (TMP_InputField inputField in inputFields)
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                allInputFieldsFilled = false;
                return; // No validar hasta que todos los campos estén llenos
            }
        }
        allInputFieldsFilled = true;
        ValidateInputs();
    }


    public void ValidateInputs()
    {
        // Solo proceder si todos los campos de entrada están llenos
        if (!allInputFieldsFilled) return;

        // Encontrar todos los InputFields dentro del panel de crucigrama
        TMP_InputField[] inputFields = panelCrucigrama.GetComponentsInChildren<TMP_InputField>();

      

        bool allCorrect = true;

        if (inputFields[0].text != crucigramaActual.Resultado_1)
        {
            allCorrect = false;
            //Debug.Log("entrada =" + inputFields[0].text + " resultado =" + crucigramaActual.Resultado_1);
        }

        if (inputFields[1].text != crucigramaActual.Resultado_2)
        {
            allCorrect = false;
            //Debug.Log("entrada =" + inputFields[1].text + " resultado =" + crucigramaActual.Resultado_2);
        }

        if (inputFields[2].text != crucigramaActual.Resultado_3)
        {
            allCorrect = false;
            //Debug.Log("entrada =" + inputFields[2].text + " resultado =" + crucigramaActual.Resultado_3);
        }

        if (allCorrect)
        {
            mensaje.text = "¡Todas las respuestas son correctas!";
            foreach (TMP_InputField inputField in inputFields)
            {
                inputField.image.color = Color.green;
                inputField.textComponent.color = Color.black;
            }

            temporizador.DetenerTemporizador();

            // Incrementa el conocimiento en 1
            conocimientoCalculoCorrecto = intentosRestantes;
            GameManager.gameManager.SetConocimiento(conocimientoCalculoCorrecto);

            // Desactivar los paneles y limpiar las imágenes después de 5 segundos
            StartCoroutine(DesactivarPanelesYLimpiar());
        }
        else
        {
            intentosRestantes--;

            if (intentosRestantes > 0)
            {
                mensaje.text = "Incorrecto. Te quedan " + intentosRestantes + " intentos.";

                foreach (TMP_InputField inputField in inputFields)
                {
                    inputField.image.color = Color.red;
                    inputField.text = "";

                    // Reiniciar el parpadeo del placeholder
                    TextMeshProUGUI placeholder = inputField.placeholder.GetComponent<TextMeshProUGUI>();
                    placeholder.text = "?";
                    placeholder.color = Color.yellow;

                    // Reiniciar el componente ParpadeoPlaceholder
                    //ParpadeoPlaceholder blinkScript = inputField.GetComponent<ParpadeoPlaceholder>();
                    if (inputField.TryGetComponent<ParpadeoPlaceholder>(out var parpadeoScript))
                    {
                        parpadeoScript.IniciarParpadeo(placeholder);
                    }
                }

                temporizador.IniciarTemporizador(tiempoParaResolver);
            }
            else
            {
                mensaje.text = "Fallaste. No te quedan más intentos.";

                temporizador.DetenerTemporizador();

                // Decrementa el conocimiento en 1
                GameManager.gameManager.SetConocimiento(conocimientoCalculoIncorrecto);

                // Desactivar los paneles y limpiar las imágenes después de tiempo segundos
                StartCoroutine(DesactivarPanelesYLimpiar());
            }
            
        }
        

    }


    // Corrutina para desactivar paneles y limpiar imágenes
    private IEnumerator DesactivarPanelesYLimpiar()
    {
        yield return new WaitForSeconds(tiempoPanelCrucigrama);

        if (panelCrucigrama != null) panelCrucigrama.SetActive(false);
        if (mensaje != null) mensaje.gameObject.SetActive(false);

        LimpiarImagenes();

        // Limpiar InputFields anteriores
        LimpiarInputFields();

        // Reactivar el movimiento del player
        playerControl.SetMovimientoPlayer(true);

        hayColisionConBoxCrucigrama = false;

        shouldValidate = false; // Deshabilitar la validación después de la primera llamada
    }

    private void LimpiarImagenes()
    {
        for (int i = 0; i < imagenOperando.Length; i++)
        {
            if (imagenOperando[i] != null)
            {
                imagenOperando[i].sprite = null;
                imagenOperando[i].color = Color.white; // Restablecer el color predeterminado
            }
        }
    }

    
    public void ValidateInputField(TMP_InputField inputField)
    {
        if (!int.TryParse(inputField.text, out _))
        {
            inputField.text = "";
        }


    }

    // Script adicional para el parpadeo del placeholder
    public class ParpadeoPlaceholder : MonoBehaviour
    {
        private Coroutine coroutineParpadeo;
        private TextMeshProUGUI placeholder;

        public void IniciarParpadeo(TextMeshProUGUI placeholder)
        {
            this.placeholder = placeholder;
        }

        private void OnEnable()
        {
            coroutineParpadeo ??= StartCoroutine(Parpadeo());
        }

        private void OnDisable()
        {
            DetenerParpadeo();
        }

        public void DetenerParpadeo()
        {
            if (coroutineParpadeo != null)
            {
                StopCoroutine(coroutineParpadeo);
                coroutineParpadeo = null;
            }
        }

        private IEnumerator Parpadeo()
        {
            while (true)
            {
                placeholder.color = placeholder.color == Color.yellow ? Color.clear : Color.yellow;
                yield return new WaitForSeconds(0.5f);
            }
        }

    }
}
