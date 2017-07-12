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

    public void AddCard(Card card) {
        
        GameObject go = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
        go.GetComponent<CardInstance>().setCard(card);
        go.GetComponent<CardInstance>().setParent(this.gameObject.transform);
        go.GetComponent<CardInstance>().setImage();
        go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        hand_cards.Add(card);
        for (int i=0; i < hand_cards.Count; i++) {
            object obj = hand_cards[i];
            Card card_in_hand = (Card)obj;
        }
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
