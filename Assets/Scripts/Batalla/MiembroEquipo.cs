using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Funcion para mostrar los datos de un miembro del equipo en la pantalla de pokemon

public class MiembroEquipo : MonoBehaviour
{
    [SerializeField] Text textoNombre;
    [SerializeField] Text textoNivel;
    [SerializeField] BarraVida barraVida;

    [SerializeField] Color highlightedColor;

    Pokemon pPokemon;

    public void SetData(Pokemon pokemon)
    {
        pPokemon = pokemon;

        textoNombre.text = pokemon.Base.Name;
        textoNivel.text = "LvL " + pokemon.Level;
        barraVida.SetVida((float) pokemon.Vida / pokemon.MaxVida);

    }

    public void SetSelected(bool selected)
    {
        if (selected)
            textoNombre.color = highlightedColor;
        else
            textoNombre.color = Color.black;
    }
}
