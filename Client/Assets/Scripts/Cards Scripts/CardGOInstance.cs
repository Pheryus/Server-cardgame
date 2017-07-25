using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardGOInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Card card;

    private Control control;

    public void setControlReference() {
        Serverino server = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>();
        this.control = server.control;
    }

    public void Update() {
        if (this.control == null) {
            setControlReference();
        }
    }

    public void setCard(Card card) {
        this.card = card;
    }

    public void setCardImage() {
        this.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/" + card.getID().ToString());
   }

    public void setParent(Transform parent) {
        this.gameObject.transform.SetParent(parent, false);
    }


    /// <summary>
    /// Define a posição da carta no campo
    /// </summary>
    /// <param name="pos"></param>
    public void setFieldPosition(Position pos) {

        string go_name = this.getGameObjectName(pos);
       
        GameObject parent = GameObject.Find(go_name);
        this.setPlayerAttackArrow(pos);
        this.disableDraggable(pos);
        this.gameObject.transform.SetParent(parent.transform, false);
        this.card.setPosition(pos);
    }

    private void setPlayerAttackArrow(Position pos) {
        if (pos.side == control.getPlayerId())
            transform.GetChild(0).gameObject.SetActive(true);
    }

    private void disableDraggable(Position pos) {
        if (pos.side != control.getPlayerId())
            GetComponent<Draggable>().enabled = false;
    }

    /// <summary>
    /// Obtém o nome da posição do lugar que a carta se encontra.
    /// Ex: front1, opponentback2
    /// </summary>
    /// <param name="pos">Posição que se quer saber o nome</param>
    /// <returns>nome do GO</returns>
    private string getGameObjectName (Position pos) {
        string line = "";
        if (pos.line == 0)
            line = "front";
        else
            line = "back";

        string player_side = "";

        int player_id = this.control.getPlayerId();
        if (pos.side != player_id)
            player_side = "opponent";

        return player_side + line + (pos.column + 1).ToString();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        GameObject go = GameObject.Find("targetCard");
       
        go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/" + card.getID().ToString());
        UpdateAttributes.updateAttributes(card, go.transform);
        Color c = go.GetComponent<Image>().color;
        c.a = 255;
        go.GetComponent<Image>().color = c;
        
    }

    public void OnPointerExit(PointerEventData eventData) {
        GameObject go = GameObject.Find("targetCard");
        Color c = go.GetComponent<Image>().color;
        c.a = 0;
        go.GetComponent<Image>().color = c;
    }

}

