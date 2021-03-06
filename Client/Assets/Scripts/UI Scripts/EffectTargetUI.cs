﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectTargetUI : MonoBehaviour, IDropHandler {

    Control control;
    Serverino server;

    public void Start() {
        this.server = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>();
        this.control = server.control;
    }


    public void OnDrop(PointerEventData eventData) {
        EffectArrowUI dragged_card = eventData.pointerDrag.GetComponent<EffectArrowUI>();

        if (dragged_card != null && control.isPlayerTurn()) {
            Card card = dragged_card.GetComponent<CardGOInstance>().card;
            if (card.isMagic()) {
                this.playMagic(card);
            }
        }

    }

    private void playMagic(Card card) {

        Card target_card = GetComponent<CardGOInstance>().card;
        if (target_card == null)
            return;

        Position target_position = target_card.getPosition();

        if (server.tryPlayMagicWithTarget(card.getID(), target_position)) {
            server.control.playMagic(card, target_position);
        }
    }

}
