using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grass;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;


    public static GameLayers i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidLayer{
        get => solidObjectLayer;
    }

    public LayerMask InteractableLayer{
        get => interactableLayer;
    }

    public LayerMask GrassLayer{
        get => grass;
    }

    public LayerMask PlayerLayer{
        get => playerLayer;
    }

    public LayerMask FovLayer{
        get => fovLayer;
    }
}
