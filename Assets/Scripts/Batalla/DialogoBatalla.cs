using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogoBatalla : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color higlightedColor;

    [SerializeField] Text textoDialogo;
    [SerializeField] GameObject selectorAccion;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    public void SetDialogo(string dialogo)
    {
        textoDialogo.text = dialogo;
    }

    public IEnumerator TypeDialog(string dialogo)
    {
        textoDialogo.text = "";
        foreach (var letter in dialogo.ToCharArray())
        {
            textoDialogo.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        textoDialogo.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        selectorAccion.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int accionSeleccionada)
    {
        for (int i=0; i<actionTexts.Count; i++)
        {
            if (i == accionSeleccionada)
                actionTexts[i].color = higlightedColor;
            else  
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int movimientoSeleccionado, Movimiento move)
    {
        for (int i=0; i<moveTexts.Count; i++)
        {
            if (i == movimientoSeleccionado)
                moveTexts[i].color = higlightedColor;
            else  
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.TipoPokemon.ToString();
    }

    public void SetNombresMovimientos(List<Movimiento> movimientos)
    {
        for (int i=0; i<moveTexts.Count;i++)
        {
            if(i < movimientos.Count)
                moveTexts[i].text = movimientos[i].Base.Name;
            else
                moveTexts[i].text = "-";
        }
    }
}
