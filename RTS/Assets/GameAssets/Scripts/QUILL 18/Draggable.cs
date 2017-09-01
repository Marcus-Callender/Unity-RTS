using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

// QUILL18
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;

    public GameObject placeholder = null;

    public BoxCollider m_coll = null;

    private UnitOrder m_order = null;
    private CreateAreaDamage m_areaDamage = null;

    private Image m_sprite;
    public int m_cost = 5;
    private Text m_text;

    private PlayerControl m_player;

    private void Start()
    {
        m_order = GetComponent<UnitOrder>();
        m_areaDamage = GetComponent<CreateAreaDamage>();
        m_sprite = GetComponent<Image>();
        m_text = GetComponentInChildren<Text>();
        m_text.text = "Ag : " + m_cost;
        m_text.enabled = false;

        m_player = GameObject.Find("Player").GetComponent<PlayerControl>();
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
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log ("OnDrag");

        this.transform.position = Input.mousePosition;

        m_player.m_dragingCard = true;

        if (m_order)
        {
            m_sprite.color = m_order.ColourSprite(m_player.m_silver >= m_cost);
        }
        else if (m_areaDamage)
        {
            m_sprite.color = m_areaDamage.ColourSprite(m_player.m_silver >= m_cost);
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
        

        placeholder.transform.SetSiblingIndex(newSiblingIndex);


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_sprite.color = Color.white;

        m_player.m_dragingCard = false;

        if (m_order)
        {
            if (m_player.m_silver >= m_cost)
            {
                if (m_order.CheckForBuilding())
                {
                    m_player.m_silver -= m_cost;
                }
            }
        }
        else if (m_areaDamage)
        {
            if (m_player.m_silver >= m_cost)
            {
                m_areaDamage.Activate();
                m_player.m_silver -= m_cost;
            }
        }

        Debug.Log("OnEndDrag");
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        

        Destroy(placeholder);
    }

    public void OnPointerEnter(PointerEventData data)
    {
        Debug.Log("Mouse Enter");
        m_text.enabled = true;

        if (m_player.m_silver >= m_cost)
        {
            m_text.color = Color.yellow;
        }
        else
        {
            m_text.color = Color.red;
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        Debug.Log("Mouse Exit");
        m_text.enabled = false;
    }
}
