using UnityEngine;
using UnityEngine.UI;

public class ControlPanelConfiguracionAudio : MonoBehaviour
{
    private void OnEnable()
    {
        // Llamamos a la función del AudioManager para asignar las referencias cuando el panel se activa
        AudioManager.audioManager.AsignaReferenciasPanelConfiguracionAudio();
        AudioManager.audioManager.CargaConfiguracionSonido();
    }
}
