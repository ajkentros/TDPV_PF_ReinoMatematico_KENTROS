using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;


public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;                              // Referencia instancia estática del GameManager para acceder desde otros scripts
        
    public delegate void VidasActualizadas(int vidas);                  // Evento que se dispara cuando se actualizan las vidas
    public event VidasActualizadas OnVidasActualizadas;     

    public delegate void ConocimientoActualizado(int conocimiento);     //Evento que se dispara cuando se actualiza el conociento
    public event ConocimientoActualizado OnConocimientoActualizado;

    public delegate void NivelTerminado();                              // Evento que se dispara cuando se termina el nivel
    public event NivelTerminado OnNivelTerminado;

    public delegate void NivelRepetido();                               // Evento que se dispara cuando se repite el nivel
    public event NivelRepetido OnNivelRepetido;

    public delegate void JuegoTerminado();                               // Evento que se dispara cuando se termina el jeugo
    public event JuegoTerminado OnJuegoTerminado;


    [SerializeField] private int[] conocimientoMaximoNivel;  // Conocimiento máximo por nivel
    
    private Reloj cronometro;       // Referencia a la clase Reloj para el cronómetro del juego

    private int conocimientoNivel;          // Cuenta el conocimiento que se logra por cada Nivel
    private int conocimientoTotal;          // Cuenta el conocimiento total logrado en el juego
    private int[] conocimientoTotalNivel;   // Guarda el conocimiento logrado en cada nivel
    
    private int conocimientoSumas;          // Guarda las sumatorias de conocimiento en los desafíos de sumas
    private int conocimientoRestas;         // Guarda las sumatorias de conocimiento en los desafíos de restas
    private int conocimientoMultiplicaciones; // Guarda las sumatorias de conocimiento en los desafíos de multiplicaciones
    private int conocimientoDivisiones;       // Guarda las sumatorias de conocimiento en los desafíos de divisiones


    private int nivel;                      // Inicio escena 1 = nivel 0
    private int totalNiveles;               // Referencia a 16 niveles (+ de 0 a 3; - de 4 a 7; x de 8 a 11 y / de 12 a 15)
    
    private int vidasInicio = 3;            // Referencia variable vidas del jugador// nivel = 0 es la escena 1 
    private readonly int maxVidas = 5;      // Máximo número de vidas

    private bool juegaNivel;                // Bandera que indica si el nivel está en juego
    private bool terminaNivel;              // Bandera que avisa si termina el nivel
    private bool repiteNivel;               // Bandera que avisa si se repite el nivel
    private bool terminaJuego;              // Bandera que avisa si se termina el juego
    private bool matematicoDescubierto;     // Bandera que avisa si se descubrió el matemático
    private bool juegoPausado;              // Bandera que avisa si el juego se pausó

    [SerializeField] private PersistenciaManager persistenciaManager;    // Rreferencia al PersistenciaManager

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (gameManager != this)
        {
            Destroy(gameObject);
        }

        // Cuenta la cantidad de escenas - 1 para saber los niveles del juego
        totalNiveles = SceneManager.sceneCountInBuildSettings - 1;  Debug.Log("totalNiveles =" + totalNiveles);

        // Inicializa el arreglo conocimientoTotalNiveles con la dimensión adecuada
        conocimientoTotalNivel = new int[totalNiveles];

        // Cargar datos de persistencia al iniciar el juego
        CargarDatosPersistenciaManager();
    }

    public void CargarDatosPersistenciaManager()
    {
        // Persistencia
        nivel = PersistenciaManager.Instance.GetInt(PersistenciaManager.KeyNivel, 0);
        Debug.Log("GameManager-CargaDatosPersistenciaManager " + "nivel= " + nivel);

        vidasInicio = PersistenciaManager.Instance.GetInt(PersistenciaManager.KeyVidasJugador, 3);
        
        conocimientoNivel = PersistenciaManager.Instance.GetInt(PersistenciaManager.KeyConocimientoNivel, 0);
        
        conocimientoTotal = PersistenciaManager.Instance.GetInt(PersistenciaManager.KeyConocimientoTotal, 0);
        Debug.Log("GameManager-CargaDatosPersistenciaManager " + "conocimientoTotal = " + conocimientoTotal);

        //int indice = Mathf.Max(nivel - 1, 0);
        for (int i = 0; i < nivel; i++)
        {
            conocimientoTotalNivel[i] = PersistenciaManager.Instance.LoadConocimientoTotalNivel(i);

        }

        conocimientoSumas = PersistenciaManager.Instance.GetInt(PersistenciaManager.KeyConocimientoSumas, 0);
        Debug.Log("GameManager-CargaDatosPersistenciaManager " + "+ = " + conocimientoSumas);

        conocimientoRestas = PersistenciaManager.Instance.GetInt(PersistenciaManager.KeyConocimientoRestas, 0);
        Debug.Log("GameManager-CargaDatosPersistenciaManager " + "- = " + conocimientoRestas);
        
        conocimientoMultiplicaciones = PersistenciaManager.Instance.GetInt(PersistenciaManager.KeyConocimientoMultiplicaciones, 0);
        Debug.Log("GameManager-CargaDatosPersistenciaManager " + "* = " + conocimientoMultiplicaciones);
        
        conocimientoDivisiones = PersistenciaManager.Instance.GetInt(PersistenciaManager.KeyConocimientoDivisiones, 0);
        Debug.Log("GameManager-CargaDatosPersistenciaManager " + "/ = " + conocimientoDivisiones);

        terminaJuego = PersistenciaManager.Instance.GetBool(PersistenciaManager.KeyTerminaJuego, false);    
        //Debug.Log("GameManager-CargarDatosPersistenciaManager() " + "terminaJuego= " + terminaJuego);
        
        juegaNivel = PersistenciaManager.Instance.GetBool(PersistenciaManager.KeyJuegaNivel, false);
        repiteNivel = PersistenciaManager.Instance.GetBool(PersistenciaManager.KeyRepiteNivel, false);
        terminaNivel = PersistenciaManager.Instance.GetBool(PersistenciaManager.KeyTerminaNivel, false);

    }

    private void Start()
    {
        IniciaStart();

    }

    // Gestiona el inicio de las variables
    private void IniciaStart()
    {
        
       
        if(nivel == 0)
        {
            conocimientoTotal = 0;
        }
        
        matematicoDescubierto = false;
        repiteNivel = false;
        terminaNivel = false;
        //terminaJuego = false;
        AudioManager.audioManager.PlayMusicaFondo(0);
        AudioManager.audioManager.StopSonidos();
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

        // Verifica y actualiza el nivel basado en la escena activa
        VerificaEscenas();

        // Para debug y pruebas: Incrementa o Decrementa vidas
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    IncrementaVidas();
        //    Debug.Log("clic en L");
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    DecrementaVidas();
        //    Debug.Log("clic en K");
        //}
    }

    // Gestiona la verificación de escenas para saber si termina el nivel o el juego
    private void VerificaEscenas()
    {
        // Obtiene el índice de la escena actual
        int escenaActual = SceneManager.GetActiveScene().buildIndex;

        //Debug.Log("VerificaEscenas()" + "escena = "+ escenaActual + " nivel = " + nivel);

        // Obtiene el índice de la última escena
        int ultimaEscena = SceneManager.sceneCountInBuildSettings - 1;

        // Verifica si la escena actual es la última
        if (escenaActual == ultimaEscena || nivel == ultimaEscena)
        {
            terminaJuego = true;
        }
    }

    // Suscribe al evento de cargar la escena
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    // Desuscribirse del evento de carga de escenas
    private void OnDisable()
    {
        if (gameManager == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; 
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Inicializar con 3 vidas
        vidasInicio = 3;

        // cacula indiceEscena = numero de escena actual
        int indiceEscena = scene.buildIndex;

        // Verifica si la escena es una de las escenas de juego
        if (indiceEscena != 0)  // Si el índice de la escena es diferente de 0
        {
            // Activa los audios
            AudioManager.audioManager.StopMusicaFondo(0);
            AudioManager.audioManager.PlayMusicaFondo(1);

            // Buscar el objeto que contiene el script Reloj
            GameObject reloj = GameObject.Find("Reloj"); //Debug.Log("encontró el reloj ");

            // Si el reloj existe => 
            if (reloj != null)
            {
                if (reloj.TryGetComponent<Reloj>(out cronometro))
                {
                    // Configura banderas
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

        // Llama a la corrutina que actualiza las vidas del nivel
        StartCoroutine(InvokeVidasActualizadas());


    }

    // Gestiona la corrutina que invoca un evento con el valor de la vida
    private IEnumerator InvokeVidasActualizadas()
    {
        // Espera un frame para asegurar que todos los objetos estén inicializados y suscritos
        yield return null;

        // Invoca el evento OnVidasActualizadas para actualizar la pantalla con el valor de las vidas del player
        OnVidasActualizadas?.Invoke(vidasInicio);
        //Debug.Log("evento OnVidasActualizadas invocado con vidas: " + vidas);
    }

    // Gestiona el incremento de vidas del player
    public void IncrementaVidas()
    {
        // Si la vida < vida máxima =>
        if (vidasInicio < maxVidas)
        {
            // Incrementa la vida
            vidasInicio++;

            // Invoca el evento OnVidasActualizadas para actualizar la pantalla con el valor de las vidas del player
            OnVidasActualizadas?.Invoke(vidasInicio);
        }
    }

    // Gestiona el decremento de vidas del player
    public void DecrementaVidas()
    {
        //Debug.Log("vidas antes "+ vidasInicio);
        // Si la vida > 0 =>
        if (vidasInicio > 0)
        {
            // Decrementa la vida en 1
            vidasInicio--;

            // Si vida = 0 =>
            if (vidasInicio == 0)
            {
                // Llama a la función que gestiona repetir el nivel
                RepiteNivel();
            }

            // Invoca el evento OnVidasActualizadas para actualizar la pantalla con el valor de las vidas del player
            OnVidasActualizadas?.Invoke(vidasInicio);
        } 
    }

    // Gestiona la actualización del panel vidas
    public void RegistrarControlPanelVidas(ControlPanelVidas controlPanelVidas)
    {
        //Debug.Log("RegistrarControlPanelVidas");
        OnVidasActualizadas += controlPanelVidas.ActualizaVidas;
        
        // Notificar al panel inicial el estado de las vidas
        OnVidasActualizadas?.Invoke(vidasInicio);
        //Debug.Log("RegistrarControlPanelVidas con vidas: " + vidas);
    }

    // Gestiona la actualización del panel vidas
    public void DesregistrarControlPanelVidas(ControlPanelVidas controlPanelVidas)
    {
        //Debug.Log("DesregistrarControlPanelVidas");
        OnVidasActualizadas -= controlPanelVidas.ActualizaVidas;

    }

    // Gestiona cuando termina un nivel
    public void TerminaNivel()
    {
        if (cronometro != null && matematicoDescubierto && vidasInicio > 0)
        {
            // Establece variables para el nivel terminado
            juegaNivel = false;
            terminaNivel = true;
            terminaJuego = false;
            
            // Detiene el cronómetro
            cronometro.DetenerTemporizador();

            // Sumar conocimiento logrado por categoría
            CalculaConocimientoPorDesafios(); 
            
            Debug.Log("GameManager - NivelTerminado(): nivel = " + nivel);

            // Usa el nivel como índice, restando 1 porque los arrays empiezan en 0, pero asegurando que no es negativo
            int indice = Mathf.Max(nivel - 1, 0);

            // Calcula el conocimiento total del juego = acumulado del conocimiento de cada nivel
            conocimientoTotal += conocimientoTotalNivel[indice];

            Debug.Log("conocimiento del nivel = " + conocimientoNivel + " conocimientoTotal = " + conocimientoTotal);

            int siguienteNivel = nivel + 1;

            // Verifica si el juego ha alcanzado el último nivel
            if (siguienteNivel >= SceneManager.sceneCountInBuildSettings)
            {
                terminaJuego = true;
                // Disparar el evento de juego terminado
                OnJuegoTerminado?.Invoke();
            }

            GuardarProgreso();

            // Disparar el evento de nivel terminado
            OnNivelTerminado?.Invoke();
        }
    }

    // Gestiona cuando se repite un nivel
    public void RepiteNivel()
    {
        // Actualiza banderas para repetir el nivel
        juegaNivel = false;
        repiteNivel = true;

        // Si el nivel > 0 => 
        if (nivel > 0)
        {
            // Decrementa el valor de nivel para jugar el mismo
            nivel--;
        }

        // Usa el nivel como índice, restando 1 porque los arrays empiezan en 0, pero asegurando que no es negativo
        int indice = Mathf.Max(nivel - 1, 0);

        // Actualiza el valor del conocimiento de ese nivel = 0 (volver a comenzar)
        conocimientoTotalNivel[indice] = 0;

        GuardarProgreso();

        // Disparar el evento de nivel repetido
        OnNivelRepetido?.Invoke();

    }

    // Gestiona el cálculo del conocimiento logrado
    private void CalculaConocimientoPorDesafios()
    {
        // Obtiene el conocimiento logrado en el nivel actual
        int indice = Mathf.Max(nivel - 1, 0);
        int conocimientoNivel = conocimientoTotalNivel[indice];

        // Determinar la categoría según el nivel (ajustando índices)
        if (nivel >= 1 && nivel <= 4) // Niveles 1-4: Sumas
        {
            conocimientoSumas += conocimientoNivel;
        }
        else if (nivel >= 5 && nivel <= 8) // Niveles 5-8: Restas
        {
            conocimientoRestas += conocimientoNivel;
        }
        else if (nivel >= 9 && nivel <= 12) // Niveles 9-12: Multiplicaciones
        {
            conocimientoMultiplicaciones += conocimientoNivel;
        }
        else if (nivel >= 13 && nivel <= 16) // Niveles 13-16: Divisiones
        {
            conocimientoDivisiones += conocimientoNivel;
        }

    }

    // Set conocimiento logrado en el nivel que se juega
    public void SetConocimiento(int _conocimiento)
    {
        // Solo permitir decrementos si conocimiento es mayor que 0
        if (_conocimiento < 0 && conocimientoNivel == 0)
        {
            // No hacer nada si _conocimiento es negativo y conocimiento es 0
            return;
        }

        conocimientoNivel += _conocimiento;

        // Asegurarse de que conocimiento no sea negativo
        if (conocimientoNivel < 0)
        {
            conocimientoNivel = 0;
        }

        // Usa el nivel como índice, restando 1 porque los arrays empiezan en 0, pero asegurando que no es negativo
        int indice = Mathf.Max(nivel - 1, 0);
        
        //Debug.Log("GameManager - SetConocimiento -> " + "conocimientoNivel = "+conocimientoNivel + " conocimientoTotalNivel[" + indice + "] = " + conocimientoTotalNivel[indice]);
        
        // Calcula en conocimiento total del nivel = conocimiento logrado en ese nivel
        conocimientoTotalNivel[indice] = conocimientoNivel;
        

        // Incova al evento que actualiza el conociento logrado
        OnConocimientoActualizado?.Invoke(_conocimiento);

        GuardarProgreso();
        
    }

    // Get conocimiento logrado en el nivel que se juega
    public int GetConocimiento()
    {
        return conocimientoNivel;
    }

    // Get conocimiento total por niveles
    public int GetconocimientoTotalNivel(int indice)
    {
        
        //Debug.Log("Game Manager - nivel = " + nivel + "GetconocimientoTotalNivel[" + indice + "] = "+ conocimientoTotalNivel[indice]);
        return conocimientoTotalNivel[indice];
    }

    // Get total de niveles
    public int GetTotalNiveles()
    {
        return totalNiveles;
    }


    // Get conocimiento total logrado el juego
    public int GetConocimientoTotal()
    {
        Debug.Log("conocimiento Final = " + conocimientoTotal);
        return conocimientoTotal;

    }

    // Get conocimiento máximo que se puede alcanzar en el nivel
    public int GetConocimientoMaximoNivel()
    {
        // Usa el nivel como índice, restando 1 porque los arrays empiezan en 0, pero asegurando que no es negativo
        int indice = Mathf.Max(nivel - 1, 0);

        return conocimientoMaximoNivel[indice];
    }

    
    // Get conocimientos de las sumas
    public int GetConocimientoSumas()
    {
        Debug.Log("conocimiento de + = " + conocimientoSumas);
        return conocimientoSumas;
    }

    // Get conocimientos de las restas
    public int GetConocimientoRestas()
    {
        Debug.Log("conocimiento de - = " + conocimientoRestas);
        return conocimientoRestas;
    }

    // Get conocimientos de las multiplicaciones
    public int GetConocimientoMultiplicaciones()
    {
        Debug.Log("conocimiento de * = " + conocimientoMultiplicaciones);
        return conocimientoMultiplicaciones;
    }

    // Get conocimientos de las divisiones
    public int GetConocimientoDivisiones()
    {
        Debug.Log("conocimiento de / = " + conocimientoDivisiones);
        return conocimientoDivisiones;
    }

    // Set cambia la bandera de matemático descubierto
    public void SetMatematicoDescubierto(bool _matematicoDescubierto)
    {
        matematicoDescubierto = _matematicoDescubierto;
    }

    // Gestiona la pausa del juego
    public void PausarJuego()
    {
        
        // Cambia la bandera 
        juegoPausado = !juegoPausado;
        // Si juego está pausado = true => 
        if (juegoPausado)
        {
            // Activa los audios
            AudioManager.audioManager.StopMusicaFondo(1);
            AudioManager.audioManager.PlayMusicaFondo(0);

            // Detiene el cronómetro
            DetenerCronometro(); 
            // Pausa el juego
            Time.timeScale = 0f; 
        }
        else
        {
            // Activa los audios
            AudioManager.audioManager.StopMusicaFondo(0);
            AudioManager.audioManager.PlayMusicaFondo(1);

            // Reanuda el juego
            Time.timeScale = 1f;
            
            // Continua con el cronómetro
            ContinuaCronometro();
        }
       
    }

    // Get del juego pausado
    public bool GetJuegoPausado()
    {
        return juegoPausado;
    }

    //public void SetJuegoPausado(bool pausa)
    //{
    //    juegoPausado = pausa;
    //}

    //private void ActivaPausaJuego()
    //{
    //    SetJuegoPausado(true);
    //    Time.timeScale = 0f;
    //    DetenerCronometro();
    //}

    //private void ActivaContinuaJuego()
    //{
    //    SetJuegoPausado(false);
    //    Time.timeScale = 1f;
    //    ContinuaCronometro();
    //}

    // Gestiona el inicio o continuidad del cronómetro
    public void ContinuaCronometro()
    {
        if (cronometro != null && !cronometro.EstaCorriendoCronometro())
        {
            // Llama a la función propia de la clase Reloj
            cronometro.IniciarCronometro();
        }
    }

    // Gestiona la detención del cronómetro
    public void DetenerCronometro()
    {
        if (cronometro != null)
        {
            // Llama a la función propia de la clase Reloj
            cronometro.DetenerCronometro();
        }
    }

    // Gestiona el reinicio del juego
    public void VolverMenuInicio()
    {
        IniciaStart();
        
    }

    // Gestiona el reinicio del juego
    public void JuegaNivel(int escena)
    {
        IniciaNivel(escena);

    }
    // Gestiona el inicio de las variables del siguiente nivel
    private void IniciaNivel(int escena)
    {
        conocimientoNivel = 0;
        nivel = escena; Debug.Log("nivel = " + nivel);
        matematicoDescubierto = false;
        repiteNivel = false;
        terminaNivel = false;
        terminaJuego = false;
        AudioManager.audioManager.PlayMusicaFondo(0);
        AudioManager.audioManager.StopSonidos();
    }

    public void ReiniciaJuego()
    {
        if (terminaJuego)
        {
            Debug.Log("ReiniciaJuego");
            nivel = 0;
            conocimientoTotal = 0;
            matematicoDescubierto = false;
            repiteNivel = false;
            terminaNivel = false;
            terminaJuego = false;
            AudioManager.audioManager.PlayMusicaFondo(0);
            AudioManager.audioManager.StopSonidos();

        }

        
    }

    // Get la bandera para avisar que se repite un nivel
    public bool GetRepiteNivel()
    {
        
        return repiteNivel; 
    }

    // Get la bandera para avisar que termina un nivel
    public bool GetTerminaNivel()
    {

        return terminaNivel;
    }

    // Get la bandera para avisar el numero de nivel
    public int GetNivel()
    {
        //Debug.Log("GetNivel()" + nivel);
        return nivel;
    }

    // Get la bandera para avisar que termina el juego
    public bool GetTerminaJuego()
    {
        //Debug.Log("GameManager-GetTerminaJuego() " + "terminaJuego= " + terminaJuego);
        return terminaJuego;
    }

    // Set de la bandera termina el juego
    public void SetTerminaJuego(bool _terminaJuego)
    {
        terminaJuego = _terminaJuego;
    }

    // Guarda el progreso del juego
    public void GuardarProgreso()
    {
        // VidasJugador
        PersistenciaManager.Instance.SetInt(PersistenciaManager.KeyVidasJugador, vidasInicio);

        // Nivel
        PersistenciaManager.Instance.SetInt(PersistenciaManager.KeyNivel, nivel);
        
        // ConocimientoNivel
        PersistenciaManager.Instance.SetInt(PersistenciaManager.KeyConocimientoNivel, conocimientoNivel);
        
        // ConocimientoTotal
        PersistenciaManager.Instance.SetInt(PersistenciaManager.KeyConocimientoTotal, conocimientoTotal);

        // ConocimientoTotalNivel
        int indice = Mathf.Max(nivel - 1, 0);
        PersistenciaManager.Instance.SaveConocimientoTotalNivel(indice, conocimientoTotalNivel[indice]);

        // ConocimientoSumas
        PersistenciaManager.Instance.SetInt(PersistenciaManager.KeyConocimientoSumas, conocimientoSumas);
        
        // ConocimientoRestas
        PersistenciaManager.Instance.SetInt(PersistenciaManager.KeyConocimientoRestas, conocimientoRestas);
        
        // ConocimientoMultiplicaciones
        PersistenciaManager.Instance.SetInt(PersistenciaManager.KeyConocimientoMultiplicaciones, conocimientoMultiplicaciones);

        // ConocimientoDivisiones
        PersistenciaManager.Instance.SetInt(PersistenciaManager.KeyConocimientoDivisiones, conocimientoDivisiones);

        // terminaJuego
        PersistenciaManager.Instance.SetBool(PersistenciaManager.KeyTerminaJuego, terminaJuego);

        // juegaNivel
        PersistenciaManager.Instance.SetBool(PersistenciaManager.KeyJuegaNivel, juegaNivel);

        // repiteNivel
        PersistenciaManager.Instance.SetBool(PersistenciaManager.KeyRepiteNivel, repiteNivel);

        // terminaNivel
        PersistenciaManager.Instance.SetBool(PersistenciaManager.KeyTerminaNivel, terminaNivel);

        // Guarda
        PersistenciaManager.Instance.Save();
    }
}
