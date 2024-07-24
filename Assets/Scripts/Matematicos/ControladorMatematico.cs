using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;


public class ControladorMatematico : MonoBehaviour
{
    //[Header("Prefabs BoxMatematico")]
    //[SerializeField] private GameObject boxMatematicoPrefab; // Prefab del BoxCalculo

    //[Header("Posiciones BoxMatematico")]
    //[SerializeField] private Transform[] posicionesMatematico; // Array de posiciones para instanciar BoxCalculo
    [Header("Paneles")]
    [SerializeField] private GameObject panelMatematico;   // PanelCalculo en el Canvas
    [SerializeField] private GameObject panelIngresarApellido; // Panel para ingresar el apellido
    [SerializeField] private TextMeshProUGUI apellidoMatematico;   // Texto donde se mostrarán las letras
    [SerializeField] private TextMeshProUGUI mensaje;   // PanelCalculo en el Canvas
    [SerializeField] private TMP_InputField inputApellido; // Campo de texto para ingresar el apellido

    [Header("MatematicoData")]
    [SerializeField] private MatematicoData matematicoData;  // Array de ScriptableObjects

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
        }
        else
        {
            Debug.LogWarning("No se ha asignado MatematicoData");
        }
    }

    private void MuestraLetra(BoxMatematico box)
    {
        
        if (indiceLetraActual < letrasDesordenadas.Count)
        {
            //Debug.Log(letrasDesordenadas[indiceLetraActual]);
            box.SetLetra(letrasDesordenadas[indiceLetraActual]);

            if (!panelMatematico.activeSelf)
            {
                panelMatematico.SetActive(true);
                
            }

            mensaje.text = "Descubrieste la letra " + letrasDesordenadas[indiceLetraActual];
            apellidoMatematico.text += letrasDesordenadas[indiceLetraActual];
            
            StartCoroutine(LimpiarMensaje());
            indiceLetraActual++;

            if (indiceLetraActual >= letrasDesordenadas.Count)
            {

                StartCoroutine(MuestraAlerta());
                mensaje.gameObject.SetActive(true);
                ActivarPanelIngresarApellido();
            }
        }
        else
        {
            mensaje.text = "No hay más letras";
            mensaje.gameObject.SetActive(true);
        }

        
    }
    public void ActivarPanelIngresarApellido()
    {
        
        StartCoroutine(MuestraDescripcion());

        mensaje.text = descripcionMatematico;

        if (panelIngresarApellido != null)
        {
            Debug.Log("ingrese apellido");
            panelIngresarApellido.SetActive(true);
            inputApellido.characterLimit = letrasDesordenadas.Count; // Establecer el límite de caracteres
            inputApellido.onEndEdit.AddListener(VerificarApellido); // Añadir listener para verificar el apellido
            //inputApellido.textComponent.color = Color.black; // Cambiar el color del texto ingresado
            //inputApellido.GetComponent<Image>().color = Color.white; // Cambiar el color de fondo



        }
    }

    private IEnumerator MuestraAlerta()
    {
        yield return new WaitForSeconds(4f);

        mensaje.text = "Ordena las letras y descubre al matemático oculto";
    }

    private IEnumerator MuestraDescripcion()
    {
        yield return new WaitForSeconds(3f);

        mensaje.text = descripcionMatematico;
    }

    private void VerificarApellido(string textoIngresado)
    {
        Debug.Log("verifica apellido");
        if (textoIngresado.Equals(nombreMatematico, StringComparison.OrdinalIgnoreCase))
        {
            mensaje.text = "Correcto";
            GameManager.gameManager.SetMatematicoDescubierto();
        }
        else
        {
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
                StartCoroutine(ResetGame());
                GameManager.gameManager.RepiteNivel();
                Debug.Log("RepiteNivel");
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
        yield return new WaitForSeconds(3);

        // Cambiar el mensaje inicial
        mensaje.text = "Busca más letras";

        // Esperar 3 segundos
        yield return new WaitForSeconds(2);

        // Restaurar el mensaje al original o a otro mensaje
        mensaje.text = "";
    }

    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(3);
        inputApellido.text = "";
        inputApellido.interactable = true;
        intentosRestantes = 3;
        InicializarMatematico();
        panelIngresarApellido.SetActive(false);
        panelMatematico.SetActive(false);
        mensaje.gameObject.SetActive(false);
    }
}
