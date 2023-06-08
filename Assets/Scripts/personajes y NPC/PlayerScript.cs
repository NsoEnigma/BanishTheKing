using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    
    private Vector2 input;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

    private Character character;

    public VectorValue startingPosition;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        transform.position = startingPosition.initialValue;
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");


            //Eliminar movimiento diagonal
            if(input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                StartCoroutine (character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if(Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        var caraDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + caraDir;

        //Debug.DrawLine(transform.position, interactPos, Color.green, 1f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);

        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        VerificacionEnfrentamientos();
        CheckIfInTrainersView();
    }

    private void VerificacionEnfrentamientos()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1,101)<=10)
            {
                character.Animator.IsMoving = false;
                OnEncountered();
            }
        }
    }

    private void CheckIfInTrainersView()
    {   
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);
        if(collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(collider);
        }
    }

    public string Name {
        get => name;
    }

    public Sprite Sprite{
        get => sprite;
    }


}