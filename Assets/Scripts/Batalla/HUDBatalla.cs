using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class HUDBatalla : MonoBehaviour
{
    [SerializeField] Text textoNombre;
    [SerializeField] Text textoNivel;
    [SerializeField] BarraVida barraVida;
    [SerializeField] GameObject expBar;

    Pokemon pPokemon;

    public void SetData(Pokemon pokemon)
    {
        pPokemon = pokemon;

        textoNombre.text = pokemon.Base.Name;
        SetLevel();
        barraVida.SetVida((float) pokemon.Vida / pokemon.MaxVida);
        SetExp();

    }

    public void SetLevel()
    {
        textoNivel.text = "Lvl " + pPokemon.Level;
    }


    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;

        if (reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);

        }

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currLevelExp = pPokemon.Base.GetExpForLevel(pPokemon.Level);
        int nextLevelExp = pPokemon.Base.GetExpForLevel(pPokemon.Level + 1);

        float normalizedExp = (float)(pPokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);

    }

    public IEnumerator UpdateVida()
    {
        yield return barraVida.SetVidaSmooth((float) pPokemon.Vida / pPokemon.MaxVida);
    }
}
