using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

// QUILL18
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;

    public GameObject placeholder = null;

    public BoxCollider m_coll = null;
    private UnitOrder m_order = null;

    private Image m_sprite;

    private void Start()
    {
        m_order = GetComponent<UnitOrder>();
        m_sprite = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

        parentToReturnTo = this.transform.parent;
        placeholderParent = parentToReturnTo;
        this.transform.SetParent(this.transform.parent.parent);

        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        m_coll = this.GetComponent<BoxCollider>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log ("OnDrag");

        this.transform.position = Input.mousePosition;

        if (m_order)
        {
            m_sprite.color = m_order.ColourSprite();
        }

        if (placeholder.transform.parent != placeholderParent)
            placeholder.transform.SetParent(placeholderParent);

        int newSiblingIndex = placeholderParent.childCount;

        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            if (this.transform.position.x < placeholderParent.GetChild(i).position.x)
            {

                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;

                break;
            }
        }

        m_coll.enabled = true;

        placeholder.transform.SetSiblingIndex(newSiblingIndex);


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_sprite.color = Color.white;

        if (m_order)
        {
            m_order.CheckForBuilding();
        }

        Debug.Log("OnEndDrag");
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        m_coll.enabled = false;

        Destroy(placeholder);
    }
}
