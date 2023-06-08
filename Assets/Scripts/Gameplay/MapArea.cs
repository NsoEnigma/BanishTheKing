using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemons;

    //Utilizado para coger un pokemon salvaje de manera aleatoria
    public Pokemon GetRamdomWildPokemon()
    {
        var wildPokemon = wildPokemons[Random.Range(0, wildPokemons.Count)];
        wildPokemon.Init();
        return wildPokemon;
    }
}
