using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicZone : MonoBehaviour, IDropHandler {

    private Serverino server;

    public void getControlInstance() {
        this.server = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>();
    }

    public void OnDrop(PointerEventData eventData) {

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.canBeMoved == true && server.control.isPlayerTurn()) {
            CardGOInstance cardInstance = eventData.pointerDrag.GetComponent<CardGOInstance>();
            Card card = cardInstance.card;

            if (card.isMagic()) {
                if (server.tryPlayMagicWithoutTarget(card.getID())) {
                    server.control.playMagic(card);
                }
            }

        }
    }


    void Update () {
        if (server == null) {
            getControlInstance();
        }
    }
}
