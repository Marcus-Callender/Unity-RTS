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

        public hexIndex(int _q, int _r)
        {
            q = _q;
            r = _r;
        }

        public void set(int _q, int _r)
        {
            q = _q;
            r = _r;
        }

        public Vector3Int CubeCoordinates()
        {
            int adjustedQ = q - ((int)(r * 0.5f));
            return new Vector3Int(adjustedQ, (-adjustedQ - r), r);
        }

        public bool isNull()
        {
            return q < 0 || r < 0;
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

    private static int AdjacentCoordenatesCount = 6;
    private static int[,] AdjacentCoordenates = { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };

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
            Vector3Int cubeIndex = index.CubeCoordinates();
            //Debug.Log("Index: " + index.q + ", " + index.r);

            m_createdHexes[index.q, index.r].Selected();
            m_selectedHexIndex = index;

            hexIndex[] m_adjesentHexes = GetAdjacentHexes(index, 3);
            for (int z = 0; z < m_adjesentHexes.Length/*AdjacentCoordenatesCount*/; z++)
            {
                if (!m_adjesentHexes[z].isNull() && (m_adjesentHexes[z].q < m_width && m_adjesentHexes[z].r < m_height))
                {
                    hexIndex hex = m_adjesentHexes[z];
                    Vector3Int cubeHex = hex.CubeCoordinates();
                    //m_createdHexes[(index.q /*- (((index.r % 2) * 2) + 2 == hex.r ? 1 : 0 /*== 1 ? 1 : 0*#/)*/ /*- ((int)(hex.r * 0.5f))*/), hex.r].Selected(Mathf.Abs(AdjacentCoordenates[z, 0]), Mathf.Abs(AdjacentCoordenates[z, 1]), Mathf.Abs(-AdjacentCoordenates[z, 0] - AdjacentCoordenates[z, 1]));
                    //m_createdHexes[(hex.q + ((index.r + (hex.r % 2 == 1 ? 1 : -1) == hex.r) ? 0 : 1)), hex.r].Selected(Mathf.Abs(AdjacentCoordenates[z, 0]), Mathf.Abs(AdjacentCoordenates[z, 1]), Mathf.Abs(-AdjacentCoordenates[z, 0] - AdjacentCoordenates[z, 1]));

                    /*if (index.r % 2 == 1 && hex.r == index.r + 1)
                    {
                        m_createdHexes[hex.q + 1, hex.r].Selected(Mathf.Abs(AdjacentCoordenates[z, 0]), Mathf.Abs(AdjacentCoordenates[z, 1]), Mathf.Abs(-AdjacentCoordenates[z, 0] - AdjacentCoordenates[z, 1]));
                    }
                    else if (index.r % 2 == 0 && hex.r == index.r - 1)
                    {
                        m_createdHexes[hex.q - 1, hex.r].Selected(Mathf.Abs(AdjacentCoordenates[z, 0]), Mathf.Abs(AdjacentCoordenates[z, 1]), Mathf.Abs(-AdjacentCoordenates[z, 0] - AdjacentCoordenates[z, 1]));
                    }
                    else
                    {
                        m_createdHexes[hex.q, hex.r].Selected(Mathf.Abs(AdjacentCoordenates[z, 0]), Mathf.Abs(AdjacentCoordenates[z, 1]), Mathf.Abs(-AdjacentCoordenates[z, 0] - AdjacentCoordenates[z, 1]));
                    }*/

                    //Debug.Log("### " + AdjacentCoordenates[z, 0] + ", " + AdjacentCoordenates[z, 1] + " : " + hex.q + ", " + hex.r);
                    //m_createdHexes[hex.q + (index.r % 2 == 1 ? 1 : 0) + ((hex.r == index.r + 1 || hex.r == index.r) ? 1 : 0) - 1, hex.r].Selected(Mathf.Abs(AdjacentCoordenates[z, 0]), Mathf.Abs(AdjacentCoordenates[z, 1]), Mathf.Abs(-AdjacentCoordenates[z, 0] - AdjacentCoordenates[z, 1]));

                    //m_createdHexes[hex.q, hex.r].Selected(Mathf.Abs(AdjacentCoordenates[z, 0]), Mathf.Abs(AdjacentCoordenates[z, 1]), Mathf.Abs(-AdjacentCoordenates[z, 0] - AdjacentCoordenates[z, 1]));
                    //m_createdHexes[hex.q, hex.r].Selected(Mathf.Abs(index.q - hex.q), Mathf.Abs(index.r - hex.r), Mathf.Abs(-(index.q - hex.q) - (index.r - hex.r)));

                    //m_createdHexes[hex.q, hex.r].Selected(Mathf.Abs(index.q - hex.q), 0.0f, 0.0f);
                    //m_createdHexes[hex.q, hex.r].Selected(0.0f, Mathf.Abs(index.r - hex.r), 0.0f);
                    //m_createdHexes[hex.q, hex.r].Selected(0.0f, 0.0f, Mathf.Abs(-(index.q - hex.q) - (index.r - hex.r)));

                    //Debug.Log(Mathf.Abs(index.q - hex.q) + ", " + Mathf.Abs(index.r - hex.r) + ", " + Mathf.Abs(-(index.q - hex.q) - (index.r - hex.r)));
                    m_createdHexes[hex.q, hex.r].Selected(Mathf.Abs(cubeIndex.x - cubeHex.x), Mathf.Abs(cubeIndex.z - cubeHex.z), Mathf.Abs(cubeIndex.y - cubeHex.y));
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
            ///Debug.Log("Index: " + index.q + ", " + index.r);
            ///Debug.Log("Distance: " + HexDistance(m_selectedHexIndex, index));

            m_createdHexes[index.q, index.r].Selected();
            /*for (int z = 0; z < m_width; z++)
            {
                for (int x = 0; x < m_height; x++)
                {
                    m_createdHexes[z, x].SetText(HexDistance(index, GetHexIndex(m_createdHexes[z, x].transform.localPosition)).ToString());
                }
            }*/
        }

        m_selectedHexIndex.set(-1, -1);
    }

    private int HexDistance(hexIndex a, hexIndex b)
    {
        Vector3Int aCube = a.CubeCoordinates();
        Vector3Int bCube = b.CubeCoordinates();
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
                //m_createdHexes[z, x].GetComponentInChildren<TextMesh>().text = ("(" + (z - ((int)(x * 0.5f))) + ", " + x + ")");

                //m_createdHexes[z, x].SetText("(" + (z - ((int)(x * 0.5f))) + ", " + x + ")");
                //m_createdHexes[z, x].SetText("(" + z + ", " + x + ")");

                Vector3Int cubeHex = new hexIndex(z, x).CubeCoordinates();
                //Debug.Log("hex: " + z + ", " + x);
                //Debug.Log("hex: " + CubeToHexIndex(cubeHex).q + ", " + CubeToHexIndex(cubeHex).r);
                //Debug.Log("------");
                m_createdHexes[z, x].SetText("(" + cubeHex.x + ",\n" + cubeHex.y + ",\n" + cubeHex.z + ")");
            }
        }
    }

    hexIndex GetHexIndex(Vector3 pos)
    {
        int _y = Mathf.RoundToInt(pos.y / (1.5f * -m_hexSize));
        return new hexIndex(Mathf.RoundToInt((pos.x / (Mathf.Sqrt(3.0f) * m_hexSize)) - (((Mathf.Sqrt(3.0f) * m_hexSize * 0.5f) * (_y % 2)) / (Mathf.Sqrt(3.0f) * m_hexSize))), _y);
    }

    hexIndex[] GetAdjacentHexes(hexIndex index)
    {
        hexIndex[] toReturn = new hexIndex[6];

        //Debug.Log(" -> " + index.q + ", " + index.r);
        Debug.Log(" -> " + index.CubeCoordinates().x + ", " + index.CubeCoordinates().y);

        for (int z = 0; z < AdjacentCoordenatesCount; z++)
        {
            //toReturn[z] = new hexIndex(index.q + AdjacentCoordenates[z, 0], index.r + AdjacentCoordenates[z, 1]);
            //hexIndex tmp = index;
            //tmp.q += AdjacentCoordenates[z, 0];
            //tmp.r += AdjacentCoordenates[z, 1];

            Vector3Int cubeHex = index.CubeCoordinates();
            cubeHex.x += AdjacentCoordenates[z, 0];
            cubeHex.z += AdjacentCoordenates[z, 1];

            Debug.Log("### " + AdjacentCoordenates[z, 0] + ", " + AdjacentCoordenates[z, 1] + " : " + cubeHex.x + ", " + cubeHex.y);
            Debug.Log("###### " + AdjacentCoordenates[z, 0] + ", " + AdjacentCoordenates[z, 1] + " : " + CubeToHexIndex(cubeHex).q + ", " + CubeToHexIndex(cubeHex).r);
            toReturn[z] = CubeToHexIndex(cubeHex/*tmp.CubeCoordinates()*/);
        }

        return toReturn;
    }
    
    hexIndex[] GetAdjacentHexes(hexIndex index, int dist)
    {
        //hexIndex[] toReturn = new hexIndex[dist * 6];
        List<hexIndex> toReturn = new List<hexIndex>();
        
        for (int z = -dist; z <= dist; z++)
        {
            for (int x = -dist; x <= dist; x++)
            {
                if (!(z == 0 && x == 0) && (Mathf.Abs(z + x) <= dist))
                {
                    Vector3Int cubeHex = index.CubeCoordinates();
                    cubeHex.x += z;//AdjacentCoordenates[z, 0];
                    cubeHex.z += x;//AdjacentCoordenates[z, 1];

                    //toReturn[z] = CubeToHexIndex(cubeHex);
                    toReturn.Add(CubeToHexIndex(cubeHex));
                }
            }
        }

        return toReturn.ToArray();
    }

    hexIndex CubeToHexIndex(Vector3Int cube)
    {
        return new hexIndex(((int)(cube.z * 0.5f)) + cube.x, cube.z);
    }
}
