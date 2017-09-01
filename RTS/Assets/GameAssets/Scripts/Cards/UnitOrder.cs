using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitOrder : MonoBehaviour
{
    public string m_unitName;
    private Camera m_cam;

    void Start()
    {
        m_cam = GameObject.Find("Main Camera").GetComponent<Camera>();

    }

    void Update()
    {

    }

    public Color ColourSprite(bool canAfford)
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(mousePos, Vector3.forward * 11);

        if (hits.Length > 0 && canAfford)
        {
            for (int z = 0; z < hits.Length; z++)
            {
                BuildUnit _unit = hits[z].transform.gameObject.GetComponent<BuildUnit>();
                
                if (_unit)
                {
                    return Color.green;
                }

            }
        }

        return Color.red;
    }

    public bool CheckForBuilding()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(mousePos, Vector3.forward * 11);

        if (hits.Length > 0)
        {
            for (int z = 0; z < hits.Length; z++)
            {
                BuildUnit _unit = hits[z].transform.gameObject.GetComponent<BuildUnit>();
                Debug.Log("Raycast Hit");

                if (_unit)
                {
                    Debug.Log("Unit found");

                    _unit.Build(this);

                    return true;
                }

            }
        }

        return false;
    }
}
