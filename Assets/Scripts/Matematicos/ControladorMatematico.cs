using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.UI;


public class ControladorMatematico : MonoBehaviour
{
    
    [Header("Paneles")]
    [SerializeField] private GameObject panelMatematico;   // PanelCalculo en el Canvas
    [SerializeField] private GameObject panelIngresarApellido; // Panel para ingresar el apellido
    [SerializeField] private TextMeshProUGUI apellidoMatematico;   // Texto donde se mostrarán las letras
    [SerializeField] private TextMeshProUGUI mensaje;   // PanelCalculo en el Canvas
    [SerializeField] private TMP_InputField inputApellido; // Campo de texto para ingresar el apellido
    

    [Header("MatematicoData")]
    [SerializeField] private MatematicoData matematicoData;  // Array de ScriptableObjects

    [Header("MatematicoData")]
    [SerializeField] private PlayerControl playerControl; // Referencia al script del Player

    private List<char> letrasDesordenadas;
    private int indiceLetraActual;
    private string nombreMatematico;
    private string descripcionMatematico;
    private int intentosRestantes = 3; // Contador de intentos

    private void OnEnable()
    {
        // Suscribirse al evento cuando el objeto está habilitado
        BoxMatematico.OnPlayerCollideBoxMatematico += MuestraLetra;
        //Debug.Log("Suscrito al evento OnPlayerCollide");
    }

    private void OnDisable()
    {
        // Desuscribirse del evento cuando el objeto está deshabilitado
        BoxMatematico.OnPlayerCollideBoxMatematico -= MuestraLetra;
        //Debug.Log("Desuscrito del evento OnPlayerCollide");
    }

    private void Start()
    {
        if (panelMatematico != null) panelMatematico.SetActive(false);
        if (panelIngresarApellido != null) panelIngresarApellido.SetActive(false);
        if (mensaje != null) mensaje.gameObject.SetActive(true);        // Asegúrate de que el Panel está desactivado al inicio
        
        //InstanciarBoxMatematico();
        
        InicializarMatematico();
    }



    private void InicializarMatematico()
    {
        if (matematicoData != null)
        {
            nombreMatematico = matematicoData.apellidoMatematico;
            descripcionMatematico = matematicoData.descripcionMatematico;
            letrasDesordenadas = nombreMatematico.ToCharArray().OrderBy(x => UnityEngine.Random.Range(0, nombreMatematico.Length)).ToList();
            indiceLetraActual = 0;
            apellidoMatematico.text = ""; // Limpia el texto de las letras encontradas
        }
        else
        {
            Debug.LogWarning("No se ha asignado MatematicoData");
        }
    }

    private void MuestraLetra(BoxMatematico box)
    {
        AudioManager.audioManager.StopMusicaFondo(1);
        AudioManager.audioManager.PlaySonidos(2);

        if (playerControl != null)
        {
            playerControl.SetMovimientoPlayer(false);

        }

        if (indiceLetraActual < letrasDesordenadas.Count)
        {
            //Debug.Log(letrasDesordenadas[indiceLetraActual]);
            box.SetLetra(letrasDesordenadas[indiceLetraActual]);

            if (!panelMatematico.activeSelf)
            {
                panelMatematico.SetActive(true);
                
            }
            mensaje.gameObject.SetActive(true);
            mensaje.text = "Descubrieste la letra " + letrasDesordenadas[indiceLetraActual];
            apellidoMatematico.text += letrasDesordenadas[indiceLetraActual];

            indiceLetraActual++;
            

            if (indiceLetraActual >= letrasDesordenadas.Count)
            {
                // Si es la última letra, seguir con el programa
                StartCoroutine(MostrarMensajesFinales());
            }
            else
            {
                StartCoroutine(LimpiarMensaje());
            }
            
        }

        

    }

    private IEnumerator MostrarMensajesFinales()
    {
        // Mostrar mensaje para ordenar letras después de 1 segundos
        yield return new WaitForSeconds(1f);
        mensaje.text = "Ordena las letras y descubre al matemático oculto";

        // Mostrar descripción después de otros 3 segundos
        yield return new WaitForSeconds(3f);
        mensaje.text = descripcionMatematico;

        // Activar el panel para ingresar el apellido
        ActivarPanelIngresarApellido();
    }

    public void ActivarPanelIngresarApellido()
    {
        
        //mensaje.text = descripcionMatematico;
        if (panelIngresarApellido != null)
        {
            //Debug.Log("ingrese apellido");

            panelIngresarApellido.SetActive(true);
            inputApellido.interactable = true;
            inputApellido.characterLimit = nombreMatematico.Length; // Establecer el límite de caracteres
            inputApellido.onEndEdit.AddListener(VerificarApellido); // Añadir listener para verificar el apellido
            
        }
    }

    private IEnumerator MuestraAlerta()
    {
        yield return new WaitForSeconds(5f);

        mensaje.text = "Ordena las letras y descubre al matemático oculto";
    }

    private IEnumerator MuestraDescripcion()
    {
        yield return new WaitForSeconds(5f);

        mensaje.text = descripcionMatematico;
    }

    private void VerificarApellido(string textoIngresado)
    {
        
        if (textoIngresado.Equals(nombreMatematico, StringComparison.OrdinalIgnoreCase))
        {
            mensaje.text = "Correcto";
            
            playerControl.SetMovimientoPlayer(true);
            StartCoroutine(ResetControlMatematico());
            GameManager.gameManager.SetMatematicoDescubierto(true);
        }
        else
        {
            playerControl.SetMovimientoPlayer(false);
            intentosRestantes--;
            if (intentosRestantes > 0)
            {
                mensaje.text = "Incorrecto. Te quedan " + intentosRestantes + " intentos.";
                StartCoroutine(LimpiarInputField());
            }
            else
            {
                mensaje.text = "Fallaste. No te quedan más intentos.";
                inputApellido.interactable = false;
                StartCoroutine(ResetControlMatematico());
                GameManager.gameManager.RepiteNivel();
                
            }
        }
        
        
    }

    private IEnumerator LimpiarInputField()
    {
        yield return new WaitForSeconds(2);
        inputApellido.text = "";
    }

    private IEnumerator LimpiarMensaje()
    {
        // Esperar 3 segundos
        yield return new WaitForSeconds(2);

        // Cambiar el mensaje inicial
        mensaje.text = "Busca más letras";

        // Esperar 3 segundos
        yield return new WaitForSeconds(1);

        // Restaurar el mensaje al original o a otro mensaje
        mensaje.text = "";

        playerControl.SetMovimientoPlayer(true);
        AudioManager.audioManager.StopSonido(2);
        AudioManager.audioManager.PlayMusicaFondo(1);
    }

    private IEnumerator ResetControlMatematico()
    {
        yield return new WaitForSeconds(3);
        inputApellido.text = "";
        inputApellido.interactable = true;
        intentosRestantes = 3;
        InicializarMatematico();
        panelIngresarApellido.SetActive(false);
        panelMatematico.SetActive(false);
        mensaje.gameObject.SetActive(false);
        AudioManager.audioManager.StopSonido(2);
        AudioManager.audioManager.PlayMusicaFondo(1);

    }
}
