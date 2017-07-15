using UnityEngine;
using System.Collections;

public class ViewModel : MonoBehaviour {



    public void ButtonClick() {
        Serverino control = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>();
        if (control.control.isPlayerTurn())
            control.endTurn();
    }
}
