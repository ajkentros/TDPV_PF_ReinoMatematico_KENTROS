
using UnityEngine;


public class Ignorancia : MonoBehaviour
{
    public Transform player;
    public float followDistance = 10f;
    public float stopFollowDistance = 15f;
    public float speed = 2f;
    public float raycastDistance = 1f; // Distancia del raycast para detectar obstáculos
    public int angleStep = 10; // Step en grados para revisar los ángulos
    public LayerMask obstacleMask; // LayerMask para los obstáculos

    private bool isFollowing = false;
    private bool isSearchingDirection = false;
    private Rigidbody2D rb;
    private Vector2 currentDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentDirection = Vector2.zero;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < followDistance)
        {
            isFollowing = true;
        }
        else if (distanceToPlayer > stopFollowDistance)
        {
            isFollowing = false;
        }
    }

    void FixedUpdate()
    {
        if (isFollowing)
        {
            if (!isSearchingDirection)
            {
                FollowPlayer();
            }
            else
            {
                SearchNewDirection();
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleMask);
        Debug.DrawRay(transform.position, direction * raycastDistance, Color.yellow); // Para visualizar los rayos

        if (hit.collider == null)
        {
            // No hay obstáculos, sigue al jugador
            currentDirection = direction;
            rb.velocity = currentDirection * speed;
        }
        else
        {
            // Obstáculo detectado, busca una nueva dirección
            isSearchingDirection = true;
            rb.velocity = Vector2.zero;
        }
    }

    void SearchNewDirection()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        bool foundClearPath = false;

        for (int i = 0; i < 360; i += angleStep)
        {
            Vector2 newDirection = Quaternion.Euler(0, 0, i) * direction;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, newDirection, raycastDistance, obstacleMask);
            Debug.DrawRay(transform.position, newDirection * raycastDistance, Color.red); // Para visualizar los rayos

            if (hit.collider == null)
            {
                // Nueva dirección libre encontrada
                currentDirection = newDirection;
                rb.velocity = currentDirection * speed;
                foundClearPath = true;
                break;
            }
        }

        if (!foundClearPath)
        {
            // Si no encuentra una dirección libre, se detiene
            rb.velocity = Vector2.zero;
        }
        else
        {
            // Una vez que encuentra una dirección libre, verifica si puede ver al jugador
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleMask);
            Debug.DrawRay(transform.position, direction * raycastDistance, Color.blue); // Para visualizar los rayos
            if (hit.collider == null)
            {
                // Si puede ver al jugador, vuelve a seguirlo
                isSearchingDirection = false;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}


