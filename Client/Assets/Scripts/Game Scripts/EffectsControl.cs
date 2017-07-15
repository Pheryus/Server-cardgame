using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsControl {


    public Control control;

    public EffectsControl(Control control) {
        this.control = control;
    }

    /// <summary>
    /// Causa dano em todo personagens no campo dos dois jogadores
    /// </summary>
    /// <param name="dmg">Dano causado a todos personagens</param>
    public void boardclear(int dmg) {
        for (int i = 0; i < 2; i++) 
            for (int j = 0; j < 3; j++) 
                for (int k = 0; k < 2; k++) { 
                    Position position = new Position(j, k, i);
                    control.getField().damageToPosition(position, dmg);
                }
    }

    /// <summary>
    /// Aqui podem existir efeitos que interajam com o dano causado, como aumentar o dano com dano magico, etc
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dmg"></param>
    public void damageToTargetPosition(Position position, int dmg) {
        control.getField().damageToPosition(position, dmg);

    }

}
