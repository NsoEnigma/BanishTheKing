using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarraVida : MonoBehaviour
{
    [SerializeField] GameObject vida;

    public void SetVida(float hpNormal)
    {
        vida.transform.localScale = new Vector3(hpNormal, 1f);
    }

    public IEnumerator SetVidaSmooth(float nuevaVida)
    {
        float vidaActual = vida.transform.localScale.x;
        float changeCantidad = vidaActual - nuevaVida;

        while (vidaActual - nuevaVida > Mathf.Epsilon)
        {
            vidaActual -= changeCantidad * Time.deltaTime;
            vida.transform.localScale = new Vector3(vidaActual, 1f);
            yield return null;
        }
        vida.transform.localScale = new Vector3(nuevaVida, 1f);
    }
}
