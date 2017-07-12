using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CardArrowDrop : MonoBehaviour, IDropHandler {

    Serverino server;

    public void getControlInstance() {
        server = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>();
    }

    private void Start() {
        getControlInstance();
    }

    public void OnDrop(PointerEventData eventData) {
        CardArrowDrag card_drag = eventData.pointerDrag.GetComponent<CardArrowDrag>();
        
        if (card_drag != null && server.control.isPlayerTurn()) {
            Control control = server.control;
            int opponent_id = control.getOpponentId();
            BattleControl battleControl = control.getBattleControl();

            Card attacker = card_drag.transform.parent.GetComponent<CardInstance>().card;
            if (gameObject.tag == "enemy") {
                if (server.tryAttackPlayer(attacker.getID(), attacker.getPosition())) {
                    battleControl.directAttack(attacker, opponent_id);
                }
            }
            else {
                Card target = transform.GetComponent<CardInstance>().card;
                if (target == null)
                    return;
                Position position = target.getPosition();
                Position position2 = attacker.getPosition();
                bool can_attack_target = server.tryAttackCharacter(attacker.getID(), target.getID(), attacker.getPosition(), target.getPosition());
                if (can_attack_target) {
                    battleControl.cardAttackCard(attacker, target);
                }
            }
        }
    }

}
