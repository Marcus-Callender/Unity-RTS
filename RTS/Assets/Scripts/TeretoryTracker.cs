using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeretoryTracker : MonoBehaviour
{
    public int m_blocksX = 60;
    public int m_blocksY = 40;

    public float m_blockSizeX = 1.0f;
    public float m_blockSizeY = 1.0f;

    public GameObject m_blockPrefab;

    void Start()
    {
        for (int x = 0; x < m_blocksX; x++)
        {
            for (int y = 0; y < m_blocksY; y++)
            {
                Instantiate(m_blockPrefab, new Vector3(m_blockSizeX * x + gameObject.transform.position.x, m_blockSizeY * y - gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity, gameObject.transform);
            }
        }
    }
    
    void Update()
    {

    }
}
