using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardArrowDrag : MonoBehaviour,IDragHandler, IEndDragHandler {

    public Shader lineShader;

    private Draggable draggable;

    private CardInstance card_instance;

    private GameObject card_line;

    private Control control;

    private void Start() {
        draggable = transform.parent.GetComponent<Draggable>();
        card_instance = transform.parent.GetComponent<CardInstance>();

        this.control = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>().control;

    }
        
    public void destroyLine() {
        if (card_line != null)
            Destroy(card_line);
    }

    public void OnDrag(PointerEventData eventData) {
        Card card = card_instance.card;
        if (draggable.played == true  && card.canAttack() && this.control.isPlayerTurn()) {
            defineLine();
        }
    }

    private void defineLine() {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; //distance of the plane from the camera
        Vector3 start = transform.parent.transform.position;
        Vector3 end = Camera.main.ScreenToWorldPoint(screenPoint);
        drawLine(start, end, new Color(255, 0, 0));
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (card_line)
            GameObject.Destroy(card_line);
    }


     void drawLine(Vector3 start, Vector3 end, Color color, float duration = 0.01f) {
        LineRenderer lr;
        if (!card_line) { 
            card_line = new GameObject();
            card_line.transform.position = start;
            card_line.AddComponent<LineRenderer>();
            lr = card_line.GetComponent<LineRenderer>();
            lr.material = new Material(lineShader);
            lr.startColor = color;
            lr.endColor = color;
            //lr.SetColors(color, color);
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            
        }
        lr = card_line.GetComponent<LineRenderer>();
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

}
