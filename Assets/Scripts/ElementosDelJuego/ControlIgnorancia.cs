using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ControlIgnorancia : MonoBehaviour
{
    [SerializeField] private Transform player;                  // Referencia al player
    [SerializeField] private float distanciaMinima;        // Distancia mínima para seguir al player
    [SerializeField] private float distanciaMaxima;        // Distancia máxima para dejar de seguir al player
    [SerializeField] private Animator animator;                 // Referencia al Animator
    [SerializeField] private float velocidadInicial = 3f;     // Velocidad inicial del NavMeshAgent
    [SerializeField] private float aceleracion = 0.1f;  // Incremento de velocidad por segundo


    private NavMeshAgent navMeshAgent;
    private bool estaVolando = false;           // Indica si Ignorancia está volando (en reposo = false = cuando no vuela)
    private float tiempoTranscurrido = 0f;      // Tiempo transcurrido para incrementar la velocidad


    void Start()
    {
        if (!TryGetComponent<NavMeshAgent>(out navMeshAgent))
        {
            Debug.LogError("NavMeshAgent no está asignado al Prefab.");
        }

        // Configurar el NavMeshAgent para que funcione en 2D
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.speed = velocidadInicial;

        // Verificar si el NavMeshAgent está en el NavMesh
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("No hay NavMesh");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanciaAlPlayer = Vector2.Distance(transform.position, player.position);

        if (distanciaAlPlayer < distanciaMinima)
        {
            estaVolando = true;
            animator.SetBool("Volando", true);
        }
        else if (distanciaAlPlayer > distanciaMaxima)
        {
            estaVolando = false;
            animator.SetBool("Volando", false);
        }

        if (estaVolando)
        {
            
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(player.position);
                IncrementaVelocidad();
            }
            else
            {
                Debug.LogWarning("Ignorancia no está en el NavMesh");
            }
        }
        else
        {
            navMeshAgent.ResetPath();
        }
    }

    private void IncrementaVelocidad()
    {
        // Incrementar la velocidad del NavMeshAgent a medida que pasa el tiempo
        tiempoTranscurrido += Time.deltaTime;
        navMeshAgent.speed = velocidadInicial + tiempoTranscurrido * aceleracion;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Reduce la vida del player
            AudioManager.audioManager.SetLoopSonidos(1, false);
            AudioManager.audioManager.PlaySonidos(1);
            GameManager.gameManager.DecrementaVidas();
            
            Destroy(gameObject);
        }
    }

}

