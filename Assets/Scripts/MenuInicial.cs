using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    public void Jugar(){

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void Salir(){

        Debug.Log("Saliendo...");
        Application.Quit();

    }

    public void Reiniciar(){

        SceneManager.LoadScene("CuartoInicial");

    }
}
