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
public final class CardInstance {
    
    
    
    static Card fireball(){
        Card card = new Card(6, "Fireball", "Magia", 4, -1, -1, 2, 1);
        card.onPlayEffect = (Object param) -> {
            Card target = (Card)param;
            target.takeDamage(5);
        };
        return card;
    }
    
    static Card flamestrike(){
        Card card = new Card(5, "Flamestrike", "Magia", 7, -1, -1, 1, 0);
        
        
        card.onPlayEffect = new ICommand() {
            @Override
            public void execute(Object param) {
                card.getFieldControl().damageToPlayerCards(4, (int)param);
            }
        };
        return card;
    }
    
    
    
}
