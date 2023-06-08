using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[System.Serializable]

public class Pokemon
{   
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        Init();
    }

    public PokemonBase Base { 
        get {
            return _base;
        }
    }

    public int Level {
        get{
            return level;
        }
    }

    public int Exp { get; set; }

    public int Vida { get; set; }

    public List<Movimiento> Movimientos {get; set;}
    public Dictionary<Stat, int> Stats { get; private set;}
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();


    public void Init()
    {
        //Esto sirve para generar los movimientos en base a su nivel, con un limite de 4 movimientos
        Movimientos = new List<Movimiento>();
        foreach (var movimiento in Base.LearnableMoves)
        {
            if (movimiento.Level <= Level)
                Movimientos.Add(new Movimiento(movimiento.Base));

            if (Movimientos.Count >= 4)
                break;
        }

        Exp = Base.GetExpForLevel(Level);

        CalculateStats();

        Vida = MaxVida;

        ResetStatBoost();

    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Ataque * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defensa * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAtaque * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefensa * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Velocidad * Level) / 100f) + 5);

        MaxVida = Mathf.FloorToInt((Base.MaxVida * Level) / 100f) + 10;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0}
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //Apply stat boodt
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0) 
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else
        {
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        }

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost ,-6, 6);

            if (boost > 0)
            {
                StatusChanges.Enqueue($"El {stat} de {Base.Name} aumentó!");
            }
            else
            {
                StatusChanges.Enqueue($"El {stat} de {Base.Name} bajó!");
            }

        }
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }

        return false;
    }

    public int Ataque {
        get { return GetStat(Stat.Attack); }
    }

    public int Defensa {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAtaque {
        get { return GetStat(Stat.SpAttack); }
    }
    
    public int SpDefensa {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Velocidad {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxVida { get; private set; }

    public DamageDetails RecibirDaño(Movimiento movimiento, Pokemon atacante)
    {
        float critical = 1f;
        if (Random.value * 100f < 6.25f)
        {
            critical = 2f;
        }
        float type = TypeChart.GetEffectiveness(movimiento.Base.TipoPokemon, this.Base.Type1) * TypeChart.GetEffectiveness(movimiento.Base.TipoPokemon, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (movimiento.Base.Category == MoveCategory.Special) ? atacante.SpAtaque : atacante.Ataque;
        float defense = (movimiento.Base.Category == MoveCategory.Special) ? SpDefensa : Defensa;

        float modificadores = Random.Range(0.85f, 1f) * type * critical;
        float a = (2*atacante.Level + 10) / 250f;
        float d = a * movimiento.Base.Poder * ((float)attack / defense) + 2;
        int danio = Mathf.FloorToInt(d * modificadores);

        Vida -= danio;
        if (Vida <=0)
        {   
            Vida=0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Movimiento GetMovimientoRandom()
    {
        int r = Random.Range(0, Movimientos.Count);
        return Movimientos[r];
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}
