using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public Transform parentToReturnTo = null;
    GameObject placeholder = null;
    public bool played = false;
    public string cardIs;

    Control control = null;

    public bool canBeMoved = false;
    public int hand_index;

    void getControlInstance() {
        control =  GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>().control;
    }


    /// <summary>
    /// Pega referencia da carta que está se tentando arrastar.
    /// Se ela ainda nao foi jogada, se o jogador tiver mana disponivel e se é possível jogá-la (nao esteja em cooldown, por exemplo),
    /// é possível movê-la.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData) {
        hand_index = transform.GetSiblingIndex();
        if (control != null) { 
            Card c = gameObject.GetComponent<CardGOInstance>().card;

            if (control.isPlayerTurn() == false)
                return;

            if (control.checkMana(c) && played == false && c.canPlay() == true) {
                this.dragCard();
            }
        }
    }

    private void dragCard() {
        this.canBeMoved = true;
        creatingPlaceholder();
        creatingLayoutElement();
        settingParent();
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }


    /// <summary>
    /// Cria placeholder no lugar original da carta. Caso ela nao seja jogada , o placeholder é destruido e ela vai para o lugar dele.
    /// </summary>
    private void creatingPlaceholder() {
        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
    }

    private void creatingLayoutElement() {
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.flexibleHeight = 0;
        le.flexibleWidth = 0;
    }

    private void settingParent() {
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent, false);
        this.transform.localPosition = this.transform.parent.position;
    }

    public void OnDrag(PointerEventData eventData) {
        if (control.isPlayerTurn() && this.canBeMoved) {
            Vector3 mousePosition = eventData.position;
            mousePosition.z = 1.0f;
            transform.localPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            //transform.position = eventData.position;
        }
    }

    private void Update() {
        if (control == null) {
            getControlInstance();
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
     
        if (this.canBeMoved) {
            this.transform.SetParent(parentToReturnTo, false);
            this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            GetComponent<CanvasGroup>().blocksRaycasts = true;

            Destroy(placeholder);
            if (cardIs == "played") {
                played = true;
            }
            this.canBeMoved = false;

        }
    }
    
}
