using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Field {

    private GameObject fieldGO;
    private Control control;

    const string cardGO_attack = "attack", cardGO_life = "life";


    public GameObject getFieldGO() {
        return fieldGO;
    }

    const string front = "front";
    const string back = "back";
    const string opponent_side = "opponent ";


    public Field(Control control) {
        this.control = control;
        fieldGO = GameObject.FindGameObjectWithTag("field");
    }

    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Card getCardByPosition(Position pos) {
        Transform card_position_transform = findCardPositionGOinField(pos);
        CardGOInstance card_instance = findCardInstanceInTransform(card_position_transform);
        if (card_instance)
            return card_instance.card;
        return null;
    }


    /// <summary>
    /// Causa dano ao personagem na posição alvo. Caso não exista personagem, nenhum dano é causado.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dmg"></param>
    public void damageToPosition (Position pos, int dmg) {
        Card card = this.getCardByPosition(pos);
        if (card != null) {
            card.dealDamage(dmg);
        }
    }


    public void check_board() {
        for (int i=0; i < 2; i++)
            for (int j=0; j < 3; j++)
                for (int k = 0; k < 2; k++) {
                    Position position = new Position(j, k, i);
                    Card card = getCardByPosition(position);
                    if (card != null) {
                        this.check_death(card);
                    }
                }
    }


    public void check_death(Card card) {
        if (card.isDead()) {
            Position position = card.getPosition();
            this.removeCard(position);
            this.removeCardFromGameObject(position);
            int player_id = this.control.getPlayerId();
            if (card.belongsToPlayer(player_id)) {
                control.returnCardToHand(card);
            }
        }
    }

    public void moveCreature(Position start_position, Position end_position) {
        Transform start_transform = findCardPositionGOinField(start_position);
        Transform end_transform = findCardPositionGOinField(end_position);
        Transform card_go = start_transform.GetChild(0);
        card_go.SetParent(end_transform);
    }



    /// <summary>
    /// Remove a instância da carta na posição do campo, e atualiza o gameobject da posição para empty (possibilitando que novas cartas sejam jogadas 
    /// naquela posição)
    /// </summary>
    /// <param name="pos">Posição da carta (linha, coluna) que será removida</param>
    public void removeCard(Position pos) {
        Transform transform = findCardPositionGOinField(pos);
        CardGOInstance card_instance = this.findCardInstanceInTransform(transform);

        if (card_instance) {
            UnityEngine.Object.Destroy(card_instance.gameObject);
            if (transform.GetComponent<DropZone>())
                transform.GetComponent<DropZone>().setEmpty(true);
        }
    }

    /// <summary>
    /// Retorna a instância da carta da posição no campo.
    /// </summary>
    /// <param name="card_position_transform"> GO que tem como filho um GO com informações da carta na posição específica</param>
    /// <returns>CardInstance da transform da posição da carta no campo. Null se não tiver cartas ali</returns>
    //
    private CardGOInstance findCardInstanceInTransform(Transform card_position_transform) {
        //se não tiver filhos, não existe nenhuma carta nessa posição.
        if (card_position_transform.childCount <= 0)
            return null;

        Transform card_transform = card_position_transform.GetChild(0);
        if (card_transform) {
            CardGOInstance card_instance = card_transform.GetComponent<CardGOInstance>();
            if (card_instance)
                return card_instance;
        }
        return null;
    }


    /// <summary>
    /// Dada a posição no campo, retorna o Transform do GO que contém a carta daquela posição.
    /// </summary>
    /// <param name="pos">Posição alvo que será retirado GO</param>
    /// <returns></returns>
    private Transform findCardPositionGOinField(Position pos) {
        string name = getFieldGOName(pos);
        Transform line = fieldGO.transform.Find(name);
        return line.transform.GetChild(pos.column);
    }

    private string getFieldGOName (Position pos) {
        string name = "";

        if (pos.side != control.getPlayerId())
            name += opponent_side;

        if (pos.line == 0)
            name += front;
        else
            name += back;
        return name;
    }

    public void removeCardFromGameObject(Position pos) {
        Transform cardParentGO = findCardPositionGOinField(pos);
        Transform card = cardParentGO.transform.GetChild(0);
        Transform card_arrow = card.GetChild(0);
        card_arrow.GetComponent<CardArrowDrag>().destroyLine();
        UnityEngine.Object.Destroy(cardParentGO.transform.GetChild(0).gameObject);
    }

   
    public void createCard(int id, Position pos, int player_id) {
        Card card = CardInstance.createCard(id);
        card.setPlayerId(player_id);
        card.setPosition(pos);

        Hand hand_reference = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
        GameObject card_prefab = hand_reference.prefab;

        //instancia uma nova carta
        GameObject go = (GameObject)UnityEngine.Object.Instantiate(card_prefab, Vector3.zero, new Quaternion(0, 0, 0, 0));

        //acha o filho attackGO
        GameObject attackGO = go.transform.Find(cardGO_attack).gameObject;
        changeGameObjectText(attackGO, card.getActualAttack().ToString());
        
        GameObject lifeGO = go.transform.Find(cardGO_life).gameObject;
        changeGameObjectText(lifeGO, card.getActualLife().ToString());

        CardGOInstance card_instance = go.GetComponent<CardGOInstance>();
        card_instance.setCard(card);
        card_instance.setImage();
        card_instance.setFieldPosition(pos);
        
        go.AddComponent<CardArrowDrop>();
        go.AddComponent<EffectTargetUI>();
    }
    
    //muda o text component do objeto
    private void changeGameObjectText(GameObject go, string text) {
        go.GetComponent<Text>().text = text;
    }

}
