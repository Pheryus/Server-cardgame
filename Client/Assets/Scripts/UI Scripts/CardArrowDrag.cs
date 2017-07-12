using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardArrowDrag : MonoBehaviour,IDragHandler, IEndDragHandler {

    public Shader lineShader;

    private Draggable draggable;

    private CardInstance card_instance;

    public bool valid_target = false;

    public Transform parentToReturnTo = null;
    GameObject myLine;

    private void Start() {
        draggable = transform.parent.GetComponent<Draggable>();
        card_instance = transform.parent.GetComponent<CardInstance>();
        Debug.Log("inicializou pelo menos");
    }
        
    public void destroyLine() {
        if (myLine != null)
            Destroy(myLine);
    }

    public void OnDrag(PointerEventData eventData) {
        Debug.Log("entrou aqui");
        Control control_reference = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>().control;
        if (draggable.played && card_instance.card.canAttack() && control_reference.isPlayerTurn()) {
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = 10.0f; //distance of the plane from the camera
            Vector3 start = transform.parent.transform.position;
            Vector3 end = Camera.main.ScreenToWorldPoint(screenPoint);
            DrawLine(start, end, new Color(255, 0, 0));
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (myLine)
            GameObject.Destroy(myLine);
    }


     void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.01f) {
        LineRenderer lr;
        if (!myLine) { 
            myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(lineShader);
            lr.SetColors(color, color);
            lr.SetWidth(0.1f, 0.1f);
        }
        lr = myLine.GetComponent<LineRenderer>();
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

}
