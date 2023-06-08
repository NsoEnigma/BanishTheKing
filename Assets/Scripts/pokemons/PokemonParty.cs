using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons {
        get {
            return pokemons;
        }
    }

    private void Start()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(x => x.Vida > 0).FirstOrDefault();
    }

    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemons.Count < 6)
        {
            pokemons.Add(newPokemon);
        }
        else
        {
            //El pokemon se eliminara, cuando avance el juego de modo personal se guardara en un almacen
        }
    }
}
