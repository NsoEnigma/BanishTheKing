using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entrenador", menuName = "Entrenador/ Crear entrenador")]

public class EntrenadorBase : ScriptableObject
{
    [SerializeField] string name;
    
    [TextArea]
    [SerializeField] string descripcion;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    //Tipos de pokemon(como maximo 2)
    [SerializeField] TipoEntrenador tipo1;

    //Estas son las estadisticas base de nuestros pokemons
    [SerializeField] int maxVida;
    [SerializeField] int ataque;
    [SerializeField] int defensa;
    [SerializeField] int spAtaque;
    [SerializeField] int spDefensa;
    [SerializeField] int velocidad;

    [SerializeField] List<LearnableMoveEntrenador> learnableMovesEntrenador;

    public string Name
    {
        get { return name; }
    }

    public string Descripcion
    {
        get { return descripcion; }
    }

    public int MaxVida
    {
        get { return maxVida; }
    }

    public int Ataque
    {
        get { return ataque; }
    }

    public int Defensa
    {
        get { return defensa; }
    }

    public int SpAtaque
    {
        get { return spAtaque; }
    }

    public int SpDefensa
    {
        get { return spDefensa; }
    }

    public int Velocidad
    {
        get { return velocidad; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public List<LearnableMoveEntrenador> LearnableMovesEntrenador{
        get { return learnableMovesEntrenador;}
    }




}

[System.Serializable]
public class LearnableMoveEntrenador
{
    [SerializeField] MoveEntrenador moveEntrenador;
    [SerializeField] int levelEnt;

    public MoveEntrenador Base {
        get { return moveEntrenador;}
    }

    public int LevelEnt {
        get { return levelEnt;}
    }
}



//Lista de tipos de pokemon existentes
public enum TipoEntrenador
{
    Humano,
    Friki,
    Otaku,
    Fifa,
    Maestro,
    Tanque

}
