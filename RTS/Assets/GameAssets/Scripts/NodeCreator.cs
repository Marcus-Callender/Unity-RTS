using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCreator : MonoBehaviour
{
    public float m_horizontalSpacing = 0.5f;
    public float m_verticalSpacing = 0.5f;

    public GameObject debugNode;

    /*void Start()
    {
        StartCoroutine(NodeCycle());
    }

    IEnumerator NodeCycle()*/
    void Start()
    {
        int verticalChecks = Mathf.FloorToInt((transform.localScale.x * 2.0f) / m_horizontalSpacing);
        int horizontalChecks = Mathf.FloorToInt((transform.localScale.y * 2.0f) / m_verticalSpacing);

        float startX = transform.position.x - transform.localScale.x;
        float startY = transform.position.y - transform.localScale.y;

        Vector3 colisionDetector = new Vector3(transform.position.x - transform.localScale.x, transform.position.y - transform.localScale.y, 3.0f);

        for (int z = 0; z < verticalChecks; z++)
        {
            for (int x = 0; x < horizontalChecks; x++)
            {
                //Ray collray = new Ray(new Vector3(startX + (m_horizontalSpacing * z), startY + (m_verticalSpacing * x), 3.0f), Vector3.forward * 5.0f);

                Debug.DrawRay(new Vector3(startX + (m_horizontalSpacing * z), startY + (m_verticalSpacing * x), -10.0f), Vector3.forward * 15.0f, Color.yellow, 15.0f);
                bool hitResult = Physics.Raycast(new Vector3(startX + (m_horizontalSpacing * z), startY + (m_verticalSpacing * x), -10.0f), Vector3.forward * 15.0f, 15.0f);

                if (!hitResult)
                {
                    if (debugNode)
                    {
                        GameObject node = Instantiate(debugNode);
                        node.transform.position = new Vector3(startX + (m_horizontalSpacing * z), startY + (m_verticalSpacing * x), 0.0f);
                        node.transform.SetParent(transform);
                    }
                    else
                    {
                        GameObject node = new GameObject("Node");
                        node.transform.position = new Vector3(startX + (m_horizontalSpacing * z), startY + (m_verticalSpacing * x), 0.0f);
                        node.transform.SetParent(transform);
                    }
                }

                //yield return new WaitForSeconds(0);
            }
        }
    }
}
