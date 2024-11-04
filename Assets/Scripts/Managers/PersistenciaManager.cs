using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersistenciaManager : MonoBehaviour
{
    public static PersistenciaManager Instance { get; private set; }


    [SerializeField] private string keyStopMusica;          // Referencia al key StopMusica
    [SerializeField] private string keyVolumenMusica;       // Referencia al key VolumenMusica
    [SerializeField] private string keyStopSonido;          // Referencia al key StopSonido
    [SerializeField] private string keyVolumenSonido;       // Referencia al key VolumenSonido

    [SerializeField] private string keyVidasJugador;            // Referencia al key vidas del jugador
    [SerializeField] private string keyNivel;                   // Referencia al key del Nivel jugado
    [SerializeField] private string keyConocimientoNivel;       // Referencia al key del conocimiento que se logra por cada Nivel
    [SerializeField] private string keyConocimientoTotal;       // Referencia al key del conocimiento total logrado en el juego
    [SerializeField] private string keyConocimientoTotalNivel;  // Referencia al key del conocimiento total logrado en cada nivel

    [SerializeField] private string keyConocimientoSumas;               // Referencia al key del conocimiento total logrado en sumas
    [SerializeField] private string keyConocimientoRestas;              // Referencia al key del conocimiento total logrado en restas
    [SerializeField] private string keyConocimientoMultiplicaciones;    // Referencia al key del conocimiento total logrado en multiplicaciones
    [SerializeField] private string keyConocimientoDivisiones;          // Referencia al key del conocimiento total logrado en divisiones

    [SerializeField] private string keyTerminaJuego;        // Referencia al key de la bandera terminaJuego
    [SerializeField] private string keyJuegaNivel;          // Referencia al key de la bandera juegaNivel
    [SerializeField] private string keyRepiteNivel;         // Referencia al key de la bandera repiteNivel
    [SerializeField] private string keyTerminaNivel;        // Referencia al key de la bandera terminaNivel

    public static string KeyStopMusica { get => Instance.keyStopMusica; }
    public static string KeyVolumenMusica { get => Instance.keyVolumenMusica; }
    public static string KeyStopSonido { get => Instance.keyStopSonido; }
    public static string KeyVolumenSonido { get => Instance.keyVolumenSonido; }


    public static string KeyVidasJugador => Instance.keyVidasJugador;
    public static string KeyNivel => Instance.keyNivel;
    public static string KeyConocimientoNivel => Instance.keyConocimientoNivel;
    public static string KeyConocimientoTotal => Instance.keyConocimientoTotal;
    public static string KeyConocimientoTotalNivel => Instance.keyConocimientoTotalNivel;

    public static string KeyConocimientoSumas => Instance.keyConocimientoSumas;
    public static string KeyConocimientoRestas => Instance.keyConocimientoRestas;
    public static string KeyConocimientoMultiplicaciones => Instance.keyConocimientoMultiplicaciones;
    public static string KeyConocimientoDivisiones => Instance.keyConocimientoDivisiones;

    public static string KeyTerminaJuego => Instance.keyTerminaJuego;
    public static string KeyJuegaNivel => Instance.keyJuegaNivel;
    public static string KeyRepiteNivel => Instance.keyRepiteNivel;
    public static string KeyTerminaNivel => Instance.keyTerminaNivel;




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public float GetFloat(string key, float defaultValue = 0.0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public void SetBool(string key, bool state)
    {
        PlayerPrefs.SetInt(key, state ? 1 : 0);
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        int value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
        return value == 1;
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteKey(KeyNivel);
        PlayerPrefs.DeleteKey(KeyConocimientoNivel);
        PlayerPrefs.DeleteKey(keyConocimientoTotal);
        PlayerPrefs.DeleteKey(KeyConocimientoSumas);
        PlayerPrefs.DeleteKey(KeyConocimientoRestas);
        PlayerPrefs.DeleteKey(KeyConocimientoMultiplicaciones);
        PlayerPrefs.DeleteKey(KeyConocimientoDivisiones);
        PlayerPrefs.DeleteKey(KeyTerminaJuego);
        PlayerPrefs.DeleteKey(KeyJuegaNivel);
        PlayerPrefs.DeleteKey(KeyRepiteNivel);
        PlayerPrefs.DeleteKey(KeyTerminaNivel);

    }

    // Gestiona el guardado de la configuración de la musica
    public void SaveMusicaConfig(bool isMuted, float volumen)
    {
        SetBool(KeyStopMusica, isMuted);
        SetFloat(KeyVolumenMusica, volumen);

    }

    // Gestiona el guardado de la configuración del sonido
    public void SaveSonidoConfig(bool isMuted, float volumen)
    {
        SetBool(KeyStopSonido, isMuted);
        SetFloat(KeyVolumenSonido, volumen);

    }

    // Devuelve false si no está muteado por defecto
    public bool LoadMuteMusicaConfig()
    {
        return GetBool(KeyStopMusica, false); 
    }

    // Devuelve 0.17f si no hay valor guardado
    public float LoadVolumenMusicaConfig()
    {
        return GetFloat(KeyVolumenMusica, 0.17f); 
    }

    // Devuelve false si no está muteado por defecto
    public bool LoadMuteSonidoConfig()
    {
        return GetBool(KeyStopSonido, false); 
    }

    // Devuelve 0.3f si no hay valor guardado
    public float LoadVolumenSonidoConfig()
    {
        return GetFloat(KeyVolumenSonido, 0.3f); 
    }

    // Guarda el conocimiento del nivel especificado
    public void SaveConocimientoTotalNivel(int indice, int conocimientoNivel)
    {
        SetInt(KeyConocimientoTotalNivel + indice, conocimientoNivel);
        Save();
        //Debug.Log("KeyConocimientoTotalNivel"+"["+ indice + "]"+ conocimientoNivel);
    }

    // Recupera el valor guardado para el nivel dado (índice nivel - 1) y devuelve 0 si no hay valor guardado
    public int LoadConocimientoTotalNivel(int indice)
    {
        return GetInt(KeyConocimientoTotalNivel + indice, 0);
    }

    public void DeleteConocimientoTotalNivel(int nivel)
    {

        for(int i = 0; i < nivel; i++)
        {
            // Generar la clave única para el nivel usando el índice
            string key = KeyConocimientoTotalNivel + i;

            // Eliminar el valor específico del arreglo en PlayerPrefs
            DeleteKey(key);

        }
        
    }

}
