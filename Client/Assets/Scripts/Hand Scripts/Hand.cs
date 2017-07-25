using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour {
   
    private const int max_cards = 10;
    public  ArrayList hand_cards = new ArrayList();
    public ArrayList canplayHandCards = new ArrayList();
    public GameObject prefab;

    public void AddCards(Card[] cards) {
        for (int i = 0; i < cards.Length; i++)
            hand_cards.Add(cards[i]);
    }


    private void createTargetArrowUI(GameObject go) {
        go.GetComponent<Draggable>().enabled = false;
        go.GetComponent<EffectArrowUI>().enabled = true;
    }

    public void addCard(Card card) {

        GameObject go = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);

        if (card.onActivateEffect != null && card.onActivateEffect.id == 1) 
            createTargetArrowUI(go);
        else
            go.GetComponent<EffectArrowUI>().enabled = false;

        go.GetComponent<CardGOInstance>().setCard(card);
        go.GetComponent<CardGOInstance>().setParent(this.gameObject.transform);
        go.GetComponent<CardGOInstance>().setCardImage();
        go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        hand_cards.Add(card);
    }

    public void endTurn() {
        foreach (Card card in hand_cards) {
            card.endOfTurnInHand();
        }
    }

    public string checkIfCanPlay(int id) {
        //hand_cards[id];
        return "";
    }
    
}
