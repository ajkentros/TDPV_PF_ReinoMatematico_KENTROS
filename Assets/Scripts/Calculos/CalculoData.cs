using UnityEngine;

[CreateAssetMenu(fileName = "CalculoData", menuName = "ScriptableObjects/CalculoData", order = 1)]
public class CalculoData : ScriptableObject
{
    [Header ("Operandos")]
    public Sprite Operando_1;
    public Sprite Operando_2;
    public Sprite Operando_3;
    [Header("Opciones")]
    public Sprite Opcion_1;
    public Sprite Opcion_2;
    public Sprite Opcion_3;
    [Header("Tipo de Operación")]
    public string Operacion; // +, -, *, /
    public string Condicion; // =, >, <, >=
    [Header("Resultado")]
    public Sprite Resultado; // Imagen que representa el resultado
}
