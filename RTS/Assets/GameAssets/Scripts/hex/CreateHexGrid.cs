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

    [Serializable]
    public struct hexIndex
    {
        public int q;
        public int r;

        public hexIndex(int _x, int _y)
        {
            q = _x;
            r = _y;
        }

        public void set(int _x, int _y)
        {
            q = _x;
            r = _y;
        }

        public Vector3Int CubeCoordinates()
        {
            int adjustedQ = q - ((int)(r * 0.5f));
            return new Vector3Int(adjustedQ, (-adjustedQ - r) , r);
            //return new Vector3Int(q , (-q - r) , r);
        }

        public bool isNull()
        {
            return q == -1 && r == -1;
        }

        public void Null()
        {
            q = -1;
            r = -1;
        }
    }

    [SerializeField]
    private Camera m_cam;

    [SerializeField]
    private GameObject m_hexPrefab;

    private HexTile[,] m_createdHexes;

    [SerializeField]
    private E_HexType m_hexType;

    [SerializeField]
    private int m_width = 5;

    [SerializeField]
    private int m_height = 5;

    [SerializeField]
    private float m_hexSize = 0.5f;

    [SerializeField]
    private LayerMask m_hexMask;

    private hexIndex m_selectedHexIndex = new hexIndex();

    private float xScreenSize = 17.77778f;
    private float yScreenSize = 10.0f;

    private int xHexPixelSize;
    private int yHexPixelSize;

    private float pixelToUU;
    private float UUToPixel;

    void Start()
    {
        m_selectedHexIndex.set(-1, -1);
        m_createdHexes = new HexTile[m_width, m_height];
        UpdateGrid();

        xHexPixelSize = Mathf.RoundToInt(m_hexSize * (xScreenSize / m_cam.pixelWidth));
        yHexPixelSize = Mathf.RoundToInt(m_hexSize * (yScreenSize / m_cam.pixelHeight));

        pixelToUU = yScreenSize / m_cam.pixelHeight;
        UUToPixel = m_cam.pixelHeight / yScreenSize;
    }

    void Update()
    {
        Debug.DrawRay(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector3.forward * 11, Color.green);

        if (Input.GetButtonDown("Fire1"))
        {
            if (m_selectedHexIndex.isNull())
            {
                StartHexPathing();
            }
            else
            {
                PathTo();
            }
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            ResetHexPaths();
        }
    }

    private void StartHexPathing()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        ContactFilter2D contactFilter = new ContactFilter2D();
        RaycastHit2D[] results = new RaycastHit2D[3];

        if (0 < Physics2D.Raycast(mousePos, Vector3.forward * 100.0f, contactFilter, results, m_hexMask))
        {
            hexIndex index = GetHexIndex(results[0].collider.gameObject.transform.localPosition);
            Debug.Log("Index: " + index.q + ", " + index.r);

            m_createdHexes[index.q, index.r].Selected();
            m_selectedHexIndex = index;

            for (int z = 0; z < m_width; z++)
            {
                for (int x = 0; x < m_height; x++)
                {
                    m_createdHexes[z, x].SetText(HexDistance(m_selectedHexIndex, GetHexIndex(m_createdHexes[z, x].transform.localPosition)).ToString());
                }
            }
        }
    }

    private void PathTo()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        ContactFilter2D contactFilter = new ContactFilter2D();
        RaycastHit2D[] results = new RaycastHit2D[3];

        if (0 < Physics2D.Raycast(mousePos, Vector3.forward * 100.0f, contactFilter, results, m_hexMask))
        {
            hexIndex index = GetHexIndex(results[0].collider.gameObject.transform.localPosition);
            Debug.Log("Index: " + index.q + ", " + index.r);
            Debug.Log("Distance: " + HexDistance(m_selectedHexIndex, index));

            m_createdHexes[index.q, index.r].Selected();
            for (int z = 0; z < m_width; z++)
            {
                for (int x = 0; x < m_height; x++)
                {
                    m_createdHexes[z, x].SetText(HexDistance(index, GetHexIndex(m_createdHexes[z, x].transform.localPosition)).ToString());
                }
            }
        }

        m_selectedHexIndex.set(-1, -1);
    }

    private int HexDistance(hexIndex a, hexIndex b)
    {
        Vector3Int aCube = a.CubeCoordinates();
        Vector3Int bCube = b.CubeCoordinates();
        //aCube.x -= ((int)(aCube.y * 0.5f));
        //bCube.x -= ((int)(bCube.y * 0.5f));
        return Mathf.Max(Mathf.Abs(aCube.x - bCube.x), Mathf.Abs(aCube.y - bCube.y), Mathf.Abs(aCube.z - bCube.z));
    }

    private void ResetHexPaths()
    {
        m_selectedHexIndex.set(-1, -1);

        for (int z = 0; z < m_width; z++)
        {
            for (int x = 0; x < m_height; x++)
            {
                m_createdHexes[z, x].DeSelect();
            }
        }
    }

    void UpdateGrid()
    {
        for (int z = 0; z < m_width; z++)
        {
            for (int x = 0; x < m_height; x++)
            {
                GameObject go = Instantiate(m_hexPrefab, new Vector3((z * (Mathf.Sqrt(3.0f) * m_hexSize)) + ((Mathf.Sqrt(3.0f) * m_hexSize * 0.5f) * (x % 2)), x * 1.5f * -m_hexSize, 0.0f), Quaternion.identity, transform);
                m_createdHexes[z, x] = go.GetComponent<HexTile>();
                m_createdHexes[z, x].transform.localPosition = m_createdHexes[z, x].transform.position;
                //m_createdHexes[z, x].GetComponentInChildren<TextMesh>().text = (z + "\n" + (-z - x) + "\n" + x);
                m_createdHexes[z, x].GetComponentInChildren<TextMesh>().text = ("(" + (z - ((int)(x * 0.5f))) + ", " + x + ")");
            }
        }
    }

    hexIndex GetHexIndex(Vector3 pos)
    {
        int _y = Mathf.RoundToInt(pos.y / (1.5f * -m_hexSize));
        return new hexIndex(Mathf.RoundToInt((pos.x / (Mathf.Sqrt(3.0f) * m_hexSize)) - (((Mathf.Sqrt(3.0f) * m_hexSize * 0.5f) * (_y % 2)) / (Mathf.Sqrt(3.0f) * m_hexSize))), _y);
    }
}
