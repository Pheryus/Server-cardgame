using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;




public class CardGOInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Card card;

    private Control control;

    public void setControlReference() {
        Serverino server = GetServerino.getServerino();
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
    public void setFieldPosition(string go_name, Position pos) {

        //string go_name = this.getGameObjectName(pos);
       
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

