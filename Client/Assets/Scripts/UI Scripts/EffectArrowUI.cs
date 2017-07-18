using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectArrowUI : MonoBehaviour, IDragHandler, IEndDragHandler {

    public Shader lineShader;

    private GameObject card_line;

    private Control control;

    public void Start() {
        this.control = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>().control;

    }


    public void OnDrag(PointerEventData eventData) {
        Card card = transform.GetComponent<CardGOInstance>().card;
        if (card != null && card.isMagic() && control.checkMana(card)) {
            this.magic(card);
        }
    }

    private void magic(Card magic) {
        this.defineLine();
    }

    public void destroyLine() {
        if (card_line != null)
            Destroy(card_line);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (card_line)
            GameObject.Destroy(card_line);
    }

    private void defineLine() {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; //distance of the plane from the camera
        Vector3 start = transform.position;
        Vector3 end = Camera.main.ScreenToWorldPoint(screenPoint);
        this.drawLine(start, end, new Color(0, 255, 0));
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
