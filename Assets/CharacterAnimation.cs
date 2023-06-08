using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;

    // parametros
    public float MoveX { get; set;}

    public float MoveY { get; set;}

    public bool isMoving { get; set;}


    // estados
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;

    //referencias
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        currentAnim = walkDownAnim;

    }

    
    public void Update()
    {   
        var prevAnim = currentAnim;

        if (MoveX == 1)
            currentAnim = walkRightAnim;
        else if (MoveX == -1)
            currentAnim = walkLeftAnim;
        else if (MoveY == 1)
            currentAnim = walkUpAnim;
        else if (MoveY == -1)
            currentAnim = walkDownAnim;
        

        if(currentAnim !=prevAnim)
            currentAnim.Start();


        if(isMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];
    }

}
