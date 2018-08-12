using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBuilding : HexUnit
{
    public GameObject m_uintToBuild = null;

    void Start()
    {

    }
    
    void Update()
    {
        if ( m_uintToBuild)
        {
            Instantiate(m_uintToBuild, transform.position + new Vector3(0.0f, -1.5f, 0.0f), Quaternion.identity);
            m_uintToBuild = null;
        }
    }

    public void BuildUnit(GameObject unitToBuild)
    {
        m_uintToBuild = unitToBuild;
    }

}
