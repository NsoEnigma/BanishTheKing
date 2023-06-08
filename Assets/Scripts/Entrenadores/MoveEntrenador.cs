using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Entrenador", menuName = "Entrenador/ Crear movimiento")]

public class MoveEntrenador : ScriptableObject
{
    [SerializeField] string name;

    [TextArea] 
    [SerializeField] string description;

    [SerializeField] TipoEntrenador tipo;
    [SerializeField] int poder;
    [SerializeField] int precision;
    [SerializeField] int pp;

    public string Name
    {
        get {return name;}
    }

    public string Descripcion
    {
        get {return description;}
    }

    public TipoEntrenador TipoEntrenador
    {
        get {return tipo;}
    }

    public int Poder
    {
        get {return poder;}
    }

    public int Precision
    {
        get {return precision;}
    }

    public int PP
    {
        get {return pp;}
    }
}
