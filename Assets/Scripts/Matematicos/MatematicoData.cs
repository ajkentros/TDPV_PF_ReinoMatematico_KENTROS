using UnityEngine;


[CreateAssetMenu(fileName = "MatematicoData", menuName = "ScriptableObjects/MatematicoData", order = 1)]
public class MatematicoData : ScriptableObject
{
    [Header("Apellido del Matem�tico")]
    public string apellidoMatematico;

    [Header("Descripci�n")]
    public string descripcionMatematico;
   
}
