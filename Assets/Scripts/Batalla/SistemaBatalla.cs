using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver}

public class SistemaBatalla : MonoBehaviour
{
    [SerializeField] UnidadBatalla unidadJugador;
    [SerializeField] UnidadBatalla unidadEnemigo;
    [SerializeField] DialogoBatalla dialogBox;
    [SerializeField] PantallaEquipo pantallaEquipo;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject chanclaSprite;

    public event Action<bool> cuandoBatallaAcaba;

    BattleState state;
    int accionActual;
    int movimientoActual;
    int currentMember;

    PokemonParty playerParty;
    PokemonParty trainerParty;
    Pokemon wildPokemon;

    bool isTrainerBattle = false;
    PlayerScript player;
    TrainerControler trainer;

    int escapeAttemps;

    public void EmpezarBatalla(PokemonParty playerParty, Pokemon wildPokemon)
    {
        isTrainerBattle = false;
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerScript>();

        StartCoroutine(SetupBatalla());
    }

    public void EmpezarBatallaEntrenador(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerScript>();
        trainer = trainerParty.GetComponent<TrainerControler>();

        StartCoroutine(SetupBatalla());
    }


    //Funcion que inicializa la batalla 
    public IEnumerator SetupBatalla()
    {   

        unidadJugador.Clear();
        unidadEnemigo.Clear();

        if(!isTrainerBattle)
        {
            // Pokemon salvaje
            unidadJugador.Setup(playerParty.GetHealthyPokemon());
            unidadEnemigo.Setup(wildPokemon);

            dialogBox.SetNombresMovimientos(unidadJugador.Pokemon.Movimientos);

            yield return dialogBox.TypeDialog($"¡Un {unidadEnemigo.Pokemon.Base.Name} salvaje apareció!");
            yield return new WaitForSeconds(1f);
            ChooseFirstTurn();
        }
        else
        {
            // combate entrenador

            //Imagen mostrar entrenadores

            unidadJugador.gameObject.SetActive(false);
            unidadEnemigo.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"¡ {trainer.Name} te reta a un combate!");
            yield return new WaitForSeconds(1f);


            //Mandar primer pokemon de nuestro rival
            trainerImage.gameObject.SetActive(false);
            unidadEnemigo.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            unidadEnemigo.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"¡ {trainer.Name} manda a {enemyPokemon.Base.Name}!");
            yield return new WaitForSeconds(1f);


            //Mandar primer pokemon nuestro
            playerImage.gameObject.SetActive(false);
            unidadJugador.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            unidadJugador.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"¡ Adelante {playerPokemon.Base.Name}!");
            dialogBox.SetNombresMovimientos(unidadJugador.Pokemon.Movimientos);
            yield return new WaitForSeconds(1f);

            ChooseFirstTurn();


        }

        
        escapeAttemps = 0;
        pantallaEquipo.Init();
        ActionSelection();
    }

    void ChooseFirstTurn()
    {
        if (unidadJugador.Pokemon.Velocidad >= unidadEnemigo.Pokemon.Velocidad)
        {
            ActionSelection();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
            
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        cuandoBatallaAcaba(won);
    }

    //Funcion para el inicio de eleccion
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialogo("Elige una accion");
        dialogBox.EnableActionSelector(true);
    }

    //Funcion que me imprime la lista de pokemons 
    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        pantallaEquipo.SetPartyData(playerParty.Pokemons);
        pantallaEquipo.gameObject.SetActive(true);
    }

    //Funcion para el movimiento del jugador se inicie o pare
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
        
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = unidadJugador.Pokemon.Movimientos[movimientoActual];
        
        yield return RunMove(unidadJugador,unidadEnemigo,move);


        
        if(state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
        
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = unidadEnemigo.Pokemon.GetMovimientoRandom();
        yield return RunMove(unidadEnemigo,unidadJugador,move);



        if (state == BattleState.PerformMove)
            ActionSelection();
        
        
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("¡Un golpe crítico!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("¡Es super efectivo!");

        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("No es muy efectivo...");
        
    }

    IEnumerator RunMove(UnidadBatalla sourceUnit, UnidadBatalla targetUnit, Movimiento move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} usó {move.Base.Name}");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();


        if (move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit.Pokemon, targetUnit.Pokemon);
        }
        else
        {
            var damageDetails = targetUnit.Pokemon.RecibirDaño(move, sourceUnit.Pokemon);
            yield return targetUnit.Hud.UpdateVida();
            yield return ShowDamageDetails(damageDetails);
        }


        if (targetUnit.Pokemon.Vida <= 0) 
        {
            yield return HadlePokemonFainted(targetUnit);
        }
    }

    IEnumerator RunMoveEffects(Movimiento move, Pokemon source, Pokemon target)
    {
        var effects = move.Base.Effects;
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }

        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HadlePokemonFainted(UnidadBatalla faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} se he debilitado");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.EsUnidadJugador)
        {
            // Gana Exp
            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            //Formula cogida de internet
            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            unidadJugador.Pokemon.Exp += expGain;
            yield return dialogBox.TypeDialog($"{unidadJugador.Pokemon.Base.Name} consiguió {expGain} pipas!");
            yield return unidadJugador.Hud.SetExpSmooth();


            // Revisar level up
            while (unidadJugador.Pokemon.CheckForLevelUp())
            {
                unidadJugador.Hud.SetLevel();

                yield return dialogBox.TypeDialog($"{unidadJugador.Pokemon.Base.Name} ahora tiene {unidadJugador.Pokemon.Level} bolsas de pipas !");

                yield return unidadJugador.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }
        else
        {
            // No gana Exp
        }

        CheckForBattleOver(faintedUnit);
    }

    void CheckForBattleOver(UnidadBatalla faintedUnit)
    {
        if (faintedUnit.EsUnidadJugador)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
                SceneManager.LoadScene("MenuPerdido");
            }
        }
        else
            if(!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                if(trainer.Name == "Igor")
                {
                    var nextPokemon = trainerParty.GetHealthyPokemon();
                    if (nextPokemon != null)
                    {
                        StartCoroutine(SendNextTrainerPokemon(nextPokemon));
                    }
                    else
                    {
                        BattleOver(true);
                        SceneManager.LoadScene("MenuGanado");
                    }
                

                }
                else
                {
                    var nextPokemon = trainerParty.GetHealthyPokemon();
                    if (nextPokemon != null)
                    {
                        StartCoroutine(SendNextTrainerPokemon(nextPokemon));
                    }
                    else
                    {
                        BattleOver(true);
                    }
                }
                
            }

    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            
        }
        
    }


    //Funcion para elegir una opcion en el menu de ataque con la letra E
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++accionActual;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --accionActual;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            accionActual += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            accionActual -= 2;


        accionActual = Mathf.Clamp(accionActual, 0, 3);

        dialogBox.UpdateActionSelection(accionActual);

        // Seleccion de tecla con la letra E
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(accionActual == 0)
            {
                //Luchar
                MoveSelection();
            }
            else if (accionActual == 1)
            {
                //Capturar bicho
                StartCoroutine(ThrowChancla());
            }
            else if (accionActual == 2)
            {
                //Pokemon cambiar
                OpenPartyScreen();
            }
            else if (accionActual == 3)
            {
                //Huir
                StartCoroutine(TryToEscape());
            }
        }

    }
    

    //Funcion para moverte entre las opciones del menu de batalla con las flchas del teclado
    void HandleMoveSelection() 
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++movimientoActual;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --movimientoActual;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            movimientoActual += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            movimientoActual -= 2;

        movimientoActual = Mathf.Clamp(movimientoActual, 0, unidadJugador.Pokemon.Movimientos.Count - 1);

        dialogBox.UpdateMoveSelection(movimientoActual, unidadJugador.Pokemon.Movimientos[movimientoActual]);

        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X)) //Funcion para volver atras en el menu con la tecla X
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        pantallaEquipo.UpdateMemberSelection(currentMember);

        if(Input.GetKeyDown(KeyCode.E))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.Vida <=0)
            {
                pantallaEquipo.SetMessageText("Tu pokemon esta debilitado... ¡Dejalo descansar!");
                return;
            }
            if (selectedMember == unidadJugador.Pokemon)
            {
                pantallaEquipo.SetMessageText("No puedes elegir el pokemon ¡Ya esta en el combate!");
                return;
            }

            pantallaEquipo.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            pantallaEquipo.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool currentPokemonFainted = true;
        if(unidadJugador.Pokemon.Vida > 0){

            currentPokemonFainted=false;
            yield return dialogBox.TypeDialog($"Es hora de volver {unidadJugador.Pokemon.Base.Name}");
            yield return new WaitForSeconds(1f);
        }

        unidadJugador.Setup(newPokemon);
        dialogBox.SetNombresMovimientos(newPokemon.Movimientos);
        yield return dialogBox.TypeDialog($"¡Vamos {newPokemon.Base.Name} acaba esto!");
        yield return new WaitForSeconds(1f);

        if (currentPokemonFainted)
        {
            ChooseFirstTurn();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }


    }

    IEnumerator SendNextTrainerPokemon(Pokemon nextPokemon)
    {
        state = BattleState.Busy;

        unidadEnemigo.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"¡{trainer.Name} mando al combate a {nextPokemon.Base.Name}!");
        yield return new WaitForSeconds(1f);

        ActionSelection();
    }

    IEnumerator ThrowChancla()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"{player.Name} no estamos en Madrid centro, no robes!");
            state = BattleState.ActionSelection;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} usó chanclazo!");

        var chanclaObj = Instantiate(chanclaSprite, unidadJugador.transform.position - new Vector3(2, 0), Quaternion.identity);
        var chancla = chanclaObj.GetComponent<SpriteRenderer>();


        //Animaciones

        yield return chancla.transform.DOJump(unidadEnemigo.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return unidadEnemigo.PlayCaptureAnimation();
        yield return chancla.transform.DOMoveY(unidadEnemigo.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(unidadEnemigo.Pokemon);

        //Hacer que la chancla se agite
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return chancla.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            //Bicho capturado
            yield return dialogBox.TypeDialog($"{unidadEnemigo.Pokemon.Base.Name} no pudo esquivar tu chanclazo y se unio a tu equipo!");
            yield return chancla.DOFade(0, 1.5f).WaitForCompletion();

            yield return new WaitForSeconds(1f);

            playerParty.AddPokemon(unidadEnemigo.Pokemon);
            yield return dialogBox.TypeDialog($"{unidadEnemigo.Pokemon.Base.Name} se unió a tu equipo!");

            Destroy( chancla );
            BattleOver(true );
        }
        else
        {
            //Bicho se escapa de la chancla
            yield return new WaitForSeconds(1f);
            chancla.DOFade(0, 0.2f);
            yield return unidadEnemigo.PlayBreakOutAnimation();

            if (shakeCount < 2)
            {
                yield return dialogBox.TypeDialog($"{unidadEnemigo.Pokemon.Base.Name} esquivo tu chanclazo!");
            }
            else
            {
                yield return dialogBox.TypeDialog($"{unidadEnemigo.Pokemon.Base.Name} esquivo tu chanclazo, pero esta mas desorientado!");
            }

            Destroy(chancla );
            yield return new WaitForSeconds(1f);
            StartCoroutine(EnemyMove());


        }
    }

    int TryToCatchPokemon(Pokemon pokemon)
    {
        //Formula pillada de internet
        float a = (3 * pokemon.MaxVida - 2 * pokemon.Vida) * pokemon.Base.CatchRate / (3 * pokemon.MaxVida);

        if (a >= 255)
        {
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }
            ++shakeCount;
        }

        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if(isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"No puede huir de entrenadores!");
            yield return new WaitForSeconds(1f);
            ActionSelection();
            yield break;
        }

        ++escapeAttemps;

        //Utilizo el algoritmo normal de pokemon para la huida
        int playerSpeed = unidadJugador.Pokemon.Velocidad;
        int enemySpeed = unidadEnemigo.Pokemon.Velocidad;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Escapa sin problemas");
            yield return new WaitForSeconds(1f);
            BattleOver(true);
        }
        else
        {
            //Formula sacada de internet
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttemps;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Escapa sin problemas");
                yield return new WaitForSeconds(1f);
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"No pudiste escapar!");
                yield return new WaitForSeconds(1f);
                StartCoroutine(EnemyMove());
            }
        }

    }
}
