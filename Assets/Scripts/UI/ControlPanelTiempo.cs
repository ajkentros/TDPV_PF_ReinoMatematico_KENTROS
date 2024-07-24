using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlPanelTiempo : MonoBehaviour
{
    [SerializeField] private Reloj cronometro;
    [SerializeField] private TextMeshProUGUI textoTiempo;

    void Update()
    {
        if (cronometro != null && textoTiempo != null)
        {
            float tiempo = cronometro.ObtenerTiempoTranscurrido(); //Debug.Log(tiempo);
            int minutos = Mathf.FloorToInt(tiempo / 60F);
            int segundos = Mathf.FloorToInt(tiempo % 60F);
            textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }
}
