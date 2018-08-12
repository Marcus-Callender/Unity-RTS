using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUnit : MonoBehaviour
{
    public GameObject[] m_canBuild;

    private HexBuilding m_building;

    void Start()
    {
        m_building = GetComponent<HexBuilding>();
    }

    void Update()
    {

    }

    public void Build(UnitOrder order)
    {
        if (m_building.m_uintToBuild == null)
        {
            GameObject _unitOrder = null;

            for (int z = 0; z < m_canBuild.Length; z++)
            {
                if (m_canBuild[z].name == order.m_unitName)
                {
                    _unitOrder = m_canBuild[z];
                }
            }

            if (_unitOrder)
            {
                Draggable data = order.gameObject.GetComponent<Draggable>();

                //Instantiate(_unitOrder, transform.position + new Vector3(0.0f, -1.5f, 0.0f), Quaternion.identity);
                m_building.BuildUnit(_unitOrder);

                if (order)
                {
                    Destroy(data.placeholder);
                    Destroy(order.gameObject);
                }
            }
        }
    }
}
