using UnityEngine;


[CreateAssetMenu(fileName = "MatematicoData", menuName = "ScriptableObjects/MatematicoData", order = 1)]
public class MatematicoData : ScriptableObject
{
    [Header("Apellido del Matemático")]
    public string apellidoMatematico;

    [Header("Descripción")]
    public string descripcionMatematico;
   
}
