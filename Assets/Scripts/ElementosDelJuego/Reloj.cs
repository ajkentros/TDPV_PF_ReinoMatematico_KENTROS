using UnityEngine;


public class Reloj : MonoBehaviour
{
    [SerializeField] private float tiempoInicial;

    private float tiempoTranscurrido;
    private float tiempoRestante;
    
    private bool estaCorriendoTemporizador;
    private bool estaCorriendoCronometro;

    public delegate void TiempoTerminado();
    public event TiempoTerminado OnTiempoTerminado;


    void Start()
    {
        IniciaTemporizador();
        IniciaCronometro();
    }

    private void IniciaTemporizador()
    {

        tiempoRestante = tiempoInicial;
        estaCorriendoTemporizador = false;

    }

    private void IniciaCronometro()
    {
        //tiempoTranscurrido = 0;
        estaCorriendoCronometro = false;
    }

    void Update()
    {
        if (estaCorriendoTemporizador)
        {

            if (tiempoRestante > 0)
            {
                tiempoRestante -= Time.deltaTime;
            }
            else
            {
                IniciaTemporizador();
                OnTiempoTerminado?.Invoke();
            }
        }
        

        if (estaCorriendoCronometro)
        {
            tiempoTranscurrido += Time.deltaTime;
        }
        
    }

    public void IniciarTemporizador(float tiempo)
    {
        tiempoInicial = tiempo;
        tiempoRestante = tiempoInicial;
        estaCorriendoTemporizador = true;
    }

    public void DetenerTemporizador()
    {
        estaCorriendoTemporizador = false;
    }

    public void IniciarCronometro()
    {
        //Debug.Log("IniciarCronometro()");
        if (!estaCorriendoCronometro)
        {
            //Debug.Log("IniciarCronometro()");
            //tiempoTranscurrido = 0; // Reiniciar tiempo transcurrido
            estaCorriendoCronometro = true;
        }
    }

    public void DetenerCronometro()
    {
        estaCorriendoCronometro = false;
    }

    public float ObtenerTiempoRestante()
    {
        return tiempoRestante;
    }

    public float ObtenerTiempoTranscurrido()
    {
        //Debug.Log(tiempoTranscurrido);
        return tiempoTranscurrido;
    }

    public bool EstaCorriendoTemporizador()
    {
        return estaCorriendoTemporizador;
    }

    public bool EstaCorriendoCronometro()
    {
        return estaCorriendoCronometro;
    }
}

