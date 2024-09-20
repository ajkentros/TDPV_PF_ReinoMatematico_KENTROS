using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Tilemaps;


public class ControladorMatematico : MonoBehaviour
{

    [Header("Paneles")]
    [SerializeField] private GameObject panelMatematico;            // PanelCalculo en el Canvas
    [SerializeField] private GameObject panelIngresarApellido;      // Panel para ingresar el apellido
    [SerializeField] private TextMeshProUGUI apellidoMatematico;    // Texto donde se mostrarán las letras
    [SerializeField] private TextMeshProUGUI mensaje;               // PanelCalculo en el Canvas
    [SerializeField] private TMP_InputField inputApellido;          // Campo de texto para ingresar el apellido

    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemapSalida;                 // Referencia al Tilemap de Salida


    [Header("MatematicoData")]
    //[SerializeField] private MatematicoData matematicoData;  // Array de ScriptableObjects
    [SerializeField] private List<MatematicoData> matematicoDataList;   // Lista de ScriptableObjects del tipo MatematicoData


    [Header("MatematicoData")]
    [SerializeField] private PlayerControl playerControl; // Referencia al script del Player

    private List<char> letrasDesordenadas;      // Lista que contiene las letras desordenadas del matemático oculto
    private int indiceLetraActual;              // Contador de letras descubiertas
    private string nombreMatematico;            // String que guarda el nombre del matemático oculto
    private string descripcionMatematico;       // String que guarda la descripción del matemático oculto
    private int intentosRestantes = 3;          // Contador de intentos para descubrir al matemático oculto

    private void OnEnable()
    {
        // Suscribe al evento cuando el objeto está habilitado
        BoxMatematico.OnPlayerCollideBoxMatematico += MuestraLetra;
        //Debug.Log("Suscrito al evento OnPlayerCollide");
    }

    private void OnDisable()
    {
        // Desuscribe del evento cuando el objeto está deshabilitado
        BoxMatematico.OnPlayerCollideBoxMatematico -= MuestraLetra;
        //Debug.Log("Desuscrito del evento OnPlayerCollide");
    }

    private void Start()
    {
        // Activa los paneles que se usarán
        if (panelMatematico != null) panelMatematico.SetActive(false);
        if (panelIngresarApellido != null) panelIngresarApellido.SetActive(false);
        if (mensaje != null) mensaje.gameObject.SetActive(true);

        // Instancializa cada BoxMatemático;
        InicializaMatematico();
    }



    private void InicializaMatematico()
    {
        // Define la variable nivel con el nivel actual del juego
        int nivelActual = GameManager.gameManager.GetNivel(); //Debug.Log("Controlador matemático nivel actual = " + nivelActual);

        // Calcula el índice de matemáticos ocultos
        int indice = nivelActual - 1;

        // Si el nivel actual del juego < que el tamaño de la lista de MatematicoData =>
        if (indice < matematicoDataList.Count)
        {
            // Instancia variable matematicoData del tipo MatematicoData como el matematicoDataList[nivelActual]
            MatematicoData matematicoData = matematicoDataList[indice];

            // Asigna a nombreMatematico = el campo apellidoMatematico de la lista
            nombreMatematico = matematicoData.apellidoMatematico;       //Debug.Log(nombreMatematico);

            // // Asigna a descripcionMatematico = el campo descripcionMatematico de la lista
            descripcionMatematico = matematicoData.descripcionMatematico;

            // Asigna a la lista letrasDesordenadas as letras de nombreMatematico en forma aleatoria (desordenada)
            letrasDesordenadas = nombreMatematico.ToCharArray().OrderBy(x => UnityEngine.Random.Range(0, nombreMatematico.Length)).ToList();

            // Inicializa el índice de la lista en 0
            indiceLetraActual = 0;

            // Limpia el texto de las letras encontradas
            apellidoMatematico.text = "";
        }
        else
        {
            Debug.LogWarning("No hay un MatematicoData para el nivel actual");
        }


    }

    private void MuestraLetra(BoxMatematico box)
    {
        // Desactiva musica 1 y activa sonido 2
        AudioManager.audioManager.StopMusicaFondo(1);
        AudioManager.audioManager.PlaySonidos(2);

        if (playerControl != null)
        {
            // Desactiva movimeinto del player
            playerControl.SetMovimientoPlayer(false);

        }

        if (indiceLetraActual < letrasDesordenadas.Count)
        {
            //Debug.Log(letrasDesordenadas[indiceLetraActual]);
            box.SetLetra(letrasDesordenadas[indiceLetraActual]);

            if (!panelMatematico.activeSelf)
            {
                // Activa panel
                panelMatematico.SetActive(true);

            }

            // Completa el apellido con la letra descubierta (desordenada)
            apellidoMatematico.text += letrasDesordenadas[indiceLetraActual];

            // activa el mensaje
            mensaje.gameObject.SetActive(true);

            // Calcula cuantas letras quedan por descubrir
            int letrasPorDescubrir = letrasDesordenadas.Count - (indiceLetraActual + 1);

            // Emite el mensaje 
            mensaje.text = $"Descubriste la letra {letrasDesordenadas[indiceLetraActual]}. Quedan {letrasPorDescubrir} por descubrir.";

            // Incremenata el índice
            indiceLetraActual++;

            // Si es la última letra => seguir con el programa
            // Sino Limpia el mensaje
            if (indiceLetraActual >= letrasDesordenadas.Count)
            {
                // Llama a corrutina para mostrar mensajes
                StartCoroutine(MostrarMensajesFinales());
            }
            else
            {
                // Llama a corrutina para limpiar mensajes
                StartCoroutine(LimpiarMensaje());
            }
        }
    }

    private IEnumerator MostrarMensajesFinales()
    {
        // Muestra mensaje para ordenar letras después de 1 segundos
        yield return new WaitForSeconds(1f);
        mensaje.text = "";
        mensaje.text = "Ordena las letras y descubre al matemático oculto";

        // Muestra descripción después de otros 3 segundos
        yield return new WaitForSeconds(3f);
        mensaje.text = descripcionMatematico;

        // Activar el panel para ingresar el apellido
        ActivarPanelIngresarApellido();
    }

    public void ActivarPanelIngresarApellido()
    {

        // Si el planel de apellido no es nulo
        if (panelIngresarApellido != null)
        {
            //Debug.Log("ingrese apellido");
            // Activa el planel de apellido
            panelIngresarApellido.SetActive(true);

            // Activa el texto de input
            inputApellido.interactable = true;

            // Establece el límite de caracteres según el nombre del matemático oculto
            inputApellido.characterLimit = nombreMatematico.Length;

            // Añade listener para verificar el apellido
            inputApellido.onEndEdit.AddListener(VerificarApellido);

        }
    }

    // Gestiona la verificación del apellido cargado en texto input
    private void VerificarApellido(string textoIngresado)
    {
        // Si textoIngresado = al nombre del matemático oculto =>
        if (textoIngresado.Equals(nombreMatematico, StringComparison.OrdinalIgnoreCase))
        {
            // Emite mensaje
            mensaje.text = "Descubriste al matemático oculto: " + nombreMatematico;

            // Desactiva el Tilemap de Salida
            if (tilemapSalida != null)
            {
                tilemapSalida.gameObject.SetActive(false);
                StartCoroutine(SalidaLiberada());
            }

            // Habilita el movimiento del player
            playerControl.SetMovimientoPlayer(true);

            // Llama a la corrutina para resetear el matemático oculto para el próximo nivel
            StartCoroutine(ResetControlMatematico());

            // Llama al método en GameManager que avisa que se descubrió al matemático oculto
            GameManager.gameManager.SetMatematicoDescubierto(true);
        }
        else
        {
            // Desactiva el movimiento del player
            playerControl.SetMovimientoPlayer(false);

            // Disminuye la cantidad de intentos para descubrir al matemático oculto
            intentosRestantes--;

            // Si quedan intento =>
            if (intentosRestantes > 0)
            {
                // Emite un mensaje
                mensaje.text = "Incorrecto. Te quedan " + intentosRestantes + " intentos.";

                // Llama a la corrutina para limpiar el texto input
                StartCoroutine(LimpiarInputField());
            }
            else
            {
                // Si la cantidad de intentos = 0 => emite mensaje
                mensaje.text = "Fallaste. No te quedan más intentos.";

                // Desactiva el texto input
                inputApellido.interactable = false;

                // Llama a corrutiina para resetera el matemático oculto
                StartCoroutine(ResetControlMatematico());

                // Llama al GameManager para avisr que debe repetir el nivel
                GameManager.gameManager.RepiteNivel();

            }
        }


    }

    // Limpia el texto input
    private IEnumerator LimpiarInputField()
    {
        yield return new WaitForSeconds(3);
        inputApellido.text = "";
    }

    // Gestiona la limpieza de mensaje
    private IEnumerator LimpiarMensaje()
    {
        // Esperar 3 segundos
        yield return new WaitForSeconds(2);

        // Cambiar el mensaje inicial
        mensaje.text = "Busca más letras";

        // Esperar 3 segundos
        yield return new WaitForSeconds(2);

        // Restaura el mensaje al original o a otro mensaje
        mensaje.text = "";

        // Desactiva sonido 2 y Activa música de fondo 1
        AudioManager.audioManager.StopSonido(2);
        AudioManager.audioManager.PlayMusicaFondo(1);

        // Habilita el movimiento del player
        playerControl.SetMovimientoPlayer(true);
    }

    // Gestiona el reseteo del matemático oculto
    private IEnumerator ResetControlMatematico()
    {
        // Espera 3 segundos
        yield return new WaitForSeconds(3);

        // Limpia el texto input
        inputApellido.text = "";

        // Activa el texto input
        inputApellido.interactable = true;

        // Inicializa la variable de intentos para descubrir el matematico oculto
        intentosRestantes = 3;

        // Llama al método que carga el elemento de la lista de ScriptableObject con los matemáticos ocultos
        InicializaMatematico();

        // Desatctiva paneles que no se usan
        panelIngresarApellido.SetActive(false);
        panelMatematico.SetActive(false);
        mensaje.text = "";

        // Desactiva sonido 2 y Activa música de fondo 1
        AudioManager.audioManager.StopSonido(2);
        AudioManager.audioManager.PlayMusicaFondo(1);

    }

    private IEnumerator SalidaLiberada()
    {
        // Espera 3 segundos
        yield return new WaitForSeconds(2);
        
        // Cambiar el mensaje inicial
        mensaje.text = "Salida liberada del laberinto";
    }
}
