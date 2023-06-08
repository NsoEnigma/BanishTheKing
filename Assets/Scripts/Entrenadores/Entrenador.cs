using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrenador
{
    public EntrenadorBase Base { get; set; }
    public int LevelEnt { get; set; }

    public int Vida { get; set; }

    public List<MovimientoEntrenador> MovimientosEntrenador {get; set;}

    public Entrenador(EntrenadorBase pBase, int pLevel)
    {
        Base = pBase;
        LevelEnt = pLevel;
        Vida = MaxVida;

        //Esto sirve para generar los movimientos en base a su nivel, con un limite de 4 movimientos
        MovimientosEntrenador = new List<MovimientoEntrenador>();
        foreach (var movimientoEntrenador in Base.LearnableMovesEntrenador)
        {
            if (movimientoEntrenador.LevelEnt <= LevelEnt)
                MovimientosEntrenador.Add(new MovimientoEntrenador(movimientoEntrenador.Base));

            if (MovimientosEntrenador.Count >= 4)
                break;
        }
    }

    public int Ataque {
        get { return Mathf.FloorToInt((Base.Ataque * LevelEnt) / 100f) +5;}
    }

    public int Defensa {
        get { return Mathf.FloorToInt((Base.Defensa * LevelEnt) / 100f) +5;}
    }

    public int SpAtaque {
        get { return Mathf.FloorToInt((Base.SpAtaque * LevelEnt) / 100f) +5;}
    }
    
    public int SpDefensa {
        get { return Mathf.FloorToInt((Base.SpDefensa * LevelEnt) / 100f) +5;}
    }

    public int Velocidad {
        get { return Mathf.FloorToInt((Base.Velocidad * LevelEnt) / 100f) +5;}
    }

    public int MaxVida {
        get { return Mathf.FloorToInt((Base.MaxVida * LevelEnt) / 100f) +10;}
    }

    public bool RecibirDaño(MovimientoEntrenador movimiento, Entrenador atacante)
    {
        float modificadores = Random.Range(0.85f, 1f);
        float a = (2*atacante.LevelEnt + 10) / 250f;
        float d = a * movimiento.Base.Poder * ((float)atacante.Ataque / Defensa) + 2;
        int danio = Mathf.FloorToInt(d * modificadores);

        Vida -= danio;
        if (Vida <=0)
        {   
            Vida=0;
            return true;
        }

        return false;
    }

    public MovimientoEntrenador GetMovimientoRandom()
    {
        int r = Random.Range(0, MovimientosEntrenador.Count);
        return MovimientosEntrenador[r];
    }
}
