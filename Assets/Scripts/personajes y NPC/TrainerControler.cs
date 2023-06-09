using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerControler : MonoBehaviour
{   
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerScript player)
    {
        //Mostrar Exclamacion
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        //Andar hacia el entrenador
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        //Mostrar dialogo
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            GameControler.Instance.EmpezarBatallaEntrenador(this);
        }));

    }

    //Cambia la direccion del fov dependiendo de la direccion del personaje
    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Left)
            angle = 270f;
        else if (dir == FacingDirection.Up)
            angle = 180f;


        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public string Name {
        get => name;
    }

    public Sprite Sprite{
        get => sprite;
    }

}
