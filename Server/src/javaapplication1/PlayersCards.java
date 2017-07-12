/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package javaapplication1;

/**
 *
 * @author Pedro
 */
public class PlayersCards {
    
    public Card[] cards;
    
        
    public PlayersCards(){
        cards = new Card[20];
        
    }
    
    Card getCardById(int id){
        for (int i=0; i < cards.length; i++){
            if (cards[i].getId() == id){
                return cards[i];
            }
        }
        return null;
    }
    
    
}
