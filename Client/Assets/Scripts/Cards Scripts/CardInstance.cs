using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private int card_id;

    public Card card;

    public void setCard(Card card) {
        this.card = card;
    }


    public void setImage() {
        this.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/" + card.getID().ToString());
   }

    public void setParent(Transform parent) {
        this.gameObject.transform.SetParent(parent, false);
    }

    public void setFieldPosition(Position pos) {
        GameObject parent;

        string lane = "";
        if (pos.y == 0) 
            lane = "front";
        else
            lane = "back";

        parent = GameObject.Find("opponent" + lane + (pos.x + 1).ToString());
        this.gameObject.transform.SetParent(parent.transform, false);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        GameObject go = GameObject.Find("targetCard");
       
        go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/" + card.getID().ToString());
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

