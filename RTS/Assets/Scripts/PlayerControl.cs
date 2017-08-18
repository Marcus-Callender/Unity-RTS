using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Timer m_leftClickTimer;
    private Camera m_cam;
    private SelectionBox m_selectionBox;

    public List<Unit> m_selectedUnits;

    public Vector2 m_selectionBoxStart;

    void Start()
    {
        m_leftClickTimer = new Timer();
        m_leftClickTimer.m_time = 0.2f;

        m_cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        m_selectionBox = GameObject.Find("Selection Box").GetComponent<SelectionBox>();

        m_selectionBox.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        Debug.DrawRay(mousePos, Vector3.forward * 11, Color.yellow);

        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 meanPos = Vector3.zero;

            for (int z = 0; z < m_selectedUnits.Count; z++)
            {
                meanPos += m_selectedUnits[z].transform.position;
            }

            meanPos /= m_selectedUnits.Count;
            
            for (int z = 0; z < m_selectedUnits.Count; z++)
            {
                Vector3 moveTo = mousePos;
                moveTo += meanPos - m_selectedUnits[z].transform.position;

                //m_selectedUnits[z].Move(mousePos);
                m_selectedUnits[z].Move(moveTo);
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            m_leftClickTimer.Play();
            m_selectionBoxStart = mousePos;
        }

        m_leftClickTimer.Cycle();

        if (Input.GetButtonUp("Fire1"))
        {
            if (m_leftClickTimer.m_playing && !m_leftClickTimer.m_completed)
            {
                Debug.Log("Click");

                RaycastHit hit;

                for (int z = 0; z < m_selectedUnits.Count; z++)
                {
                    m_selectedUnits[z].DeSelect();
                }

                m_selectedUnits.Clear();

                if (Physics.Raycast(mousePos, Vector3.forward * 11, out hit))
                {
                    Unit _unit = hit.transform.gameObject.GetComponent<Unit>();
                    Debug.Log("Raycast Hit");

                    if (_unit && !m_selectedUnits.Contains(_unit))
                    {
                        Debug.Log("Unit fouund");

                        _unit.Select();
                        m_selectedUnits.Add(_unit);
                    }
                }
            }

            m_selectionBox.gameObject.SetActive(false);
        }

        if (Input.GetButton("Fire1"))
        {
            if (m_leftClickTimer.m_completed)
            {
                for (int z = 0; z < m_selectedUnits.Count; z++)
                {
                    Debug.Log("Drag DeSelect");
                    m_selectedUnits[z].DeSelect();
                }
                
                m_selectedUnits.Clear();

                Debug.Log("Drag Clear Length: " + m_selectedUnits.Count);

                Debug.Log("Hold");

                List<Unit> oldUnitList = m_selectedUnits;

                m_selectedUnits.AddRange(m_selectionBox.m_detected);

                Debug.Log("Drag Detected Length: " + m_selectionBox.m_detected.Count);

                for (int z = 0; z < oldUnitList.Count; z++)
                {
                    if (!m_selectedUnits.Contains(oldUnitList[z]))
                    {
                        oldUnitList[z].DeSelect();
                    }
                }

                for (int z = 0; z < m_selectedUnits.Count; z++)
                {
                    Debug.Log("Drag select");
                    m_selectedUnits[z].Select();
                }

                m_selectionBox.gameObject.SetActive(true);

                m_selectionBox.transform.position = new Vector3((m_selectionBoxStart.x + mousePos.x) * 0.5f, (m_selectionBoxStart.y + mousePos.y) * 0.5f, m_selectionBox.transform.position.z);

                m_selectionBox.transform.localScale = new Vector3(Mathf.Abs(m_selectionBoxStart.x - mousePos.x), Mathf.Abs(m_selectionBoxStart.y - mousePos.y), m_selectionBox.transform.localScale.z);
            }
        }
    }
}
