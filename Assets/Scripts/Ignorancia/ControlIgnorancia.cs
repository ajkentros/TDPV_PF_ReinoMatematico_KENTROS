using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;



public class ControlIgnorancia : MonoBehaviour
{

    [Header("Ignorancia")]
    [SerializeField] private Slider sliderIgnorancia;               // Que mide el conocimiento
    [SerializeField] private float distanciaMinima;                 // Distancia m�nima para seguir al player
    [SerializeField] private float distanciaMaxima;                 // Distancia m�xima para dejar de seguir al player
    [SerializeField] private Animator animator;                     // Referencia al Animator
    [SerializeField] private float velocidadInicial = 0f;           // Velocidad inicial del NavMeshAgent
    [SerializeField] private float velocidadAcercamiento = 0.5f;    // Velocidad de movimiento cuando Ignorancia detecta al Player
    [SerializeField] private int vidaIgnorancia = 3;           // Vida de Ignorancia


    [Header("Player")]
    [SerializeField] private PlayerControl playerControl;           // Referencia a la clase Player

    [Header("Paneles")]
    [SerializeField] private GameObject panelCalculoIgnorancia;
    [SerializeField] private GameObject panelOpciones;
    [SerializeField] private TextMeshProUGUI mensaje;

    [Header("Operandos")]
    [SerializeField] private Image imageOperando_1;
    [SerializeField] private Image imageOperando_2;
    [SerializeField] private Image imageOperando_3;

    [Header("Opciones")]
    [SerializeField] private Button botonOpcion_1;
    [SerializeField] private Button botonOpcion_2;
    [SerializeField] private Button botonOpcion_3;
    [SerializeField] private Sprite sinNumero;

    [Header("CalculoData")]
    [SerializeField] private CalculoData[] calculoDatas;

    private Sprite resultadoEsperado;                       // Sprite con el resultado de la ecuaci�n
    private bool haDetectadoAlPlayer = false;               // Para controlar si Ignorancia ha detectado al Player
    private bool estaVolando = false;                       // Indica si Ignorancia est� volando (en reposo = false = cuando no vuela)
    private bool panelCalculoIgnoranciaActivo = false;      // Bandera que habilita los desaf�os
    private float tiempoTranscurrido = 0f;                  // Tiempo transcurrido para incrementar la velocidad
    private float distanciaAlPlayer;                        // Distancia entre Ignorancia y Player
    private List<int> indicesDisponibles;                   // Lista con los �ndices para las 3 ecuaciones

    private NavMeshAgent navMeshAgent;



    void Start()
    {
        if (!TryGetComponent<NavMeshAgent>(out navMeshAgent))
        {
            Debug.LogError("NavMeshAgent no est� asignado al Prefab.");
        }

        // Configurar el NavMeshAgent para que funcione en 2D
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.speed = velocidadInicial;

        // Verificar si el NavMeshAgent est� en el NavMesh
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("No hay NavMesh");
        }

        IniciaPaneles();

        // Inicializar y barajar la lista de �ndices
        indicesDisponibles = new List<int>();

        for (int i = 0; i < calculoDatas.Length; i++)
        {
            indicesDisponibles.Add(i);
        }

        // 
        EligeCalculoData(indicesDisponibles);

        // Configura el valor m�ximo del Slider como 1 (100%)
        vidaIgnorancia = 3;
        sliderIgnorancia.maxValue = 1f;
        ActualizaSliderIgnorancia();
    }

    private void IniciaPaneles()
    {
        if (panelCalculoIgnorancia != null) panelCalculoIgnorancia.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (mensaje != null) mensaje.gameObject.SetActive(true);
    }

    void Update()
    {
        // Si el player es nulo => return
        if (playerControl == null) return;

        // Calcula la distancia entre ignorancia y el player
        distanciaAlPlayer = Vector2.Distance(transform.position, playerControl.transform.position);
        
        GestionaIgnoranciaCerca();


        // Si la distancia al player < distancia m�nima
        if (distanciaAlPlayer < distanciaMinima && !panelCalculoIgnoranciaActivo)
        {
            // Debug.Log(vidaIgnorancia);

            // 
            panelCalculoIgnoranciaActivo = true;

            // Ejecutar l�gica cuando Ignorancia est� cerca del jugador
            GestionaIgnoranciaDetectaPlayer();

            // Asigna operandos y opciones de alg�n scriptableObject CalculoData
            AsignaOperandosOpciones();

            // Muestra el panel c�lculo cuando detecta al player
            MuestraPanelCalculoIgnorancia();

            // Verifica que la opci�n elegida = resultado esperado
            VerificaOpcionElegida();

        }
        else if (distanciaAlPlayer > distanciaMaxima)
        {
            // Ejecutar l�gica cuando Ignorancia est� lejos del jugador
            GestionaIgnoranciaLejosDelPlayer();


            // Permite que se muestre nuevamente si el jugador se aleja y regresa
            panelCalculoIgnoranciaActivo = false;

        }


        if (haDetectadoAlPlayer && navMeshAgent.isOnNavMesh && estaVolando)
        {
            navMeshAgent.SetDestination(playerControl.transform.position);
            GestionaVelocidadIgnorancia();

        }
        else
        {
            navMeshAgent.ResetPath();
        }
    }

    // Gestiona si el player est� cerca de ignorancia
    private void GestionaIgnoranciaCerca()
    {
        if (distanciaAlPlayer <= (distanciaMinima + 1) && distanciaAlPlayer > distanciaMinima)
        {
            mensaje.text = "IGNORANCIA EST� CERCA, SI TE ALCANZA, PIERDES VIDA";
        }
    }

    // Gestiona cuando Ignorancia detecta al Player
    private void GestionaIgnoranciaDetectaPlayer()
    {
        // Inicia la detecci�n
        //Debug.Log("player detectado");

        // Desactiva musica 1 y activa sonido 2
        AudioManager.audioManager.StopMusicaFondo(1);
        AudioManager.audioManager.PlaySonidos(2);

        haDetectadoAlPlayer = true;

        mensaje.text = "IGNORANCIA TE DETECT�, RESUELVE LOS C�LCULOS";

        // Activa la animaci�n de ignorancia
        estaVolando = true;
        animator.SetBool("Volando", true);

        // Desactiva el movimiento del player
        playerControl.SetMovimientoPlayer(false);


    }

    private void AsignaOperandosOpciones()
    {

        if (indicesDisponibles.Count == 0) return;

        int indice = indicesDisponibles[0];
        indicesDisponibles.RemoveAt(0);

        CalculoData data = calculoDatas[indice];

        imageOperando_1.sprite = data.Operando_1;
        imageOperando_2.sprite = data.Operando_2;
        imageOperando_3.sprite = data.Operando_3;
        resultadoEsperado = data.Resultado;

        botonOpcion_1.GetComponentInChildren<Image>().sprite = data.Opcion_1;
        botonOpcion_2.GetComponentInChildren<Image>().sprite = data.Opcion_2;
        botonOpcion_3.GetComponentInChildren<Image>().sprite = data.Opcion_3;

    }

    private void MuestraPanelCalculoIgnorancia()
    {

        panelCalculoIgnorancia.SetActive(true);
        panelOpciones.SetActive(true);
        mensaje.gameObject.SetActive(true);

    }

    private void GestionaIgnoranciaLejosDelPlayer()
    {
        // Desactiva animaci�n "Volando" de ignorancia
        haDetectadoAlPlayer = false;
        estaVolando = false;
        animator.SetBool("Volando", false);
    }
           
    private void VerificaOpcionElegida()
    {
        // Asigna m�todos de clic a los botones de las opciones
        botonOpcion_1.onClick.RemoveAllListeners();
        botonOpcion_2.onClick.RemoveAllListeners();
        botonOpcion_3.onClick.RemoveAllListeners();

        // Crea una variable local para almacenar la opci�n seleccionada
        Sprite opcionSeleccionada = null;

        // Clic en opcion1
        botonOpcion_1.onClick.AddListener(() => {
            // Asigna a opcionSeleccionada el sprite de la opcion
            opcionSeleccionada = botonOpcion_1.GetComponentInChildren<Image>().sprite;

            // Llama al m�todo que cubre el operando que falta en el panelCalculoIgnorancia
            MuestraOpcionSeleccionadaEnOperando(opcionSeleccionada);

            // Llama al m�todo que verifica el resultado pasando la opcionSeleccionada
            VerificarResultado(opcionSeleccionada);
        });

        // Clic en opcion2
        botonOpcion_2.onClick.AddListener(() => {
            // Asigna a opcionSeleccionada el sprite de la opcion
            opcionSeleccionada = botonOpcion_2.GetComponentInChildren<Image>().sprite;

            // Llama al m�todo que cubre el operando que falta en el panelCalculoIgnorancia
            MuestraOpcionSeleccionadaEnOperando(opcionSeleccionada);

            // Llama al m�todo que verifica el resultado pasando la opcionSeleccionada
            VerificarResultado(opcionSeleccionada);
        });

        // Clic en opcion3
        botonOpcion_3.onClick.AddListener(() => {
            // Asigna a opcionSeleccionada el sprite de la opcion
            opcionSeleccionada = botonOpcion_3.GetComponentInChildren<Image>().sprite;

            // Llama al m�todo que cubre el operando que falta en el panelCalculoIgnorancia
            MuestraOpcionSeleccionadaEnOperando(opcionSeleccionada);


            // Llama al m�todo que verifica el resultado pasando la opcionSeleccionada
            VerificarResultado(opcionSeleccionada);
        });

    }

    // 
    private void MuestraOpcionSeleccionadaEnOperando(Sprite opcionSeleccionada)
    {

        // Verifica cu�l operando est� vac�o y asigna la opci�n seleccionada
        if (imageOperando_1.sprite == sinNumero)
        {
            imageOperando_1.sprite = opcionSeleccionada; //Debug.Log("imageOperando_1");
        }

        if (imageOperando_2.sprite == sinNumero)
        {
            imageOperando_2.sprite = opcionSeleccionada; //Debug.Log("imageOperando_2");
        }

        if (imageOperando_3.sprite == sinNumero)
        {
            imageOperando_3.sprite = opcionSeleccionada; //Debug.Log("imageOperando_3");
        }


    }

    // 
    private void VerificarResultado(Sprite imagenSeleccionada)
    {
        
        if (imagenSeleccionada == resultadoEsperado)
        {
            mensaje.text = "MUY BIEN, DESAF�IO RESUELTO";
            vidaIgnorancia --; //Debug.Log(vidaIgnorancia);

            // Actualiza el slider despu�s de reducir la vida
            ActualizaSliderIgnorancia();

            // Verifica si la vida de Ignorancia es 0
            VerificaVidasIgnorancia();

        }
        else
        {
            mensaje.text = "INCORRECTA ELECCI�N";

            StartCoroutine(DesactivaPanelesYMuestraNuevoCalculo());
        }

        
    }

    private void VerificaVidasIgnorancia()
    {
        if (vidaIgnorancia <= 0)
        {
            //
            // Muestra mensaje de eliminaci�n
            mensaje.text = "ELIMINSATE A IGNORANCIA Y GANASTE CONOCIMIENTO";
            mensaje.gameObject.SetActive(true);

            // Activa sonidos
            AudioManager.audioManager.SetLoopSonidos(1, false);
            AudioManager.audioManager.PlaySonidos(1);
            AudioManager.audioManager.PlayMusicaFondo(1);

            

            StartCoroutine(IgnoranciaEliminada());

            // Aumenta el conocimiento del player
            GameManager.gameManager.SetConocimiento(3);

            // Permite que el Player se mueva nuevamente
            playerControl.SetMovimientoPlayer(true);
        }
        else
        {
            StartCoroutine(DesactivaPanelesYMuestraNuevoCalculo());
        }
    }

    private IEnumerator IgnoranciaEliminada()
    {
        // Espera 2 segundos
        yield return new WaitForSeconds(1f);
        
        // Limpia paneles y sprites
        LimpiaPaneles();
        LimpiaSprites();

        // Destruye Ignorancia
        Destroy(gameObject);


    }

    

    private IEnumerator DesactivaPanelesYMuestraNuevoCalculo()
    {
        // Activa sonidos
        AudioManager.audioManager.SetLoopSonidos(1, false);
        AudioManager.audioManager.PlaySonidos(1);
        AudioManager.audioManager.PlayMusicaFondo(1);

        // Espera 2 segundos
        yield return new WaitForSeconds(2f);

        // Limpia paneles y sprites
        LimpiaPaneles();
        LimpiaSprites();

        // Verifica si hay m�s c�lculos disponibles
        if (indicesDisponibles.Count > 0 && vidaIgnorancia > 0)
        {
            AsignaOperandosOpciones();
            MuestraPanelCalculoIgnorancia();
            VerificaOpcionElegida();
        }
        else
        {
            // Aqu� podr�as manejar el caso en que no hay m�s c�lculos
            mensaje.text = "No hay m�s desaf�os";
        }
    }

    //
    private void LimpiaPaneles()
    {
        panelCalculoIgnorancia.SetActive(false);
        panelOpciones.SetActive(false);
        mensaje.text = "";
    }

    private void LimpiaSprites()
    {
        // Restablece los sprites a un estado "vac�o" o inactivo
        imageOperando_1.sprite = null;
        imageOperando_2.sprite = null;
        imageOperando_3.sprite = null;

        //Debug.Log("Sprites limpiados.");

        botonOpcion_1.GetComponentInChildren<Image>().sprite = null;
        botonOpcion_2.GetComponentInChildren<Image>().sprite = null;
        botonOpcion_3.GetComponentInChildren<Image>().sprite = null;

        resultadoEsperado = null; // Limpia el resultado esperado tambi�n
    }
    private void GestionaVelocidadIgnorancia()
    {
        // Incrementar la velocidad del NavMeshAgent a medida que pasa el tiempo
        tiempoTranscurrido += Time.deltaTime;
        navMeshAgent.speed = velocidadInicial + tiempoTranscurrido * velocidadAcercamiento;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("colision� con el player");
            // Reduce la vida del player
            playerControl.SetMovimientoPlayer(true);
            
            // Activa sonidos
            AudioManager.audioManager.SetLoopSonidos(1, false);
            AudioManager.audioManager.PlaySonidos(1);
            
            // Reduce vida del player
            GameManager.gameManager.DecrementaVidas();

            // Limpia panales
            LimpiaPaneles();

            // Limpia sprite
            LimpiaSprites();

            // Destruye ingnorancia
            Destroy(gameObject);
        }
    }

    private void EligeCalculoData(List<int> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            (lista[rnd], lista[i]) = (lista[i], lista[rnd]);
        }
    }

    private void ActualizaSliderIgnorancia()
    {
        // Calcula el porcentaje de vida en funci�n de las vidas restantes
        sliderIgnorancia.value = vidaIgnorancia / 3f; // vidaIgnorancia puede ser 3, 2, 1

        // Tambi�n puedes actualizar el color del Slider para mejorar la visualizaci�n (opcional)
        Color sliderColor = Color.Lerp(Color.red, Color.green, sliderIgnorancia.value);
        sliderIgnorancia.fillRect.GetComponent<Image>().color = sliderColor;
    }
}

