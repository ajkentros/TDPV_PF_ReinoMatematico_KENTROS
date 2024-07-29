using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelConocimiento : MonoBehaviour
{
    [SerializeField] private Slider sliderConocimiento;         // Que mide el conocimiento
    [SerializeField] private TextMeshProUGUI textConocimiento;  // Muestra conocimiento logrado en cada desafío

    private void OnEnable()
    {
        //Debug.Log("ControlPanelVidas OnEnable");

        // Suscribirse al evento del GameManager
        if (GameManager.gameManager != null)
        {

            GameManager.gameManager.OnConocimientoActualizado += MuestraConocimiento;       // Evento OnConocimientoActualizado
            //Debug.Log("Evento OnConocimientoActualizado");

        }
    }

    private void OnDisable()
    {
        //Debug.Log("ControlPanelVidas OnDisable");

        // Desuscribirse del evento del GameManager
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.OnConocimientoActualizado -= MuestraConocimiento;       // OnConocimientoActualizado

        }


    }

    // Update is called once per frame
    private void Update()
    {
        ActualizaSliderConocimiento();
    }

    private void ActualizaSliderConocimiento()
    {
        /*
         * Obtiene el conocimiento de GameManager
         * Obtiene el conocimiento máximo que se puede lograr en el nivel
         * Calcula porcentaje relativo del nivel
         * Configura el color del slider según el valor obtenido
         * Muestra el texto del conocimiento logrado
         */
        if (GameManager.gameManager != null)
        {
            int conocimientoActual = GameManager.gameManager.GetConocimiento();
            int conocimientoMaximo = GameManager.gameManager.GetConocimientoMaximoNivel();

            float porcentajeConocimiento = ((float)conocimientoActual / conocimientoMaximo) * 100f;
            sliderConocimiento.value = porcentajeConocimiento;


            

            // Cambia el color del temporizador
            Color sliderConocimientoColor;

            if (sliderConocimiento.value >= 70f)
            {
                sliderConocimientoColor = Color.green;
            }
            else if (sliderConocimiento.value >= 40f)
            {
                sliderConocimientoColor = Color.yellow;
            }
            else
            {
                sliderConocimientoColor = Color.red;
            }

            // Cambia el color del fill del slider
            sliderConocimiento.fillRect.GetComponent<Image>().color = sliderConocimientoColor;
        }
    }

    private void MuestraConocimiento(int conocimientoActual)
    {
        //Debug.Log("MmuestraConcimiento()");

        // Muestra el conocimiento actualizado
        textConocimiento.text = conocimientoActual >= 0 ? "+" + conocimientoActual : conocimientoActual.ToString();

        // Inicia la corrutina para borrar el texto después de 2 segundos
        StartCoroutine(BorraTextoConocimiento());
    }

    private IEnumerator BorraTextoConocimiento()
    {
        // Espera 2 segundos
        yield return new WaitForSeconds(2);
        // Borra el texto
        textConocimiento.text = "";
    }
}

