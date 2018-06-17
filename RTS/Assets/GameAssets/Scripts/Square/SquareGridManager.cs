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

    public static SquareGridManager m_instance;

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

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Debug.LogError("Two Square grid amangers detected in scene.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        m_createdTiles = new SquareTile[m_width, m_height];
        UpdateGrid();
    }

    public void PathTo(HexUnit unit)
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        ContactFilter2D contactFilter = new ContactFilter2D();
        RaycastHit2D[] results = new RaycastHit2D[3];

        if (0 < Physics2D.Raycast(mousePos, Vector3.forward * 100.0f, contactFilter, results, m_TileMask))
        {
            squareIndex index = GetTileIndex(results[0].collider.gameObject.transform.localPosition);

            squareIndex[] Path = PathBetweenTiles(GetTileIndex(transform.InverseTransformPoint(unit.transform.position)), index);

            List<Vector3> movementPath = new List<Vector3>();
            for (int x = 0; x < Path.Length; x++)
            {
                movementPath.Add(GetTilePosition(Path[x]));
            }

            movementPath.Reverse();

            unit.m_path = movementPath;

            for (int x = 0; x < movementPath.Count - 1; x++)
            {
                Debug.DrawLine(movementPath[x], movementPath[x + 1], Color.red);
            }
        }
    }

    public void PathTo(List<HexUnit> units)
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        ContactFilter2D contactFilter = new ContactFilter2D();
        RaycastHit2D[] results = new RaycastHit2D[3];

        if (0 < Physics2D.Raycast(mousePos, Vector3.forward * 100.0f, contactFilter, results, m_TileMask))
        {
            for (int z = 0; z < units.Count; z++)
            {
                squareIndex index = GetTileIndex(results[0].collider.gameObject.transform.localPosition);

                squareIndex[] Path = PathBetweenTiles(GetTileIndex(transform.InverseTransformPoint(units[z].transform.position)), index);

                //Debug.Log("---Path Start: " + GetTileIndex(transform.InverseTransformPoint(m_selectedUnit.transform.position)).q + ", " + GetTileIndex(transform.InverseTransformPoint(m_selectedUnit.transform.position)).r);

                List<Vector3> movementPath = new List<Vector3>();
                for (int x = 0; x < Path.Length; x++)
                {
                    //Debug.Log("Path: " + Path[z].q + ", " + Path[z].r);

                    movementPath.Add(GetTilePosition(Path[x]));
                }

                movementPath.Reverse();

                units[z].m_path = movementPath;

                for (int x = 0; x < movementPath.Count - 1; x++)
                {
                    Debug.DrawLine(movementPath[x], movementPath[x + 1], Color.red);
                }

                //Debug.Log("---Path End: " + index.q + ", " + index.r);
            }
        }
    }

    public void PathTo(List<HexUnit> units, Vector3 targate)
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        ContactFilter2D contactFilter = new ContactFilter2D();
        RaycastHit2D[] results = new RaycastHit2D[3];

        if (0 < Physics2D.Raycast(mousePos, Vector3.forward * 100.0f, contactFilter, results, m_TileMask))
        {
            for (int z = 0; z < units.Count; z++)
            {
                squareIndex index = GetTileIndex(results[0].collider.gameObject.transform.localPosition);

                squareIndex[] Path = PathBetweenTiles(GetTileIndex(transform.InverseTransformPoint(units[z].transform.position)), index);

                //Debug.Log("---Path Start: " + GetTileIndex(transform.InverseTransformPoint(m_selectedUnit.transform.position)).q + ", " + GetTileIndex(transform.InverseTransformPoint(m_selectedUnit.transform.position)).r);

                List<Vector3> movementPath = new List<Vector3>();
                for (int x = 0; x < Path.Length; x++)
                {
                    //Debug.Log("Path: " + Path[z].q + ", " + Path[z].r);

                    movementPath.Add(GetTilePosition(Path[x]));
                }

                movementPath.Reverse();

                units[z].m_path = movementPath;

                for (int x = 0; x < movementPath.Count - 1; x++)
                {
                    Debug.DrawLine(movementPath[x], movementPath[x + 1], Color.red);
                }

                //Debug.Log("---Path End: " + index.q + ", " + index.r);
            }
        }
    }

    squareIndex[] PathBetweenTiles(squareIndex start, squareIndex end)
    {
        List<squareIndex> toReturn = new List<squareIndex>();

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
                toReturn.Add(end);

                // TODO: clean this code
                ///int breakCount = (m_width * m_height) / 8;
                //while (true)
                while (toReturn[toReturn.Count - 1] != start)
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

                    ///breakCount--;
                    ///if (breakCount <= 0)
                    ///{
                    ///    Debug.LogWarning("Break count was excided.");
                    ///    break;
                    ///}

                    ///if (toReturn[toReturn.Count - 1] == start)
                    ///    break;
                }

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

        return toReturn.ToArray();
    }

    private float TileDistance(squareIndex a, squareIndex b)
    {
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
        return new squareIndex(Mathf.RoundToInt(pos.x / (m_TileSize * 2.0f)), -Mathf.RoundToInt(pos.y / (m_TileSize * 2.0f)));
    }

    squareIndex[] GetAdjacentTiles(squareIndex index, int dist)
    {
        List<squareIndex> toReturn = new List<squareIndex>();

        for (int z = -dist; z <= dist; z++)
        {
            for (int x = -dist; x <= dist; x++)
            {
                if (!(z == 0 && x == 0))
                {
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
        return transform.TransformPoint(new Vector3(hex.q * m_TileSize * 2.0f, -hex.r * m_TileSize * 2.0f, 0.0f));
    }

    public void FreeHex(Vector3 position)
    {
        squareIndex hexIn = GetTileIndex(transform.InverseTransformPoint(position));
        m_createdTiles[hexIn.q, hexIn.r].SetActive();
    }

    public void BlockHex(Vector3 position)
    {
        squareIndex hexIn = GetTileIndex(transform.InverseTransformPoint(position));
        m_createdTiles[hexIn.q, hexIn.r].SetInactive();
    }
}
