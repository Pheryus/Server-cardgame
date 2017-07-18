using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class CardInstance {
    
    public static Card createCard(int id) {
        switch (id) {
            case 10:
                return bola_de_fogo();
            case 7:
                return flamestrike();

            case 11:
                return faisca();

            case 9:
                return nevasca();

            case 12:
                return ariete();
            case 8:
                return assassino_agil();
            case 13:
                return arqueiro();
                
            default:
                return non_effect_card(id);
        }
    }

    public static Card non_effect_card(int id) {

        if (id == 0)
            return new Card(0, "Yeti", "Personagem", 4, 4, 5, 1, 0);
        else if (id == 1)
            return new Card(1, "Raptor", "Personagem", 2, 3, 2, 1, 0);
        else if (id == 2)
            new Card(2, "Crocolisk", "Personagem", 2, 2, 3, 1, 0);
        else if (id == 3)
            return new Card(3, "Ogre", "Personagem", 6, 6, 7, 1, 0);
        
        return new Card(4, "War Golem", "Personagem", 7, 7, 7, 1, 0);
    }


    public static Card bola_de_fogo() {
        Card card = new Card(10, "Bola de Fogo", "Magia", 4, -1, -1, 2, 1);
        card.onActivateEffect = new Effects(1);
        card.onActivateEffect.dmg = 5;
        return card;
    }

    public static Card arqueiro() {
        Card card = new global::Card(13, "Arqueiro", "Personagem", 3, 2, 1, 1, 0);
        card.setRanged(true);
        return card;
    }

    

    public static Card faisca() {
        Card card = new Card(11, "Faísca", "Magia", 2, -1, -1, 1, 0);
        card.onActivateEffect = new Effects(1);
        card.onActivateEffect.dmg = 1;
        return card;
    }

    public static Card nevasca() {
        Card card = new Card(9, "Nevasca", "Magia", 4, -1, -1, 2, 1);
        card.onActivateEffect = new Effects(3);
        card.onActivateEffect.dmg = 2;
        return card;
    }

    public static Card flamestrike() {
        Card card = new Card(5, "Flamestrike", "Magia", 7, -1, -1, 1, 0);
        card.onActivateEffect = new Effects(2);
        card.onActivateEffect.dmg = 4;
        return card;
    }

    public static Card ariete() {
        Card card = new Card(12, "Aríete", "Personagem", 3, 0, 3, 3, 0);
        card.setSiegeDmg(3);
        return card;
    }

    public static Card assassino_agil() {
        Card card = new Card(8, "Assassino Ágil", "Personagem", 3, 2, 2, 1, 0);
        card.setCharge(true);
        return card;
    }




}
