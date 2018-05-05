using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateHexGrid : MonoBehaviour
{
    [Serializable]
    enum E_HexType
    {
        POINTED_TOP = 0,
        FLAT_TOP = 1
    }
    
    [SerializeField]
    private GameObject m_hexPrefab;

    private GameObject[,] m_createdHexes;

    [SerializeField]
    private E_HexType m_hexType;

    [SerializeField]
    private int m_width = 5;

    [SerializeField]
    private int m_height = 5;

    [SerializeField]
    private float m_hexSize = 0.5f;

    void Start()
    {
        m_createdHexes = new GameObject[m_width, m_height];
        UpdateGrid();
    }

    void Update()
    {

    }

    void UpdateGrid()
    {
        for (int z = 0; z < m_width; z++)
        {
            for (int x = 0; x < m_height; x++)
            {
                m_createdHexes[z, x] = Instantiate(m_hexPrefab, new Vector3((z * (Mathf.Sqrt(3.0f) * m_hexSize)) + ((Mathf.Sqrt(3.0f) * m_hexSize * 0.5f) * (x % 2)), x * 1.5f * -m_hexSize, 0.0f), Quaternion.identity, transform);
                m_createdHexes[z, x].transform.localPosition = m_createdHexes[z, x].transform.position;
            }
        }
    }
}
