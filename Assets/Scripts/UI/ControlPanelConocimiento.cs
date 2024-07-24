using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelConocimiento : MonoBehaviour
{
    [SerializeField] private Slider sliderConocimiento;   // slider que mide el conocimiento


    

    // Update is called once per frame
    private void Update()
    {
        ActualizaSliderConocimiento();
    }

    private void ActualizaSliderConocimiento()
    {
        if (GameManager.gameManager != null)
        {
            int conocimientoActual = GameManager.gameManager.GetConocimiento();
            int conocimientoMaximo = GameManager.gameManager.GetConocimientoDelNivel();

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
}

