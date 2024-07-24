using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlPanelResumenNivel : MonoBehaviour
{
    [SerializeField] private GameObject panelResumenNivel;
    [SerializeField] private GameObject boton;
    [SerializeField] private TextMeshProUGUI mensaje;
    [SerializeField] private Sprite spriteFondo;  // Imagen que se mostrará cuando el panel esté activo

    private Image panelImage;

    private void Awake()
    {
        // Obtener el componente Image del panel
        panelImage = panelResumenNivel.GetComponent<Image>();
    }


    private void OnEnable()
    {
        //GameManager.gameManager.OnRepiteNivelActualizado += ActualizaPanelResumenNivel; Debug.Log("ActualizaPanelResumenNivel");

        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.OnRepiteNivelActualizado += ActualizaPanelResumenNivel;
            Debug.Log("Suscrito a OnRepiteNivelActualizado");
        }
        else
        {
            Debug.LogError("GameManager.gameManager es nulo en OnEnable de ControlPanelResumenNivel");
        }
    }

    private void OnDisable()
    {
        GameManager.gameManager.OnRepiteNivelActualizado -= ActualizaPanelResumenNivel;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Asegura que el fondo esté transparente al inicio
        Color colorTransparente = panelImage.color;
        colorTransparente.a = 0f;  // Hacer completamente transparente
        panelImage.color = colorTransparente;
        mensaje.gameObject.SetActive(false);
        boton.SetActive(false);

    }

    
    private void ActualizaPanelResumenNivel(bool repiteNivel)
    {

        if (repiteNivel)
        {
            // Cambiar el texto del mensaje
            mensaje.gameObject.SetActive(true);
            mensaje.text = "Tendrás que repetir el nivel";

            boton.SetActive(true);

            // Cambiar el sprite del fondo
            panelImage.sprite = spriteFondo;

            // Cambiar el color para hacer el fondo opaco
            Color colorOpaco = panelImage.color;
            colorOpaco.r = 1f;  // Hacer completamente opaco
            colorOpaco.g = 0f;  // Hacer completamente opaco
            colorOpaco.b = 0f;  // Hacer completamente opaco
            colorOpaco.a = 1f;  // Hacer completamente opaco
            panelImage.color = colorOpaco;
        }
        else
        {
            // Mantener el panel transparente si no se repite el nivel
            Color colorTransparente = panelImage.color;
            colorTransparente.a = 0f;
            panelImage.color = colorTransparente;
        }
    }

    public void BotonInicioReino()
    {
        SceneManager.LoadScene(0);
    }
}
