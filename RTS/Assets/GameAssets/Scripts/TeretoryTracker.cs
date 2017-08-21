using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeretoryTracker : MonoBehaviour
{
    public int m_totalBlocks;
    public int m_blueBlocks;
    public int m_greenBlocks;

    public GameObject m_topObject;
    public GameObject m_bottomObject;

    private float m_top;
    private float m_bottom;

    private float m_objectsDistance;
    private float m_objectsMidpoint;

    void Start()
    {
        m_totalBlocks = transform.childCount;

        m_top = m_topObject.transform.localPosition.y + (m_topObject.transform.localScale.y * 0.5f);
        m_bottom = m_bottomObject.transform.localPosition.y - (m_bottomObject.transform.localScale.y * 0.5f);
        m_objectsDistance = m_top - m_bottom;

        m_objectsMidpoint = (m_topObject.transform.localPosition.y + m_bottomObject.transform.localPosition.y) * 0.5f;

        StartCoroutine(CheckScore());
    }

    void Update()
    {

    }

    IEnumerator CheckScore()
    {
        while (true)
        {
            m_blueBlocks = 0;
            m_greenBlocks = 0;

            for (int z = 0; z < transform.childCount; z++)
            {
                if (transform.GetChild(z).tag == "Green")
                {
                    m_greenBlocks += 1;
                }
                else if (transform.GetChild(z).tag == "Blue")
                {
                    m_blueBlocks += 1;
                }
            }

            ScaleProgressObjects();

            yield return new WaitForSeconds(2.0f);
        }
    }

    private void ScaleProgressObjects()
    {
        float bluePercent = (float)m_blueBlocks / ((float)m_blueBlocks + (float)m_greenBlocks);
        float greenPercent = 1.0f - bluePercent;

        m_topObject.transform.localScale = new Vector3(m_topObject.transform.localScale.x, m_objectsDistance * greenPercent, m_topObject.transform.localScale.z);
        m_bottomObject.transform.localScale = new Vector3(m_topObject.transform.localScale.x, m_objectsDistance * bluePercent, m_topObject.transform.localScale.z);

        m_topObject.transform.localPosition = new Vector3(m_topObject.transform.localPosition.x, m_objectsMidpoint + ((m_objectsDistance * bluePercent) * 0.5f), m_topObject.transform.localPosition.z);
        m_bottomObject.transform.localPosition = new Vector3(m_topObject.transform.localPosition.x, m_objectsMidpoint - ((m_objectsDistance * greenPercent) * 0.5f), m_topObject.transform.localPosition.z);
    }
}
