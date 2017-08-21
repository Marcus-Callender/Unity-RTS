using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeretoryTracker : MonoBehaviour
{
    public int m_totalBlocks;
    public int m_BlueBlocks;
    public int m_GreenBlocks;

    void Start()
    {
        m_totalBlocks = transform.childCount;

        StartCoroutine(CheckScore());
    }

    void Update()
    {

    }

    IEnumerator CheckScore()
    {
        while (true)
        {
            m_BlueBlocks = 0;
            m_GreenBlocks = 0;

            for (int z = 0; z < transform.childCount; z++)
            {
                if (transform.GetChild(z).tag == "Green")
                {
                    m_GreenBlocks += 1;

                }
                else if (transform.GetChild(z).tag == "Blue")
                {
                    m_BlueBlocks += 1;
                }
            }

            yield return new WaitForSeconds(2.0f);
        }
    }
}
