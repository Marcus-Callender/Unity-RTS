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

        public void set(int _x, int _y)
        {
            q = _x;
            r = _y;
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
            StartHexPathing();
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            ResetHexPaths();
        }
    }

    private void StartHexPathing()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        Ray click = m_cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        ContactFilter2D contactFilter = new ContactFilter2D();
        RaycastHit2D[] results = new RaycastHit2D[3];
        
        if (0 < Physics2D.Raycast(mousePos, Vector3.forward * 100.0f, contactFilter, results, m_hexMask))
        {
            results[0].collider.gameObject.GetComponent<HexTile>().Selected();
        }
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
                m_createdHexes[z, x].GetComponentInChildren<TextMesh>().text = ("(" + z + ", " + x + ")");
            }
        }
    }
}
