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

    #region hexIndex
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

        public static bool operator ==(hexIndex i1, hexIndex i2)
        {
            return (i1.q == i2.q) && (i1.r == i2.r);
        }

        public static bool operator !=(hexIndex i1, hexIndex i2)
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
            SetHexInactive();
        }
        else if (Input.GetButtonDown("Fire3"))
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

            m_createdHexes[index.q, index.r].StartPath();
            m_selectedHexIndex = index;
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

            m_createdHexes[index.q, index.r].EndPath();

            hexIndex[] Path = PathBetweenHexes(m_selectedHexIndex, index);

            Debug.Log("---Path Start: " + m_selectedHexIndex.q + ", " + m_selectedHexIndex.r);

            for (int z = 0; z < Path.Length; z++)
            {
                Debug.Log("Path: " + Path[z].q + ", " + Path[z].r);

                m_createdHexes[Path[z].q, Path[z].r].OnPath();
            }

            Debug.Log("---Path End: " + index.q + ", " + index.r);

            /*List<hexIndex> frontier = new List<hexIndex>();
            frontier.Add(m_selectedHexIndex);
            ICollection<KeyValuePair<hexIndex, hexIndex>> came_from = new Dictionary<hexIndex, hexIndex>();
            ICollection<KeyValuePair<hexIndex, int>> cost_so_far = new Dictionary<hexIndex, int>();
            //came_from[start] = None;
            came_from.Add(new KeyValuePair<hexIndex, hexIndex>(m_selectedHexIndex, m_selectedHexIndex));
            //cost_so_far[start] = 0;
            cost_so_far.Add(new KeyValuePair<hexIndex, int>(m_selectedHexIndex, 0));

            while (frontier.Count > 0)
            {
                //hexIndex current = frontier.get();
                hexIndex current = frontier[0];
                //frontier.Remove(current);

                if (current == m_selectedHexIndex)
                    break;


                foreach (hexIndex next in GetAdjacentHexes(current, 1))
                {
                    //int new_cost = cost_so_far[current] + graph.cost(current, next);
                    int new_cost = cost_so_far.[current];

                    //if (next not in cost_so_far || new_cost < cost_so_far[next])
                    if (DictContains(cost_so_far, next) || new_cost < GetIntVal(cost_so_far, next))
                    {
                        //cost_so_far[next] = new_cost;
                        //priority = new_cost + heuristic(goal, next);
                        //frontier.put(next, priority);
                        //came_from[next] = current;

                        cost_so_far.Add(new KeyValuePair<hexIndex, int>(next, new_cost));
                        int priority = new_cost + HexDistance(index, next);
                        frontier.put(next, priority);
                        came_from[next] = current;
                    }
                }
            }*/
        }

        m_selectedHexIndex.set(-1, -1);
    }

    hexIndex[] PathBetweenHexes(hexIndex start, hexIndex end)
    {
        Dictionary<hexIndex, hexIndex> cameFrom = new Dictionary<hexIndex, hexIndex>();
        Dictionary<hexIndex, double> costSoFar = new Dictionary<hexIndex, double>();

        var frontier = new PriorityQueue<hexIndex>();
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current.Equals(end))
            {
                break;
            }

            foreach (var next in GetAdjacentHexes(current, 1))
            {
                double newCost = costSoFar[current];

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    double priority = newCost + HexDistance(next, end);
                    frontier.Enqueue(next, priority);

                    //if (!cameFrom.ContainsKey(next) && !cameFrom.ContainsValue(current))
                    cameFrom[next] = current;
                }
            }
        }

        List<hexIndex> toReturn = new List<hexIndex>();

        /*foreach (hexIndex index in cameFrom.Values)
        {
            toReturn.Add(index);
        }*/

        toReturn.Add(end);

        while (true)
        {
            for (int z = 0; z < cameFrom.Count; z++)
            {
                hexIndex toAdd;
                if (cameFrom.TryGetValue(toReturn[toReturn.Count - 1], out toAdd))
                {
                    toReturn.Add(toAdd);
                    break;
                }
            }

            Debug.Log(toReturn[0].q + ", " + toReturn[0].r);
            
            if (toReturn[toReturn.Count - 1] == start)
                break;
        }

        toReturn.RemoveAt(0);
        toReturn.RemoveAt(toReturn.Count - 1);

        return toReturn.ToArray();
    }

    private int HexDistance(hexIndex a, hexIndex b)
    {
        Vector3Int aCube = a.CubeCoordinates();
        Vector3Int bCube = b.CubeCoordinates();
        return Mathf.Max(Mathf.Abs(aCube.x - bCube.x), Mathf.Abs(aCube.y - bCube.y), Mathf.Abs(aCube.z - bCube.z));
    }

    private void SetHexInactive()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        ContactFilter2D contactFilter = new ContactFilter2D();
        RaycastHit2D[] results = new RaycastHit2D[3];

        if (0 < Physics2D.Raycast(mousePos, Vector3.forward * 100.0f, contactFilter, results, m_hexMask))
        {
            hexIndex index = GetHexIndex(results[0].collider.gameObject.transform.localPosition);

            m_createdHexes[index.q, index.r].SetInactive();
        }
    }

    private void ResetHexPaths()
    {
        m_selectedHexIndex.set(-1, -1);

        for (int z = 0; z < m_width; z++)
        {
            for (int x = 0; x < m_height; x++)
            {
                //m_createdHexes[z, x].DeSelect();
                m_createdHexes[z, x].SetActive();
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

                Vector3Int cubeHex = new hexIndex(z, x).CubeCoordinates();

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

        for (int z = 0; z < AdjacentCoordenatesCount; z++)
        {
            Vector3Int cubeHex = index.CubeCoordinates();
            cubeHex.x += AdjacentCoordenates[z, 0];
            cubeHex.z += AdjacentCoordenates[z, 1];

            Debug.Log("### " + AdjacentCoordenates[z, 0] + ", " + AdjacentCoordenates[z, 1] + " : " + cubeHex.x + ", " + cubeHex.y);
            Debug.Log("###### " + AdjacentCoordenates[z, 0] + ", " + AdjacentCoordenates[z, 1] + " : " + CubeToHexIndex(cubeHex).q + ", " + CubeToHexIndex(cubeHex).r);
            toReturn[z] = CubeToHexIndex(cubeHex);
        }

        return toReturn;
    }

    hexIndex[] GetAdjacentHexes(hexIndex index, int dist)
    {
        List<hexIndex> toReturn = new List<hexIndex>();

        for (int z = -dist; z <= dist; z++)
        {
            for (int x = -dist; x <= dist; x++)
            {
                if (!(z == 0 && x == 0) && (Mathf.Abs(z + x) <= dist))
                {
                    Vector3Int cubeHex = index.CubeCoordinates();
                    cubeHex.x += z;
                    cubeHex.z += x;
                    
                    hexIndex hex = CubeToHexIndex(cubeHex);
                    if (hex.q >= 0 && hex.r >= 0 && hex.q < m_width && hex.r < m_height)
                    {
                        if (m_createdHexes[hex.q, hex.r].isActive())
                            toReturn.Add(hex);
                    }

                    //toReturn.Add(CubeToHexIndex(cubeHex));
                }
            }
        }

        return toReturn.ToArray();
    }

    hexIndex CubeToHexIndex(Vector3Int cube)
    {
        return new hexIndex(((int)(cube.z * 0.5f)) + cube.x, cube.z);
    }

    bool DictContains(ICollection<KeyValuePair<hexIndex, hexIndex>> dict, hexIndex Val)
    {
        foreach (KeyValuePair<hexIndex, hexIndex> KV in dict)
        {
            if (KV.Key == Val)
            {
                return true;
            }
        }

        return false;
    }

    bool DictContains(ICollection<KeyValuePair<hexIndex, int>> dict, hexIndex Val)
    {
        foreach (KeyValuePair<hexIndex, int> KV in dict)
        {
            if (KV.Key == Val)
            {
                return true;
            }
        }

        return false;
    }

    hexIndex GetHexVal(ICollection<KeyValuePair<hexIndex, hexIndex>> dict, hexIndex Val)
    {
        foreach (KeyValuePair<hexIndex, hexIndex> KV in dict)
        {
            if (KV.Key == Val)
            {
                return KV.Value;
            }
        }

        return new hexIndex(-1, -1);
    }

    int GetIntVal(ICollection<KeyValuePair<hexIndex, int>> dict, hexIndex Val)
    {
        foreach (KeyValuePair<hexIndex, int> KV in dict)
        {
            if (KV.Key == Val)
            {
                return KV.Value;
            }
        }

        return -1;
    }
}
