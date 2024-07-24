using UnityEngine;

[CreateAssetMenu(fileName = "CrucigramaData", menuName = "ScriptableObjects/CrucigramaData", order = 1)]
public class CrucigramaData : ScriptableObject
{
    [Header ("Fila 1")]
    public Sprite Operando_11;
    public Sprite Operando_12;
    public Sprite Operando_13;

    [Header("Fila 2")]
    public Sprite Operando_21;
    public Sprite Operando_22;
    public Sprite Operando_23;

    [Header("Fila 3")]
    public Sprite Operando_31;
    public Sprite Operando_32;
    public Sprite Operando_33;

    [Header("Resultado")]
    public string Resultado_1;
    public string Resultado_2;
    public string Resultado_3;
}
