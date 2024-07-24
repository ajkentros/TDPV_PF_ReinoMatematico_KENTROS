using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;     // Referencia instancia estática del GameManager para acceder desde otros scripts

    public delegate void VidasActualizadas(int vidas);
    public event VidasActualizadas OnVidasActualizadas;

    public delegate void RepiteNivelActualizado(bool repiteNivel);
    public event RepiteNivelActualizado OnRepiteNivelActualizado;

    [SerializeField] private int vidasInicio = 3;         // Referencia variable vidas del jugador

    private Reloj cronometro;

    private int conocimiento;
    private int conocimientoTotal;
    private int[] conocimientoTotalNivel = new int[10];
    private readonly int[] conocimientoMaximoNivel = { 25, 25, 25, 25, 25, 25, 25, 25 };  // Conocimiento máximo por nivel

    private int nivel;      // nivel = 0 es la escena 1 

    private readonly int maxVidas = 5;  // Máximo número de vidas

    private bool juegaNivel;  // Variable que indica si el nivel está en juego
    private bool repiteNivel;
    private bool matematicoDescubierto;
    private bool juegoPausado;






    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;

            // Evita que el objeto GameManager se destruya al cargar una nueva escena.
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Mover la suscripción aquí para asegurar que siempre esté activa

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        IniciaStart();

    }
    private void Update()
    {
        // Iniciar cronómetro cuando el nivel está en juego

        if (juegaNivel && cronometro != null && !cronometro.EstaCorriendoCronometro())
        {
            cronometro.IniciarCronometro(); //Debug.Log("IniciarCronometro()");
        }

        // Gestiona la pausa con la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausarJuego();
        }

        VerificaEscenas();



        // Para debug y pruebas
        if (Input.GetKeyDown(KeyCode.L))
        {
            IncrementaVidas();
            Debug.Log("clic en L");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            DecrementaVidas();
            Debug.Log("clic en K");
        }
    }



    private void OnDestroy()
    {
        if (gameManager == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Desuscribirse del evento de carga de escenas
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        vidasInicio = 3; // Inicializar con 3 vidas


        // Verifica si la escena es una de las escenas de juego
        if (scene.buildIndex != 0)  // Si el índice de la escena es diferente de 0
        {

            // Buscar el objeto que contiene el script Reloj
            GameObject reloj = GameObject.Find("Reloj"); //Debug.Log("encontró el reloj ");

            if (reloj != null)
            {
                if (reloj.TryGetComponent<Reloj>(out cronometro))
                {
                    juegaNivel = true; //Debug.Log("juegaNivel " + juegaNivel);
                    repiteNivel = false;
                }
                else
                {
                    Debug.LogError("No se encontró el componente Temporizador en el objeto Temporizador");
                }
            }
            else
            {
                Debug.LogError("No se encontró el objeto Temporizador en la escena");
            }
        }

        StartCoroutine(InvokeVidasActualizadas());


    }

    private IEnumerator InvokeVidasActualizadas()
    {
        // Esperar un frame para asegurar que todos los objetos estén inicializados y suscritos
        yield return null;
        OnVidasActualizadas?.Invoke(vidasInicio);
        //Debug.Log("evento OnVidasActualizadas invocado con vidas: " + vidas);
    }


    public void IncrementaVidas()
    {
        if (vidasInicio < maxVidas)
        {
            vidasInicio++;
            OnVidasActualizadas?.Invoke(vidasInicio);
        }
    }

    public void DecrementaVidas()
    {
        //Debug.Log("vidas antes "+ vidasInicio);
        if (vidasInicio > 0)
        {
            
            vidasInicio--;
            if (vidasInicio == 0)
            {
                RepiteNivel();
            }
            OnVidasActualizadas?.Invoke(vidasInicio);

        } 

    }

    public void RegistrarControlPanelVidas(ControlPanelVidas controlPanelVidas)
    {
        //Debug.Log("RegistrarControlPanelVidas");
        OnVidasActualizadas += controlPanelVidas.ActualizaVidas;
        // Notificar al panel inicial el estado de las vidas
        OnVidasActualizadas?.Invoke(vidasInicio);
        //Debug.Log("RegistrarControlPanelVidas con vidas: " + vidas);
    }

    public void DesregistrarControlPanelVidas(ControlPanelVidas controlPanelVidas)
    {
        //Debug.Log("DesregistrarControlPanelVidas");
        OnVidasActualizadas -= controlPanelVidas.ActualizaVidas;

    }

    public void NivelTerminado()
    {
        if (cronometro != null)
        {
            juegaNivel = false;
            cronometro.DetenerTemporizador();
            Debug.Log("Terminó el nivel " + "Tiempo total: " + cronometro.ObtenerTiempoTranscurrido());
        }
    }

    public void SetConocimiento(int _conocimiento)
    {
        // Solo permitir decrementos si conocimiento es mayor que 0
        if (_conocimiento < 0 && conocimiento == 0)
        {
            // No hacer nada si _conocimiento es negativo y conocimiento es 0
            return;
        }

        conocimiento += _conocimiento;

        // Asegurarse de que conocimiento no sea negativo
        if (conocimiento < 0)
        {
            conocimiento = 0;
        }

        conocimientoTotalNivel[nivel] = conocimiento;
        conocimientoTotal += _conocimiento;

        Debug.Log(conocimiento);
    }

    public int GetConocimiento()
    {
        return conocimiento;
    }

    public int GetConocimientoDelNivel()
    {
        return conocimientoMaximoNivel[nivel];
    }

    public void SetMatematicoDescubierto()
    {
        matematicoDescubierto = !matematicoDescubierto;
    }

    public void PausarJuego()
    {
        if (juegoPausado)
        {
            ActivaContinuaJuego();
        }
        else
        {
            ActivaPausaJuego();
        }
    }

    public bool GetJuegoPausado()
    {
        return juegoPausado;
    }

    public void SetJuegoPausado(bool pausa)
    {
        juegoPausado = pausa;
    }

    private void ActivaPausaJuego()
    {
        SetJuegoPausado(true);
        Time.timeScale = 0f;
        DetenerCronometro();
    }

    private void ActivaContinuaJuego()
    {
        SetJuegoPausado(false);
        Time.timeScale = 1f;
        ContinuaCronometro();
    }

    public void ContinuaCronometro()
    {
        if (cronometro != null && !cronometro.EstaCorriendoCronometro())
        {
            cronometro.IniciarCronometro();
        }
    }

    public void DetenerCronometro()
    {
        if (cronometro != null)
        {
            cronometro.DetenerCronometro();
        }
    }

    // Gestiona el reinicio del juego
    public void ReiniciaJuego()
    {
        IniciaStart();
        
    }

    // Gestiona el inicio de las variables
    private void IniciaStart()
    {
        

        conocimientoTotal = 0;
        matematicoDescubierto = false;
        repiteNivel = false;
    }

    private void VerificaEscenas()
    {
        // Obtiene el índice de la escena actual
        int escenaActual = SceneManager.GetActiveScene().buildIndex;

        if (escenaActual > 0)
        {
            // Asigna el nivel basado en la escena actual menos 1
            nivel = escenaActual - 1; 
            //Debug.Log("nivel = " + nivel);
        }
        
    }

    public void RepiteNivel()
    {
        juegaNivel = false;
        repiteNivel = true;
        Debug.Log("Disparando evento OnRepiteNivelActualizado");
        OnRepiteNivelActualizado?.Invoke(repiteNivel);

         
    }

    public bool GetRepiteNivel()
    {
        return repiteNivel;
    }


}
