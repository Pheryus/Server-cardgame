﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenCards {
    public static Card getMeleeGoblin() {
        Card card = new Card(16, "Goblin Warrior", "Personagem", 1, 1, 1, -1, -1);
        card.setReusable(false);
        return card;
    }

    public static Card getRangedGoblin() {
        Card card = new Card(15, "Goblin Ranger", "Personagem", 1, 1, 1, -1, -1);
        card.setRanged(true);
        card.setReusable(false);
        return card;
    }

}
