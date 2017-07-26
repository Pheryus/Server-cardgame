using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler {

    private bool empty = true;

    Serverino server = null;

    public int x;
    public int y;

    const string attackGO_name = "attack";
    const string lifeGO_name = "life";

    public void getControlInstance() {
        server = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>();
    }

    private Position getCardPosition(int player_side) {
        return new Position(y, x, player_side);
    }

    public void OnDrop(PointerEventData eventData) {
        
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();

        if (d != null && empty && d.canBeMoved && server.control.isPlayerTurn()) {
            this.playCreature(eventData, d);
        }

        CardArrowDrag dragged_arrow = eventData.pointerDrag.GetComponent<CardArrowDrag>();

        if (dragged_arrow != null) {
            this.moveCreature(eventData, dragged_arrow);
        }
    }

    public int getPlayerId() {
        return server.control.getPlayerId();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="dragged_arrow"></param>
    public void moveCreature (PointerEventData eventData, CardArrowDrag dragged_arrow) {
        Card card = dragged_arrow.transform.parent.GetComponent<CardGOInstance>().card;
        if (card != null && card.isCreature() && card.canAct()) {
            Position field_position = getCardPosition(this.getPlayerId());
            Position card_position = card.getPosition();
            Card other_card = null;

            if (this.empty == false) { 
                other_card = transform.GetChild(0).GetComponent<CardGOInstance>().card;
                if (other_card.canAct() == false)
                    return;
            }

            if (this.movementIsValid(field_position, card_position) == false)
                return;

            if (server.tryMoveCharacter(card, field_position)) { 
                empty = false;
                card.setPosition(field_position);
                GameObject card_go = dragged_arrow.transform.parent.gameObject;

                Transform old_position_go = card_go.transform.parent;

                DropZone old_position_dropzone = card_go.transform.parent.GetComponent<DropZone>();

                if (other_card != null) {
                    other_card.setPosition(card_position);
                    other_card.setCanAct(false);
                    transform.GetChild(0).SetParent(old_position_go);
                }
                else
                    old_position_dropzone.empty = true;

                card.setCanAct(false);
                
                card_go.transform.SetParent(transform);

            }
        }
    }

    /// <summary>
    /// Checa se o personagem tenta se mover para uma posição adjacente a que se encontra.
    /// </summary>
    /// <param name="field_position">Posição que ele se move</param>
    /// <param name="card_position">Posição que a carta se encontra</param>
    /// <returns>true se a carta se moverá para uma posição adjacente</returns>
    public bool movementIsValid(Position field_position, Position card_position) {
        int y = Mathf.Abs(field_position.line - card_position.line);
        int x = Mathf.Abs(field_position.column - card_position.column);

        return (x + y <= 2 && x <= 1 && y <= 1) && field_position != card_position;
    }

    public void setEmpty(bool empty) {
        this.empty = empty;
    }

    public void playCreature(PointerEventData eventData, Draggable d) {
        CardGOInstance cardInstance = eventData.pointerDrag.GetComponent<CardGOInstance>();
        Card card = cardInstance.card;

        int player_side = this.getPlayerId();
        Position field_position = getCardPosition(player_side);

        //se o servidor deixar ele jogar a cartita;
        if (card.isCreature() && server.tryPlayCharacter(card, field_position, d.hand_index)) {
            d.parentToReturnTo = this.transform;
            d.cardIs = "played";
            empty = false;

            cardInstance.setFieldPosition(transform.name, field_position);
            server.playerPlayCreature(card, field_position);
            //addCreatureAttributes(d.gameObject, card);

        }
    }


    private void addCreatureAttributes(GameObject go, Card card) {
        GameObject attackGO = go.transform.Find(attackGO_name).gameObject;
        GameObject lifeGO = go.transform.Find(lifeGO_name).gameObject;
        attackGO.GetComponent<Text>().text = card.getActualAttack().ToString();
        lifeGO.GetComponent<Text>().text = card.getActualLife().ToString();
    }


    private void Update() {
        if (server == null) {
            getControlInstance();
        }
    }

}
