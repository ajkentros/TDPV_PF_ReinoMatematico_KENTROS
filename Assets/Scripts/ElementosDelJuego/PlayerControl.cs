using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    [SerializeField] private float velocidadPlayer = 1.5f;
    [SerializeField] private Animator animacionPlayer;

    private Rigidbody2D rigidbody2DPlayer;
    

    private bool movimientoHabilitado = true;
    private Vector2 entradaMovimiento;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2DPlayer = GetComponent<Rigidbody2D>();    
        
        if (!TryGetComponent<Animator>(out animacionPlayer))
        {
            Debug.LogError("No se encontró el componente Animator en el objeto " + gameObject.name);
        }
        else if (animacionPlayer.runtimeAnimatorController == null)
        {
            Debug.LogError("No se ha asignado un AnimatorController al componente Animator en el objeto " + gameObject.name);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!movimientoHabilitado)
        {
            entradaMovimiento = Vector2.zero;           // Detiene el movimiento si no está habilitado
           
            animacionPlayer.SetFloat("Velocidad", 0);   // Asegura que la animación también se detenga
            return;
        }
        
        float movimientoX = Input.GetAxisRaw("Horizontal");
        float movimientoY = Input.GetAxisRaw("Vertical");

        entradaMovimiento = new Vector2(movimientoX, movimientoY).normalized;

        animacionPlayer.SetFloat("Horizontal", movimientoX);
        animacionPlayer.SetFloat("Vertical", movimientoY);
        animacionPlayer.SetFloat("Velocidad", entradaMovimiento.sqrMagnitude);
        


    }

    private void FixedUpdate()
    {
        if (!movimientoHabilitado)
        {
            return;
        }

        rigidbody2DPlayer.MovePosition(Time.fixedDeltaTime * velocidadPlayer * entradaMovimiento + rigidbody2DPlayer.position );
        
        
    }

    public void DetenerAnimacion()
    {
        movimientoHabilitado = true;
        animacionPlayer.SetFloat("Velocidad", 0);
    }

    public void SetMovimientoPlayer(bool habilitado)
    {
        //Debug.Log("SetMovimientoPlayer =" + habilitado);
        movimientoHabilitado = habilitado;
        if (!movimientoHabilitado)
        {
            entradaMovimiento = Vector2.zero;           // Reinicia el vector de movimiento
            animacionPlayer.SetFloat("Velocidad", 0);
            
        }
    }

    
}
