

using UnityEngine;



public class Ignorancia : MonoBehaviour
{
    [SerializeField] private float radioBusqueda; // Ahora será el ancho del rectángulo
    [SerializeField] private float alturaBusqueda; // Nueva variable para la altura del rectángulo
    [SerializeField] private LayerMask capaPlayer;
    [SerializeField] private LayerMask capaObstaculo; // Nueva capa para detectar obstáculos
    [SerializeField] private Transform transformPlayer;
    [SerializeField] private float velocidadIgnorancia;
    [SerializeField] private float distanciaMaxima;

    private Vector3 puntoInicialIgnorancia;
    private bool miraIgnooranciaDerecha;
    [SerializeField] private Rigidbody2D rigidBody2DIgnorancia;
    [SerializeField] private Animator animatorIgnorancia;


    public EstadosDeIgnorancia estadoActual;

    public enum EstadosDeIgnorancia
    {
        Espera,
        SiguieAlPlayer,
        Frena,
    }

    private void Start()
    {
        puntoInicialIgnorancia = transform.position;
    }
    private void Update()
    {

        switch (estadoActual)
        {
            case EstadosDeIgnorancia.Espera:
                EstadoEspera();
                break;

            case EstadosDeIgnorancia.SiguieAlPlayer:
                EstadoSiguieAlPlayer();
                break;

            case EstadosDeIgnorancia.Frena:
                EstadoFrena();
                break;

        }

    }

    private void EstadoEspera()
    {
        Collider2D playerCollider = Physics2D.OverlapBox(transform.position, new Vector2(radioBusqueda, alturaBusqueda), 0f, capaPlayer);

        if (playerCollider)
        {
            transformPlayer = playerCollider.transform;

            estadoActual = EstadosDeIgnorancia.SiguieAlPlayer;
        }
    }

    private void EstadoSiguieAlPlayer()
    {
        animatorIgnorancia.SetBool("Volando", true);

        if (transformPlayer == null)
        {
            estadoActual = EstadosDeIgnorancia.Frena;
            return;
        }

        // Detectar obstáculos
        if (Physics2D.OverlapBox(transform.position, new Vector2(radioBusqueda, alturaBusqueda), 0f, capaObstaculo))
        {
            estadoActual = EstadosDeIgnorancia.Frena;
            transformPlayer = null;
            return;
        }

        if (transform.position.x < transformPlayer.position.x)
        {
            rigidBody2DIgnorancia.velocity = new Vector2(velocidadIgnorancia, rigidBody2DIgnorancia.velocity.y);
        }
        else
        {
            rigidBody2DIgnorancia.velocity = new Vector2(-velocidadIgnorancia, rigidBody2DIgnorancia.velocity.y);
        }

        GiraIgnorancia(transform.position);

        if (Vector2.Distance(transform.position, puntoInicialIgnorancia) > distanciaMaxima || Vector2.Distance(transform.position, transformPlayer.position) > distanciaMaxima)
        {
            estadoActual = EstadosDeIgnorancia.Frena;
            transformPlayer = null;
        }
    }

    private void EstadoFrena()
    {
        if (transformPlayer == null)
        {
            estadoActual = EstadosDeIgnorancia.Frena;
            return;
        }

        if (transform.position.x < puntoInicialIgnorancia.x)
        {
            rigidBody2DIgnorancia.velocity = new Vector2(velocidadIgnorancia, rigidBody2DIgnorancia.velocity.y);
        }
        else
        {
            rigidBody2DIgnorancia.velocity = new Vector2(-velocidadIgnorancia, rigidBody2DIgnorancia.velocity.y);
        }

        GiraIgnorancia(puntoInicialIgnorancia);

        if (Vector2.Distance(transform.position, puntoInicialIgnorancia) < 0.1f)
        {
            rigidBody2DIgnorancia.velocity = Vector2.zero;

            animatorIgnorancia.SetBool("Volando", false);

            estadoActual = EstadosDeIgnorancia.Espera;
        }
    }

    private void GiraIgnorancia(Vector3 ignorancia)
    {
        if (ignorancia.x > transform.position.x && !miraIgnooranciaDerecha)
        {
            Gira();
        }
        else if (ignorancia.x < transform.position.x && miraIgnooranciaDerecha)
        {
            Gira();
        }
    }

    private void Gira()
    {
        miraIgnooranciaDerecha = !miraIgnooranciaDerecha;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector2(radioBusqueda, alturaBusqueda)); // Cambiado para un rectángulo
        Gizmos.DrawWireCube(puntoInicialIgnorancia, new Vector2(distanciaMaxima, alturaBusqueda));
    }

}


