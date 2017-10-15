using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    private Camera m_cam;
    private DetectObjectsInTrigger m_selectionBox;

    public List<Unit> m_selectedUnits;

    public Vector2 m_selectionBoxStart;

    public int m_silver = 10;
    public Text m_silverUI;

    public bool m_dragingCard = false;

    public GameObject m_clickIndicator;

    void Start()
    {
        m_cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        m_selectionBox = GameObject.Find("Selection Box").GetComponent<DetectObjectsInTrigger>();

        m_selectionBox.gameObject.SetActive(false);
        m_silverUI = GameObject.Find("Resorces").GetComponent<Text>();

        m_silverUI.text = "Ag: " + m_silver;
    }

    void Update()
    {
        m_silverUI.text = "Ag: " + m_silver;

        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        Debug.DrawRay(mousePos, Vector3.forward * 11, Color.yellow);

        CheckForKOdUnits();
        m_selectionBox.CheckRefrences();

        if (Input.GetButtonDown("Fire2"))
        {
            if (m_clickIndicator)
            {
                Instantiate(m_clickIndicator, new Vector3(mousePos.x, mousePos.y, 1.0f), Quaternion.identity);
            }

            RaycastHit[] hits = Physics.RaycastAll(mousePos, Vector3.forward * 11);

            UnitData taragte = null;

            if (hits.Length > 0)
            {
                for (int z = 0; z < hits.Length; z++)
                {
                    Unit _unit = hits[z].transform.gameObject.GetComponent<Unit>();
                    Debug.Log("Raycast Hit");

                    if (_unit && !m_selectedUnits.Contains(_unit))
                    {
                        Debug.Log("Unit fouund");

                        taragte = _unit.m_data;

                        break;
                    }

                }
            }

            if (taragte)
            {
                for (int z = 0; z < m_selectedUnits.Count; z++)
                {
                    m_selectedUnits[z].m_data.m_targateUnit = taragte;
                }
            }
            else
            {
                for (int z = 0; z < m_selectedUnits.Count; z++)
                {
                    m_selectedUnits[z].m_data.m_targateUnit = null;
                }

                Vector3 meanPos = Vector3.zero;

                for (int z = 0; z < m_selectedUnits.Count; z++)
                {
                    meanPos += m_selectedUnits[z].transform.position;
                }

                meanPos /= m_selectedUnits.Count;

                for (int z = 0; z < m_selectedUnits.Count; z++)
                {
                    Vector3 moveTo = mousePos;
                    //moveTo += m_selectedUnits[z].transform.position - meanPos;

                    m_selectedUnits[z].Move(moveTo);
                }
            }
        }

        if (Input.GetButtonDown("Fire1") && !m_dragingCard)
        {
            m_selectionBoxStart = mousePos;
        }

        if (Input.GetButtonUp("Fire1") && !m_dragingCard)
        {
            if (Vector2.Distance(mousePos, m_selectionBoxStart) < 0.01f)
            {
                Debug.Log("Click");

                for (int z = 0; z < m_selectedUnits.Count; z++)
                {
                    m_selectedUnits[z].DeSelect();
                }

                m_selectedUnits.Clear();

                RaycastHit[] hits = Physics.RaycastAll(mousePos, Vector3.forward * 11);

                if (hits.Length > 0)
                {
                    for (int z = 0; z < hits.Length; z++)
                    {
                        Unit _unit = hits[z].transform.gameObject.GetComponent<Unit>();
                        Debug.Log("Raycast Hit");

                        if (_unit && !m_selectedUnits.Contains(_unit))
                        {
                            Debug.Log("Unit fouund");

                            _unit.Select();
                            m_selectedUnits.Add(_unit);
                            break;
                        }

                    }
                }
            }

            m_selectionBox.gameObject.SetActive(false);
        }

        if (Input.GetButton("Fire1") && !m_dragingCard)
        {
            if (Vector2.Distance(mousePos, m_selectionBoxStart) > 0.01f)
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

                m_selectedUnits.AddRange(m_selectionBox.m_UnitsInTrigger);

                Debug.Log("Drag Detected Length: " + m_selectionBox.m_UnitsInTrigger.Count);

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

    private void CheckForKOdUnits()
    {
        for (int z = 0; z < m_selectedUnits.Count; z++)
        {
            if (!m_selectedUnits[z])
            {
                m_selectedUnits.Remove(m_selectedUnits[z]);
            }
        }
    }
}
