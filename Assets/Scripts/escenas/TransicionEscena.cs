using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionEscena : MonoBehaviour
{
    public string scene;
    public Vector2 playerPosition;
    public VectorValue playerStorage;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !collision.isTrigger)
        {
            playerStorage.initialValue = playerPosition;
            SceneManager.LoadScene(scene);
        }
    }
}
