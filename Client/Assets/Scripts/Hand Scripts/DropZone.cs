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
        return new Position(x, y, player_side);
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

    public void moveCreature (PointerEventData eventData, CardArrowDrag dragged_arrow) {
        Card card = dragged_arrow.transform.parent.GetComponent<CardGOInstance>().card;
        if (this.empty && card != null && card.isCreature() && card.canAttack()) {
            Position field_position = getCardPosition(this.getPlayerId());
            Position card_position = card.getPosition();

            if (this.movementIsValid(field_position, card_position) == false)
                return;

            if (server.tryMoveCharacter(card, field_position)) {
                empty = false;
                card.setPosition(field_position);
                GameObject card_go = dragged_arrow.transform.parent.gameObject;
                DropZone old_position = card_go.transform.parent.GetComponent<DropZone>();
                card.setCanAttack(false);
                old_position.empty = true;
                card_go.transform.SetParent(transform);
            }

            
        }

    }

    public bool movementIsValid(Position field_position, Position card_position) {
        return (field_position.column == card_position.column && Mathf.Abs(field_position.line - card_position.line) == 1 ||
                    field_position.line == card_position.line && Mathf.Abs(field_position.column - card_position.column) == 1);
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
            d.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            server.playerPlayCreature(card, field_position);
            addCreatureAttributes(d.gameObject, card);
            card.setPosition(field_position);
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
