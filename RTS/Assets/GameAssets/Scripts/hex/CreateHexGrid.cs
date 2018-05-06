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

    private GameObject[,] m_createdHexes;
    private SpriteRenderer[,] m_createdHexSprites;

    [SerializeField]
    private E_HexType m_hexType;

    [SerializeField]
    private int m_width = 5;

    [SerializeField]
    private int m_height = 5;

    [SerializeField]
    private float m_hexSize = 0.5f;

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
        m_createdHexes = new GameObject[m_width, m_height];
        m_createdHexSprites = new SpriteRenderer[m_width, m_height];
        UpdateGrid();

        xHexPixelSize = Mathf.RoundToInt(m_hexSize * (xScreenSize / m_cam.pixelWidth));
        yHexPixelSize = Mathf.RoundToInt(m_hexSize * (yScreenSize / m_cam.pixelHeight));

        pixelToUU = yScreenSize / m_cam.pixelHeight;
        UUToPixel = m_cam.pixelHeight / yScreenSize;
    }

    void Update()
    {
        Layout pointy = new Layout(Layout.pointy, new Point(42, 63), new Point(0, 0));
        FractionalHex hex = pointy.PixelToHex(new Point(Input.mousePosition.x, Input.mousePosition.y));
        Debug.Log(hex.q + " : " + hex.r + " : " + hex.s);

        if (Input.GetButtonDown("Fire1"))
        {
            StartHexPathing(m_cam.ScreenToWorldPoint(Input.mousePosition) * pixelToUU /*- new Vector3(0.5f * xScreenSize, 0.5f * yScreenSize)*/);
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            ResetHexPaths();
        }
    }

    private void StartHexPathing(Vector2 pos)
    {



        // convert unity position to hex
        float x = (Mathf.Sqrt(3) / 3.0f * pos.x - 1.0f / 3.0f * pos.y) / m_hexSize;
        float y =                                (2.0f / 3.0f * pos.y) / m_hexSize;
        
        //float rx = Mathf.Round(x);
        //float ry = Mathf.Round(y);

        //Vector3 roundedHex = cubeRound(x, -x - y, y);
        //Vector3 roundedHex = cubeRound(x, -y, -x+y);
        Vector3 roundedHex = new Vector3(x, y, 0.0f);

        ///roundedHex.x += Mathf.Round(m_width * 0.5f);
        ///roundedHex.y += Mathf.Round(m_height * 0.5f);
        //roundedHex.y = m_height - roundedHex.y;

        HexUtils.Hex selectedHex = HexUtils.HexUtils.pixel_to_pointy_hex(roundedHex);
        //selectedHex.q = m_height - selectedHex.q;
        Debug.Log("q: " + selectedHex.q + ", r: " + selectedHex.r);

        m_createdHexSprites[(int)selectedHex.q, (int)selectedHex.r].color = Color.red;
        m_createdHexSprites[(int)selectedHex.r, (int)selectedHex.q].color = Color.yellow;

        //Debug.Log("x: " + (Mathf.Sqrt(3) / 3 * pos.x - 1.0f / 3.0f * pos.y) + "y: " + (2.0f / 3.0f * pos.y));
    }

    private void ResetHexPaths()
    {
        m_selectedHexIndex.set(-1, -1);

        for (int z = 0; z < m_width; z++)
        {
            for (int x = 0; x < m_height; x++)
            {
                m_createdHexSprites[z, x].color = Color.white;
            }
        }
    }

    void UpdateGrid()
    {
        for (int z = 0; z < m_width; z++)
        {
            for (int x = 0; x < m_height; x++)
            {
                m_createdHexes[z, x] = Instantiate(m_hexPrefab, new Vector3((z * (Mathf.Sqrt(3.0f) * m_hexSize)) + ((Mathf.Sqrt(3.0f) * m_hexSize * 0.5f) * (x % 2)), x * 1.5f * -m_hexSize, 0.0f), Quaternion.identity, transform);
                m_createdHexes[z, x].transform.localPosition = m_createdHexes[z, x].transform.position;
                m_createdHexes[z, x].GetComponentInChildren<TextMesh>().text = ("(" + z + ", " + x + ")");

                m_createdHexSprites[z, x] = m_createdHexes[z, x].GetComponent<SpriteRenderer>();
            }
        }
    }
}
