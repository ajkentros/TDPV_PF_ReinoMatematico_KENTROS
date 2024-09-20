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
    [SerializeField] private TextMeshProUGUI apellidoMatematico;    // Texto donde se mostrar�n las letras
    [SerializeField] private TextMeshProUGUI mensaje;               // PanelCalculo en el Canvas
    [SerializeField] private TMP_InputField inputApellido;          // Campo de texto para ingresar el apellido

    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemapSalida;                 // Referencia al Tilemap de Salida


    [Header("MatematicoData")]
    //[SerializeField] private MatematicoData matematicoData;  // Array de ScriptableObjects
    [SerializeField] private List<MatematicoData> matematicoDataList;   // Lista de ScriptableObjects del tipo MatematicoData


    [Header("MatematicoData")]
    [SerializeField] private PlayerControl playerControl; // Referencia al script del Player

    private List<char> letrasDesordenadas;      // Lista que contiene las letras desordenadas del matem�tico oculto
    private int indiceLetraActual;              // Contador de letras descubiertas
    private string nombreMatematico;            // String que guarda el nombre del matem�tico oculto
    private string descripcionMatematico;       // String que guarda la descripci�n del matem�tico oculto
    private int intentosRestantes = 3;          // Contador de intentos para descubrir al matem�tico oculto

    private void OnEnable()
    {
        // Suscribe al evento cuando el objeto est� habilitado
        BoxMatematico.OnPlayerCollideBoxMatematico += MuestraLetra;
        //Debug.Log("Suscrito al evento OnPlayerCollide");
    }

    private void OnDisable()
    {
        // Desuscribe del evento cuando el objeto est� deshabilitado
        BoxMatematico.OnPlayerCollideBoxMatematico -= MuestraLetra;
        //Debug.Log("Desuscrito del evento OnPlayerCollide");
    }

    private void Start()
    {
        // Activa los paneles que se usar�n
        if (panelMatematico != null) panelMatematico.SetActive(false);
        if (panelIngresarApellido != null) panelIngresarApellido.SetActive(false);
        if (mensaje != null) mensaje.gameObject.SetActive(true);

        // Instancializa cada BoxMatem�tico;
        InicializaMatematico();
    }



    private void InicializaMatematico()
    {
        // Define la variable nivel con el nivel actual del juego
        int nivelActual = GameManager.gameManager.GetNivel(); //Debug.Log("Controlador matem�tico nivel actual = " + nivelActual);

        // Calcula el �ndice de matem�ticos ocultos
        int indice = nivelActual - 1;

        // Si el nivel actual del juego < que el tama�o de la lista de MatematicoData =>
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

            // Inicializa el �ndice de la lista en 0
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

            // Incremenata el �ndice
            indiceLetraActual++;

            // Si es la �ltima letra => seguir con el programa
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
        // Muestra mensaje para ordenar letras despu�s de 1 segundos
        yield return new WaitForSeconds(1f);
        mensaje.text = "";
        mensaje.text = "Ordena las letras y descubre al matem�tico oculto";

        // Muestra descripci�n despu�s de otros 3 segundos
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

            // Establece el l�mite de caracteres seg�n el nombre del matem�tico oculto
            inputApellido.characterLimit = nombreMatematico.Length;

            // A�ade listener para verificar el apellido
            inputApellido.onEndEdit.AddListener(VerificarApellido);

        }
    }

    // Gestiona la verificaci�n del apellido cargado en texto input
    private void VerificarApellido(string textoIngresado)
    {
        // Si textoIngresado = al nombre del matem�tico oculto =>
        if (textoIngresado.Equals(nombreMatematico, StringComparison.OrdinalIgnoreCase))
        {
            // Emite mensaje
            mensaje.text = "Descubriste al matem�tico oculto: " + nombreMatematico;

            // Desactiva el Tilemap de Salida
            if (tilemapSalida != null)
            {
                tilemapSalida.gameObject.SetActive(false);
                StartCoroutine(SalidaLiberada());
            }

            // Habilita el movimiento del player
            playerControl.SetMovimientoPlayer(true);

            // Llama a la corrutina para resetear el matem�tico oculto para el pr�ximo nivel
            StartCoroutine(ResetControlMatematico());

            // Llama al m�todo en GameManager que avisa que se descubri� al matem�tico oculto
            GameManager.gameManager.SetMatematicoDescubierto(true);
        }
        else
        {
            // Desactiva el movimiento del player
            playerControl.SetMovimientoPlayer(false);

            // Disminuye la cantidad de intentos para descubrir al matem�tico oculto
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
                mensaje.text = "Fallaste. No te quedan m�s intentos.";

                // Desactiva el texto input
                inputApellido.interactable = false;

                // Llama a corrutiina para resetera el matem�tico oculto
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
        mensaje.text = "Busca m�s letras";

        // Esperar 3 segundos
        yield return new WaitForSeconds(2);

        // Restaura el mensaje al original o a otro mensaje
        mensaje.text = "";

        // Desactiva sonido 2 y Activa m�sica de fondo 1
        AudioManager.audioManager.StopSonido(2);
        AudioManager.audioManager.PlayMusicaFondo(1);

        // Habilita el movimiento del player
        playerControl.SetMovimientoPlayer(true);
    }

    // Gestiona el reseteo del matem�tico oculto
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

        // Llama al m�todo que carga el elemento de la lista de ScriptableObject con los matem�ticos ocultos
        InicializaMatematico();

        // Desatctiva paneles que no se usan
        panelIngresarApellido.SetActive(false);
        panelMatematico.SetActive(false);
        mensaje.text = "";

        // Desactiva sonido 2 y Activa m�sica de fondo 1
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
