using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum e_playerType
{
    HUMAN_PLAYER = 0,
    AI_PLAYER = 1
}

public class Player : MonoBehaviour
{
    [SerializeField]
    private Camera m_cam;

    public delegate void DeSelect();
    public DeSelect OnDeSelect;

    [SerializeField]
    private List<HexUnit> m_units;
    private List<int> m_selectedUnitsIndex;
    
    private Vector3 m_selectionStartPoint;

    [SerializeField]
    private RectTransform m_selectionBox;

    [SerializeField]
    private float m_selectionBoxPadding = 0.2f;

    [SerializeField]
    private e_playerType m_myType;

    void Awake()
    {
        m_units = new List<HexUnit>();

        m_selectedUnitsIndex = new List<int>();
    }

    void Update()
    {
        if (m_myType == e_playerType.HUMAN_PLAYER)
        {
            if (Input.GetButton("Fire1"))
            {
                UnitSelection();
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                if (m_selectedUnitsIndex.Count > 0)
                    SquareGridManager.m_instance.PathTo(m_units[m_selectedUnitsIndex[0]]);
            }

            if (Input.GetButtonUp("Fire1"))
            {
                m_selectionBox.gameObject.SetActive(false);
            }
        }
    }
    
    private void UnitSelection()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            m_selectionStartPoint = m_cam.ScreenToWorldPoint(Input.mousePosition);
            m_selectionBox.gameObject.SetActive(true);
        }

        Vector3 m_selectionStartScreenPoint = m_cam.WorldToScreenPoint(m_selectionStartPoint);
        Vector3 mouseWorldPoint = m_cam.ScreenToWorldPoint(Input.mousePosition);

        Vector2 topLeft = new Vector2(Mathf.Min(m_selectionStartScreenPoint.x, Input.mousePosition.x), Mathf.Max(m_selectionStartScreenPoint.y, Input.mousePosition.y));
        Vector2 botRight = new Vector2(Mathf.Max(m_selectionStartScreenPoint.x, Input.mousePosition.x), Mathf.Min(m_selectionStartScreenPoint.y, Input.mousePosition.y));

        Vector2 topLeftWorld = new Vector2(Mathf.Min(m_selectionStartPoint.x, mouseWorldPoint.x), Mathf.Max(m_selectionStartPoint.y, mouseWorldPoint.y));
        Vector2 botRightWorld = new Vector2(Mathf.Max(m_selectionStartPoint.x, mouseWorldPoint.x), Mathf.Min(m_selectionStartPoint.y, mouseWorldPoint.y));

        m_selectionBox.position = new Vector3((topLeft.x + botRight.x) / 2.0f, (topLeft.y + botRight.y) / 2.0f);
        m_selectionBox.sizeDelta = new Vector2(botRight.x - topLeft.x, topLeft.y - botRight.y);

        m_selectedUnitsIndex.Clear();
        
        for (int z = 0; z < m_units.Count; z++)
        {
            if (m_units[z].transform.position.x > topLeftWorld.x - m_selectionBoxPadding
                && m_units[z].transform.position.x < botRightWorld.x + m_selectionBoxPadding
                && m_units[z].transform.position.y < topLeftWorld.y + m_selectionBoxPadding
                && m_units[z].transform.position.y > botRightWorld.y - m_selectionBoxPadding)
            {
                m_units[z].Select();
                m_selectedUnitsIndex.Add(z);
            }
            else
            {
                m_units[z].DeSelect();
            }
        }
    }
    
    public void AddUnit(HexUnit unit)
    {
        if (!m_units.Contains(unit))
        {
            m_units.Add(unit);

            Debug.Log(gameObject.name + " added " + unit.gameObject.name);
        }
    }

    public void RemoveUnit(HexUnit unit)
    {
        if (m_units.Contains(unit))
        {
            Debug.Log(gameObject.name + " removed " + unit.gameObject.name);

            int index = m_units.IndexOf(unit);

            for (int z = 0; z < m_selectedUnitsIndex.Count; z++)
            {
                if (m_selectedUnitsIndex[z] == index)
                {
                    m_selectedUnitsIndex.Remove(z);
                }
                else if (m_selectedUnitsIndex[z] > index)
                {
                    m_selectedUnitsIndex[z]--;
                }
            }

            m_units.Remove(unit);
        }
    }
}
