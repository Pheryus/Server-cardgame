using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Field {

    private GameObject fieldGO;
    private Control control;
    private int player_id;

    const string cardGO_attack = "attack", cardGO_life = "life";
    private GameObject card_prefab;

    public GameObject getFieldGO() {
        return fieldGO;
    }

    const string front = "front";
    const string back = "back";
    const string opponent_side = "opponent ";


    public Field(Control control) {
        this.control = control;
        this.player_id = control.getPlayerId();
        fieldGO = GameObject.FindGameObjectWithTag("field");
        Hand hand_reference = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
        card_prefab = hand_reference.prefab;
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

    /// <summary>
    /// Avalia todas posições do campo, checando se morreu.
    /// </summary>
    public void check_board() {
        for (int i=0; i < 2; i++)
            for (int j=0; j < 3; j++)
                for (int k = 0; k < 2; k++) {
                    Position position = new Position(j, k, i);
                    Card card = getCardByPosition(position);
                    if (card != null) {
                        this.kill_card(card);
                    }
                }
    }

    /// <summary>
    /// Caso a carta esteja morta, remove ela do campo, remove sua posição, e retorna ela para o jogador caso pertença a ela.
    /// </summary>
    /// <param name="card">Carta a ser testada</param>
    public void kill_card(Card card) {
        if (card.isDead()) {
            Position position = card.getPosition();
            card.setPosition(null);
            this.removeCard(position);
            this.removeCardFromGameObject(position);
            int player_id = this.control.getPlayerId();
            if (card.belongsToPlayer(player_id) && card.isReusable()) {
                control.returnCardToHand(card);
            }
        }
    }

    /// <summary>
    /// Move a carta de uma posição para outra.
    /// Se a outra posição possuir uma carta também, as duas trocam de posições
    /// </summary>
    /// <param name="start_position">Posição inicial da carta</param>
    /// <param name="end_position">Posição final da carta</param>
    public void moveCreature(Position start_position, Position end_position) {
        Transform start_transform = findCardPositionGOinField(start_position);
        Transform end_transform = findCardPositionGOinField(end_position);
        Transform card_go = start_transform.GetChild(0);

        card_go.GetComponent<CardGOInstance>().card.setPosition(end_position);

        if (end_transform.childCount > 0) {
            Transform other_card_go = end_transform.GetChild(0);
            other_card_go.SetParent(start_transform);
            other_card_go.GetComponent<CardGOInstance>().card.setPosition(start_position);
        }
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
   
    

    public void createCard(Card card, Position pos, int player_id) {

        if (getCardByPosition(pos) != null) {
            Debug.Log("Erro! Já existe carta nessa posição");
            return;
        }


        card.setPlayerId(player_id);

        //instância uma nova carta
        GameObject card_go = (GameObject)UnityEngine.Object.Instantiate(card_prefab, Vector3.zero, new Quaternion(0, 0, 0, 0));

        /*
        //acha o filho attackGO
        GameObject attackGO = card_go.transform.Find(cardGO_attack).gameObject;
        //changeGameObjectText(attackGO, card.getActualAttack().ToString());
        
        GameObject lifeGO = card_go.transform.Find(cardGO_life).gameObject;
        //changeGameObjectText(lifeGO, card.getActualLife().ToString());
        */

        CardGOInstance card_instance = card_go.GetComponent<CardGOInstance>();
        card_instance.setControlReference();
        card_instance.setCard(card);
        card_instance.setCardImage();
        card_instance.setFieldPosition(pos);

        card_go.AddComponent<CardArrowDrop>();
        card_go.AddComponent<EffectTargetUI>();
    }
    
    //muda o text component do objeto
    private void changeGameObjectText(GameObject go, string text) {
        go.GetComponent<Text>().text = text;
    }

}
