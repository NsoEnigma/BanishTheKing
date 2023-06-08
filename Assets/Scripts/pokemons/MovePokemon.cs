using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/ Crear movimiento")]

public class MovePokemon : ScriptableObject
{
    [SerializeField] string name;

    [TextArea] 
    [SerializeField] string description;

    [SerializeField] TipoPokemon tipo;
    [SerializeField] int poder;
    [SerializeField] int precision;
    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

    public string Name
    {
        get {return name;}
    }

    public string Descripcion
    {
        get {return description;}
    }

    public TipoPokemon TipoPokemon
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

    public MoveCategory Category
    {
        get { return category;}
    }

    public MoveEffects Effects
    {
        get { return effects;}
    }

    public MoveTarget Target
    {
        get { return target;}
    }
 

}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts { 
        get { return boosts;} 
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}
