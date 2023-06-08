using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Dialog, Batalla, Cutscene }
public class GameControler : MonoBehaviour
{
    [SerializeField] PlayerScript playerController;
    [SerializeField] SistemaBatalla battleSystem;
    [SerializeField] Camera camaraMundo;
    
    GameState state;

    public static GameControler Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {   
        

        playerController.OnEnterTrainersView += (Collider2D trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerControler>();
            if (trainer != null)
            {   
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
        };

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if(state == GameState.Dialog)
            {
                state = GameState.FreeRoam;
            }
        };

        playerController.OnEncountered += EmpezarBatalla;
        battleSystem.cuandoBatallaAcaba += FinalizarBatalla;

        
    }

    void EmpezarBatalla()
    {
        state = GameState.Batalla;
        battleSystem.gameObject.SetActive(true);
        camaraMundo.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRamdomWildPokemon();

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        battleSystem.EmpezarBatalla(playerParty, wildPokemonCopy);
    }

    public void EmpezarBatallaEntrenador(TrainerControler trainer)
    {
        state = GameState.Batalla;
        battleSystem.gameObject.SetActive(true);
        camaraMundo.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>();


        battleSystem.EmpezarBatallaEntrenador(playerParty, trainerParty);
    }

    void FinalizarBatalla(bool ganar)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        camaraMundo.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }else if(state == GameState.Batalla)
        {
            battleSystem.HandleUpdate();
        }else if(state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }
}
