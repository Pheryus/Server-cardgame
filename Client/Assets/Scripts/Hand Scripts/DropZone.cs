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
            CardInstance cardInstance = eventData.pointerDrag.GetComponent<CardInstance>();
            Card card = cardInstance.card;
            
            int player_side = card.getPlayerId();
            Position field_position = getCardPosition(player_side);

            //se o servidor deixar ele jogar a cartita;
            if (server.TryPlayCharacter(card, field_position, d.hand_index)) {
                d.parentToReturnTo = this.transform;
                d.cardIs = "played";
                
                empty = false;
                d.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                server.playerPlayCreature(card, field_position);
                addCreatureAttributes(d.gameObject, card);
                d.gameObject.GetComponent<CardInstance>().card.setPosition(field_position);
            }
        }
    }

    public void setEmpty(bool empty) {
        this.empty = empty;
    }

    private void addCreatureAttributes(GameObject go, Card card) {
        GameObject attackGO = go.transform.Find(attackGO_name).gameObject;
        GameObject lifeGO = go.transform.Find(lifeGO_name).gameObject;
        attackGO.GetComponent<Text>().text = card.getActualDmg().ToString();
        lifeGO.GetComponent<Text>().text = card.getActualLife().ToString();
    }


    private void Update() {
        if (server == null) {
            getControlInstance();
        }
    }

}
