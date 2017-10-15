using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCreator : MonoBehaviour
{
    public float m_horizontalSpacing = 0.5f;
    public float m_verticalSpacing = 0.5f;

    public GameObject debugNode;

    public bool[,] m_nodes;
    public GameObject[,] m_debugNodes;

    private Vector2 m_start;
    private int[] m_divisions;

    /*void Start()
    {
        StartCoroutine(NodeCycle());
    }

    IEnumerator NodeCycle()*/
    void Start()
    {
        int verticalChecks = Mathf.FloorToInt((transform.localScale.x * 2.0f) / m_horizontalSpacing);
        int horizontalChecks = Mathf.FloorToInt((transform.localScale.y * 2.0f) / m_verticalSpacing);

        m_nodes = new bool[verticalChecks, horizontalChecks];
        m_debugNodes = new GameObject[verticalChecks, horizontalChecks];

        float startX = transform.position.x - transform.localScale.x;
        float startY = transform.position.y - transform.localScale.y;

        m_start = new Vector2(startX, startY);
        m_divisions = new int[2] { horizontalChecks, verticalChecks };

        Vector3 colisionDetector = new Vector3(transform.position.x - transform.localScale.x, transform.position.y - transform.localScale.y, 3.0f);

        for (int z = 0; z < verticalChecks; z++)
        {
            for (int x = 0; x < horizontalChecks; x++)
            {
                Debug.DrawRay(new Vector3(startX + (m_horizontalSpacing * z), startY + (m_verticalSpacing * x), -10.0f), Vector3.forward * 15.0f, Color.yellow, 15.0f);
                bool hitResult = Physics.Raycast(new Vector3(startX + (m_horizontalSpacing * z), startY + (m_verticalSpacing * x), -10.0f), Vector3.forward * 15.0f, 15.0f, 1 << 8);

                if (!hitResult)
                {
                    m_nodes[z, x] = true;

                    if (debugNode)
                    {
                        GameObject node = Instantiate(debugNode);
                        node.transform.position = new Vector3(startX + (m_horizontalSpacing * z), startY + (m_verticalSpacing * x), 0.0f);
                        node.transform.SetParent(transform);

                        m_debugNodes[z, x] = node;
                    }
                    else
                    {
                        GameObject node = new GameObject("Node");
                        node.transform.position = new Vector3(startX + (m_horizontalSpacing * z), startY + (m_verticalSpacing * x), 0.0f);
                        node.transform.SetParent(transform);

                        m_debugNodes[z, x] = node;
                    }
                }
                else
                {
                    m_nodes[z, x] = false;
                    m_debugNodes[z, x] = null;
                }

                //yield return new WaitForSeconds(0);
            }
        }
    }

    public void findPath(Vector3 start, Vector3 end)
    {
        int[] m_startNode = new int[2];
        int[] m_endNode = new int[2];

        m_startNode[0] = Mathf.RoundToInt((start.x - m_start.x) / m_horizontalSpacing);
        m_startNode[1] = Mathf.RoundToInt((start.y - m_start.y) / m_verticalSpacing);
        m_endNode[0] = Mathf.RoundToInt((end.x - m_start.x) / m_horizontalSpacing);
        m_endNode[1] = Mathf.RoundToInt((end.y - m_start.y) / m_verticalSpacing);

        m_startNode[0] = Mathf.Clamp(0, m_startNode[0], m_divisions[0]);
        m_startNode[1] = Mathf.Clamp(0, m_startNode[1], m_divisions[1]);
        m_endNode[0] = Mathf.Clamp(0, m_endNode[0], m_divisions[0]);
        m_endNode[1] = Mathf.Clamp(0, m_endNode[1], m_divisions[1]);

        Renderer startRenderer = m_debugNodes[m_startNode[0], m_startNode[1]].GetComponent<Renderer>();
        Renderer endRenderer = m_debugNodes[m_endNode[0], m_endNode[1]].GetComponent<Renderer>();

        startRenderer.material.color = Color.green;
        endRenderer.material.color = Color.red;
    }
}
