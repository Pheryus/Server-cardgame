using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Classe responsável pelo ataque a personagens inimigos. Todas personagens que o oponente jogar ganham o componente CardArrowDrop, possibilitando que
/// sejam alvejados pelos personagens do jogador.
/// </summary>
public class CardArrowDrop : MonoBehaviour, IDropHandler {

    Serverino server;

    public void getControlInstance() {
        server = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>();
    }

    private void Start() {
        this.getControlInstance();
    }


    private void attack(Card attacker) {
        Control control = server.control;
        BattleControl battleControl = control.getBattleControl();
        int opponent_id = control.getOpponentId();

        if (gameObject.tag == "enemy") {

            if (battleControl.canAttackPlayer(attacker) && 
                    server.tryAttackPlayer(attacker.getID(), attacker.getPosition())) {
                battleControl.directAttack(attacker, opponent_id);
            }
        }
        else {
            Card target = transform.GetComponent<CardGOInstance>().card;
            if (target == null)
                return;

            if (battleControl.canAttackCharacter(attacker, target) == false)
                return;

            attacker.setCanAttack(false);

            bool can_attack_target = server.tryAttackCharacter(attacker.getID(), target.getID(), attacker.getPosition(), target.getPosition());
            if (can_attack_target) {
                battleControl.cardAttackCard(attacker, target);
            }
            else
                attacker.setCanAttack(true);
        }
    }

    public void OnDrop(PointerEventData eventData) {
        CardArrowDrag dragged_card = eventData.pointerDrag.GetComponent<CardArrowDrag>();
        
        if (dragged_card != null && server.control.isPlayerTurn()) {
            
            Card card = dragged_card.transform.parent.GetComponent<CardGOInstance>().card;

            if (card.isCreature()) {
                this.attack(card);
            }
        }

    }

}
