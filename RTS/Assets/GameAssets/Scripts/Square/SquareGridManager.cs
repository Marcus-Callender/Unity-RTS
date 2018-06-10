using System;
using System.Collections.Generic;
using UnityEngine;

public class SquareGridManager : MonoBehaviour
{
    #region squareIndex
    [Serializable]
    public struct squareIndex
    {
        public int q;
        public int r;

        public squareIndex(int _q, int _r)
        {
            q = _q;
            r = _r;
        }

        public void set(int _q, int _r)
        {
            q = _q;
            r = _r;
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

        public static bool operator ==(squareIndex i1, squareIndex i2)
        {
            return (i1.q == i2.q) && (i1.r == i2.r);
        }

        public static bool operator !=(squareIndex i1, squareIndex i2)
        {
            return (i1.q != i2.q) || (i1.r != i2.r);
        }
    }
    #endregion

    #region Tuple
    // https://answers.unity.com/questions/381993/does-unity-4-mono-support-tuples.html
    public class Tuple<T1, T2>
    {
        public T1 First { get; private set; }
        public T2 Second { get; private set; }
        internal Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            var tuple = new Tuple<T1, T2>(first, second);
            return tuple;
        }
    }
    #endregion

    #region PriorityQueue
    // https://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
    public class PriorityQueue<T>
    {
        // I'm using an unsorted array for this example, but ideally this
        // would be a binary heap. There's an open issue for adding a binary
        // heap to the standard C# library: https://github.com/dotnet/corefx/issues/574
        //
        // Until then, find a binary heap class:
        // * https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
        // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
        // * http://xfleury.github.io/graphsearch.html
        // * http://stackoverflow.com/questions/102398/priority-queue-in-net

        private List<Tuple<T, double>> elements = new List<Tuple<T, double>>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(T item, double priority)
        {
            //elements.Add(Tuple.Create(item, priority));
            elements.Add(new Tuple<T, double>(item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Second < elements[bestIndex].Second)
                {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].First;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
    #endregion

    [SerializeField]
    private Camera m_cam;

    [SerializeField]
    private GameObject m_TilePrefab;

    private SquareTile[,] m_createdTiles;

    [SerializeField]
    private int m_width = 5;

    [SerializeField]
    private int m_height = 5;

    [SerializeField]
    private float m_TileSize = 0.5f;

    [SerializeField]
    private LayerMask m_unitMask;

    [SerializeField]
    private LayerMask m_TileMask;
    private HexUnit m_selectedUnit = null;

    void Start()
    {
        m_createdTiles = new SquareTile[m_width, m_height];
        UpdateGrid();
    }

    void Update()
    {
        Debug.DrawRay(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector3.forward * 11, Color.green);

        if (Input.GetButtonDown("Fire1"))
        {
            if (m_selectedUnit == null)
            {
                StartPathing();
            }
            else
            {
                PathTo();
            }
        }
    }

    private void StartPathing()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit result;
        Physics.Raycast(mousePos, Vector3.forward, out result, 100.0f);

        if (result.collider != null)
        {
            squareIndex index = GetTileIndex(transform.InverseTransformPoint(result.collider.gameObject.transform.position));
            
            m_selectedUnit = result.collider.gameObject.GetComponent<HexUnit>();
        }
    }

    private void PathTo()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        ContactFilter2D contactFilter = new ContactFilter2D();
        RaycastHit2D[] results = new RaycastHit2D[3];

        if (0 < Physics2D.Raycast(mousePos, Vector3.forward * 100.0f, contactFilter, results, m_TileMask))
        {
            squareIndex index = GetTileIndex(results[0].collider.gameObject.transform.localPosition);
            
            squareIndex[] Path = PathBetweenTiles(GetTileIndex(transform.InverseTransformPoint(m_selectedUnit.transform.position)), index);

            Debug.Log("---Path Start: " + GetTileIndex(transform.InverseTransformPoint(m_selectedUnit.transform.position)).q + ", " + GetTileIndex(transform.InverseTransformPoint(m_selectedUnit.transform.position)).r);

            List<Vector3> movementPath = new List<Vector3>();
            for (int z = 0; z < Path.Length; z++)
            {
                Debug.Log("Path: " + Path[z].q + ", " + Path[z].r);

                movementPath.Add(GetTilePosition(Path[z]));
            }

            movementPath.Reverse();

            m_selectedUnit.m_path = movementPath;

            for (int z = 0; z < movementPath.Count - 1; z++)
            {
                Debug.DrawLine(movementPath[z], movementPath[z + 1], Color.red);
            }

            Debug.Log("---Path End: " + index.q + ", " + index.r);
        }

        m_selectedUnit = null;
    }

    squareIndex[] PathBetweenTiles(squareIndex start, squareIndex end)
    {
        var frontier = new PriorityQueue<squareIndex>();
        frontier.Enqueue(start, 0);

        Dictionary<squareIndex, squareIndex> cameFrom = new Dictionary<squareIndex, squareIndex>();
        Dictionary<squareIndex, double> costSoFar = new Dictionary<squareIndex, double>();

        cameFrom[start] = new squareIndex(-1, -1);
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            squareIndex current = frontier.Dequeue();

            if (current.Equals(end))
            {
                break;
            }

            foreach (squareIndex next in GetAdjacentTiles(current, 1))
            {
                double newCost = costSoFar[current];

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    double priority = newCost + TileDistance(end, next);
                    frontier.Enqueue(next, priority);

                    //if (!cameFrom.ContainsKey(next) && !cameFrom.ContainsValue(current))
                    cameFrom[next] = current;
                }
            }
        }

        List<squareIndex> toReturn = new List<squareIndex>();

        toReturn.Add(end);

        // TODO: clean this code
        while (true)
        {
            for (int z = 0; z < cameFrom.Count; z++)
            {
                squareIndex toAdd;
                if (cameFrom.TryGetValue(toReturn[toReturn.Count - 1], out toAdd))
                {
                    toReturn.Add(toAdd);
                    break;
                }
            }
            
            if (toReturn[toReturn.Count - 1] == start)
                break;
        }
        
        return toReturn.ToArray();
    }

    private float/*int*/ TileDistance(squareIndex a, squareIndex b)
    {
        //Vector3Int aCube = a.CubeCoordinates();
        //Vector3Int bCube = b.CubeCoordinates();
        //return Mathf.Max(Mathf.Abs(aCube.x - bCube.x), Mathf.Abs(aCube.y - bCube.y), Mathf.Abs(aCube.z - bCube.z));
        ///return (/*Mathf.Abs*/(Mathf.Sqrt(Mathf.Pow((a.q - b.q), 2) + Mathf.Pow((a.r - b.r), 2))));
        return Mathf.Sqrt(Mathf.Pow((a.q - b.q), 2) + Mathf.Pow((a.r - b.r), 2));
    }

    private void SetTileInactive()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        ContactFilter2D contactFilter = new ContactFilter2D();
        RaycastHit2D[] results = new RaycastHit2D[3];

        if (0 < Physics2D.Raycast(mousePos, Vector3.forward * 100.0f, contactFilter, results, m_TileMask))
        {
            squareIndex index = GetTileIndex(results[0].collider.gameObject.transform.localPosition);

            m_createdTiles[index.q, index.r].SetInactive();
        }
    }

    private void ResetPaths()
    {
        m_selectedUnit = null;

        for (int z = 0; z < m_width; z++)
        {
            for (int x = 0; x < m_height; x++)
            {
                m_createdTiles[z, x].SetActive();
            }
        }
    }

    void UpdateGrid()
    {
        for (int z = 0; z < m_width; z++)
        {
            for (int x = 0; x < m_height; x++)
            {
                Vector3 pos = new Vector3(z * 2.0f * m_TileSize, -x * 2.0f * m_TileSize, 0.0f);

                GameObject go = Instantiate(m_TilePrefab, pos, Quaternion.identity, transform);
                m_createdTiles[z, x] = go.GetComponent<SquareTile>();
                m_createdTiles[z, x].transform.localPosition = m_createdTiles[z, x].transform.position;
                
                if (Physics.Raycast(transform.TransformPoint(pos) + (Vector3.back * 20.0f), Vector3.forward * 30.0f))
                {
                    m_createdTiles[z, x].SetInactive();
                }
                else
                {
                    m_createdTiles[z, x].SetActive();
                }
            }
        }
    }

    squareIndex GetTileIndex(Vector3 pos)
    {
        //int _y = Mathf.RoundToInt(pos.y / (1.5f * -m_hexSize));
        //return new squareIndex(Mathf.RoundToInt((pos.x / (Mathf.Sqrt(3.0f) * m_hexSize)) - (((Mathf.Sqrt(3.0f) * m_hexSize * 0.5f) * (_y % 2)) / (Mathf.Sqrt(3.0f) * m_hexSize))), _y);
        return new squareIndex(Mathf.RoundToInt(pos.x / (m_TileSize * 2.0f)), -Mathf.RoundToInt(pos.y / (m_TileSize * 2.0f)));
    }

    squareIndex[] GetAdjacentTiles(squareIndex index, int dist)
    {
        List<squareIndex> toReturn = new List<squareIndex>();

        for (int z = -dist; z <= dist; z++)
        {
            for (int x = -dist; x <= dist; x++)
            {
                //if (!(z == 0 && x == 0) && (Mathf.Abs(z + x) <= dist))
                if (!(z == 0 && x == 0))
                {
                    /*Vector3Int cubeHex = index.CubeCoordinates();
                    cubeHex.x += z;
                    cubeHex.z += x;

                    squareIndex hex = CubeToHexIndex(cubeHex);
                    if (hex.q >= 0 && hex.r >= 0 && hex.q < m_width && hex.r < m_height)
                    {
                        if (m_createdTiles[hex.q, hex.r].isActive())
                            toReturn.Add(hex);
                    }*/

                    squareIndex newIndex = new squareIndex(index.q + z, index.r + x);

                    if (newIndex.q >= 0 && newIndex.r >= 0 && newIndex.q < m_width && newIndex.r < m_height)
                    {
                        if (m_createdTiles[newIndex.q, newIndex.r].isActive())
                            toReturn.Add(newIndex);
                    }
                }
            }
        }

        return toReturn.ToArray();
    }

    /*squareIndex CubeToHexIndex(Vector3Int cube)
    {
        return new squareIndex(((int)(cube.z * 0.5f)) + cube.x, cube.z);
    }*/

    bool DictContains(ICollection<KeyValuePair<squareIndex, squareIndex>> dict, squareIndex Val)
    {
        foreach (KeyValuePair<squareIndex, squareIndex> KV in dict)
        {
            if (KV.Key == Val)
            {
                return true;
            }
        }

        return false;
    }

    bool DictContains(ICollection<KeyValuePair<squareIndex, int>> dict, squareIndex Val)
    {
        foreach (KeyValuePair<squareIndex, int> KV in dict)
        {
            if (KV.Key == Val)
            {
                return true;
            }
        }

        return false;
    }

    squareIndex GetTileVal(ICollection<KeyValuePair<squareIndex, squareIndex>> dict, squareIndex Val)
    {
        foreach (KeyValuePair<squareIndex, squareIndex> KV in dict)
        {
            if (KV.Key == Val)
            {
                return KV.Value;
            }
        }

        return new squareIndex(-1, -1);
    }

    int GetIntVal(ICollection<KeyValuePair<squareIndex, int>> dict, squareIndex Val)
    {
        foreach (KeyValuePair<squareIndex, int> KV in dict)
        {
            if (KV.Key == Val)
            {
                return KV.Value;
            }
        }

        return -1;
    }

    Vector3 GetTilePosition(squareIndex hex)
    {
        //return transform.TransformPoint(new Vector3((hex.q * (Mathf.Sqrt(3.0f) * m_hexSize)) + ((Mathf.Sqrt(3.0f) * m_hexSize * 0.5f) * (hex.r % 2)), hex.r * 1.5f * -m_hexSize, 0.0f));
        return transform.TransformPoint(new Vector3(hex.q * m_TileSize * 2.0f, -hex.r * m_TileSize * 2.0f , 0.0f));
    }
}
