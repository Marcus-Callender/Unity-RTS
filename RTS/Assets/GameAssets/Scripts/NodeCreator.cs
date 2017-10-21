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

    private int[,] m_path;
    private int m_pathLength = 0;
    private int[,] m_toCheck;
    private int m_toChecLength = 0;

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
        m_divisions = new int[2] { verticalChecks, horizontalChecks };

        m_path = new int[horizontalChecks * verticalChecks, 2];
        m_toCheck = new int[horizontalChecks * verticalChecks, 2];

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

    public List<Vector2> pathTo(Vector3 start, Vector3 end)
    {
        int[] m_startNode = new int[2];
        int[] m_endNode = new int[2];

        m_startNode[0] = Mathf.RoundToInt((start.x - m_start.x) / m_horizontalSpacing);
        m_startNode[1] = Mathf.RoundToInt((start.y - m_start.y) / m_verticalSpacing);
        m_endNode[0] = Mathf.RoundToInt((end.x - m_start.x) / m_horizontalSpacing);
        m_endNode[1] = Mathf.RoundToInt((end.y - m_start.y) / m_verticalSpacing);

        m_startNode[0] = Mathf.Clamp(0, m_startNode[0], m_divisions[1]);
        m_startNode[1] = Mathf.Clamp(0, m_startNode[1], m_divisions[0]);
        m_endNode[0] = Mathf.Clamp(0, m_endNode[0], m_divisions[0]);
        m_endNode[1] = Mathf.Clamp(0, m_endNode[1], m_divisions[1]);

        Renderer startRenderer = m_debugNodes[m_startNode[0], m_startNode[1]].GetComponent<Renderer>();
        Renderer endRenderer = m_debugNodes[m_endNode[0], m_endNode[1]].GetComponent<Renderer>();

        startRenderer.material.color = Color.green;
        endRenderer.material.color = Color.red;

        m_path[0, 0] = m_startNode[0];
        m_path[0, 1] = m_startNode[1];
        m_pathLength = 1;
        m_toChecLength = 0;

        while (!(m_path[m_pathLength - 1, 0] == m_endNode[0] && m_path[m_pathLength - 1, 1] == m_endNode[1]))
        {
            GetSurroundingNodes(m_path[m_pathLength - 1, 0], m_path[m_pathLength - 1, 1]);

            int nextNode = GetClosestNode(m_endNode[0], m_endNode[1]);

            m_path[m_pathLength, 0] = 
                m_toCheck[nextNode, 0];
            m_path[m_pathLength, 1] = 
                m_toCheck[nextNode, 1];

            m_pathLength++;

            for (int z = nextNode; z < m_toChecLength -1; z++)
            {
                m_toCheck[z, 0] = m_toCheck[z + 1, 0];
                m_toCheck[z, 1] = m_toCheck[z + 1, 1];
            }

            m_toChecLength--;
        }

        List<Vector2> toReturn = new List<Vector2>();

        Debug.Log("Travel from: " + m_startNode[0] + ", " + m_startNode[1] + " to: " + m_endNode[0] + ", " + m_endNode[1] + ".");

        for (int z = 0; z < m_pathLength; z++)
        {
            if (z != 0 && z != m_pathLength - 1)
            {
                Debug.Log("Path: " + m_path[z, 0] + ", " + m_path[z, 1] + ".");

                Renderer _rend = m_debugNodes[m_path[z, 0], m_path[z, 1]].GetComponent<Renderer>();
                
                _rend.material.color = Color.yellow;
            }

            toReturn.Add(new Vector2(m_path[z, 0] * m_horizontalSpacing + m_start.x, m_path[z, 1] * m_verticalSpacing + m_start.y));
        }

        return toReturn;
    }

    private bool isValueInspected(int x, int y)
    {
        for (int z = 0; z < m_toChecLength; z++)
        {
            if (m_toCheck[z, 0] == x && m_toCheck[z, 1] == y)
            {
                return true;
            }
        }

        for (int z = 0; z < m_pathLength; z++)
        {
            if (m_path[z, 0] == x && m_path[z, 1] == y)
            {
                return true;
            }
        }

        return false;
    }

    private void GetSurroundingNodes(int x, int y)
    {
        for (int z = -1; z < 2; z++)
        {
            for (int c = -1; c < 2; c++)
            {
                if (z != 0 && c != 0)
                {
                    int[] newNode = new int[2];

                    newNode[0] = x + z;
                    newNode[1] = y + c;

                    if (newNode[0] > 0 && newNode[0] < m_divisions[0] && newNode[1] > 0 && newNode[1] < m_divisions[1])
                    {
                        if (m_nodes[newNode[0], newNode[1]])
                        {
                            if (!isValueInspected(newNode[0], newNode[1]))
                            {
                                m_toCheck[m_toChecLength, 0] = newNode[0];
                                m_toCheck[m_toChecLength, 1] = newNode[1];
                                m_toChecLength++;
                            }
                        }
                    }
                }
            }
        }
    }

    private int GetClosestNode(int compX, int compY)
    {
        int toReturn = 0;

        for (int z = 1; z < m_toChecLength; z++)
        {
            if (nodeDistance(m_toCheck[z,0], m_toCheck[z, 1], compX, compY) <= nodeDistance(m_toCheck[toReturn, 0], m_toCheck[toReturn, 1], compX, compY))
            {
                toReturn = z;
            }
        }

        return toReturn;
    }

    private float nodeDistance(int x1, int y1, int x2, int y2)
    {
        //return Mathf.Sqrt((Mathf.Abs(x1 - x2)) ^ 2 + (Mathf.Abs(y1 - y2)) ^ 2);
        return ((Mathf.Abs(x1 - x2)) + (Mathf.Abs(y1 - y2)));
    }
}
