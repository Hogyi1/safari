using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoadManager : MonoBehaviour, IStructureManager
{
    // https://en.wikipedia.org/wiki/A*_search_algorithm
#warning Ezeket mindenképpen be kell állítani, ha meg van, hogy a Griden hol helyezkedik a ki és bejárat
    [SerializeField] private GameObject firstCell;
    [SerializeField] private GameObject lastCell;

    public static RoadManager Instance;

    public bool HasRoute = false;
    public event Action OnRoadRemoved;

    private Dictionary<Vector2Int, Node> Nodes = new Dictionary<Vector2Int, Node>();
    public List<Road> ActiveRoads = new List<Road>();

    private Node StartingNode;
    private Node DestinationNode;
    // Irányok amerre kapcsolódhat két út, ha akarjuk akkor az oldal irányt is belerakhatjuk
    private static readonly List<Vector2Int> directions = new()
    {
        new Vector2Int(0, 1),  // fel
        new Vector2Int(0, -1), // le
        new Vector2Int(-1, 0), // balra
        new Vector2Int(1, 0)   // jobbra
    };

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Vector2Int garagePos = PlacementManager.Instance.GetRoadCellByPosition(firstCell.transform.position);
        Vector2Int exitPos = PlacementManager.Instance.GetRoadCellByPosition(lastCell.transform.position);

        StartingNode = AddNode(garagePos, 0);
        DestinationNode = AddNode(exitPos, 1);
    }

    // Létrehozza a megadott Model réteget és eltárolja
    // Visszaadja a Model-t, hogy a fő manager tudjon vele foglalkozni
    public Structure AddStructure(BuildingData Data, int ID, Vector2Int NodePosition)
    {
        Road road = new Road(Data, ID);
        if (road == null) throw new Exception("Nem sikerült léterhozni a következőt: Water");

        ActiveRoads.Add(road);
        AddNode(NodePosition, ID);
        HasRoute = SearchForPath();

        return road;
    }

    // Törli a saját referenciáját
    public void RemoveStructure(int ID)
    {
        Road road = ActiveRoads.Find(t => t.GetID() == ID);
        if (road == null) return;

        ActiveRoads.Remove(road);
        RemoveNode(ID);

        HasRoute = SearchForPath();
        OnRoadRemoved?.Invoke();
    }

    // Beállítja a megfelelő modellhez a nézetet
    public void SetView(IPlaceable view, int ID) { }

    public List<Vector3> SearchForRandomPath()
    {
        List<Node> nodes = RandomDFS(StartingNode, DestinationNode);

        if (nodes.Count == 0) { Debug.Log("Nincsen út"); return new(); }

        return GetRoadPositions(nodes);
    }

    public List<Vector3> FindNewPath(Vector3 from, bool toExit)
    {
        Node fromNode = Nodes[PlacementManager.Instance.GetRoadCellByPosition(from)];
        Node dest = toExit ? DestinationNode : StartingNode;
        List<Node> nodes = AStar(fromNode, dest);

        if (nodes.Count == 0) { Debug.Log("Nincsen út"); return new(); }

        return GetRoadPositions(nodes);
    }

    public bool SearchForPath()
    {
        List<Node> nodes = AStar(StartingNode, DestinationNode);
        return nodes.Count > 0;
    }

    // NodeID alapján visszakeressük a View-kat
    private List<Vector3> GetRoadPositions(List<Node> nodes)
    {
        List<Vector3> positions = new();

        foreach (var node in nodes)
        {
            if (node.NodeID == 0 || node.NodeID == 1) continue;
            positions.Add(StructureManager.Instance.GetCorrespondingView(node.NodeID).GetGameObject().transform.Find("Middle").position);
        }

        return positions;
    }

    // Ha a legrövidebb utat keressük
    public List<Node> AStar(Node start, Node goal)
    {
        List<Node> openList = new List<Node> { start };
        HashSet<Node> closedList = new HashSet<Node>();

        // A g(n) számolja a kezdő távolságot, a h(n) számolja a céltól
        var gScore = new Dictionary<int, double> { [start.NodeID] = 0 };
        var fScore = new Dictionary<int, double> { [start.NodeID] = Heuristic(start, goal) };
        var cameFrom = new Dictionary<int, Node>();

        while (openList.Count > 0)
        {
            // Kinek van a legkevesebb költsége?
            var current = openList.OrderBy(node => gScore[node.NodeID] + fScore[node.NodeID]).First();

            if (current.NodeID == goal.NodeID)
            {
                return ReconstructPath(cameFrom, current);
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (var neighborId in current.Neighbors.Keys)
            {
                var neighbor = GetNodeById(neighborId);
                if (neighbor == null || closedList.Contains(neighbor)) continue;

                // Tentative gScore (current gScore + distance to neighbor)
                double tentativeGScore = gScore[current.NodeID] + current.Neighbors[neighborId];

                if (!gScore.ContainsKey(neighbor.NodeID) || tentativeGScore < gScore[neighbor.NodeID])
                {
                    // Átállítjuk a gScore-t az új-ra
                    gScore[neighbor.NodeID] = tentativeGScore;
                    fScore[neighbor.NodeID] = Heuristic(neighbor, goal);

                    // A jelenlegi Node a szomszádjának a parentje lesz
                    cameFrom[neighbor.NodeID] = current;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
        return new();
    }

    // Ha Random utat keresünk
    public List<Node> RandomDFS(Node start, Node goal)
    {
        Stack<Node> stack = new();
        HashSet<Node> visited = new();
        Dictionary<int, Node> cameFrom = new();

        stack.Push(start);

        while (stack.Count > 0)
        {
            Node current = stack.Pop();

            if (visited.Contains(current)) continue;
            visited.Add(current);

            if (current.NodeID == goal.NodeID)
            {
                return ReconstructPath(cameFrom, current);
            }

            // Véletlenszerű sorrendben bejárjuk a szomszédokat
            List<int> shuffledNeighborIds = current.Neighbors.Keys.ToList();
            shuffledNeighborIds = shuffledNeighborIds.OrderBy(_ => UnityEngine.Random.value).ToList();

            foreach (int neighborId in shuffledNeighborIds)
            {
                Node neighbor = GetNodeById(neighborId);
                if (neighbor == null || visited.Contains(neighbor)) continue;

                cameFrom[neighbor.NodeID] = current;
                stack.Push(neighbor);
            }
        }

        return new();
    }

    // Felépítjük visszafele az utat
    public List<Node> ReconstructPath(Dictionary<int, Node> parentMap, Node current)
    {
        var path = new List<Node> { current };

        while (parentMap.ContainsKey(current.NodeID))
        {
            current = parentMap[current.NodeID];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }


    // ID alapján megkeressük a Node-ot
    public Node GetNodeById(int nodeId)
    {
        return Nodes.Values.FirstOrDefault(node => node.NodeID == nodeId);
    }

    // A heurisztikus távolság két Node között
    // G(n) és H(n) költségek kiszámítása
    private double Heuristic(Node current, Node goal)
    {
        return Math.Abs(current.Position.x - goal.Position.x) + Math.Abs(current.Position.y - goal.Position.y);
    }

    // Hozzáadjuk a Dictionarybe a Node-ot, 
    // majd beállítjuk a szomszédjait illetve önmagát
    private Node AddNode(Vector2Int position, int ID)
    {
        if (Nodes.ContainsKey(position)) { Debug.LogWarning(" A pozíció foglalt: " + position); return null; }

        Node newNode = new Node(ID, position);

        Nodes.Add(position, newNode);

        List<Node> Neighbours = GetNeighbours(position);

        foreach (Node node in Neighbours)
        {
            node.AddNeighbour(ID, 1);
        }

        newNode.AddNeighbours(Neighbours);

        return newNode;
    }

    // A Dictionaryből kiszedjük a Node-ot és a 
    // szomszédjaiból töröljük őt.
    private void RemoveNode(int ID)
    {
        if (GetNodeById(ID).IsUnityNull()) return;
        Node removeNode = GetNodeById(ID);

        foreach (var node in removeNode.Neighbors)
        {
            int neighbourID = node.Key;
            Node neighbour = GetNodeById(neighbourID);

            neighbour.RemoveNeighbour(ID);
        }
        Nodes.Remove(removeNode.Position);
    }

    // Visszaadjuk a szomszédos Node-okat egy GridPosition alapján
    private List<Node> GetNeighbours(Vector2Int position)
    {
        List<Node> nodes = new();

        foreach (var dir in directions)
        {
            Vector2Int neighbourPosition = position + dir;

            if (Nodes.TryGetValue(neighbourPosition, out Node neighbour)) nodes.Add(neighbour);
        }

        return nodes;
    }

    // Kezdetben minden Weight 1, eszerint randomizáljuk az út keresést
    public class Node
    {
        public int NodeID; // Ez alapján keressük meg a View-t is ami a ROADVIEW egy külön objektum
        public Vector2Int Position; // A griden a pozicio
        public Dictionary<int, double> Neighbors = new(); // NodeID, Weight
        public Node(int nodeID, Vector2Int gridPosition)
        {
            NodeID = nodeID;
            Position = gridPosition;
        }

        public void AddNeighbour(int ID, double weight)
        {
            Neighbors[ID] = weight;
        }
        public void RemoveNeighbour(int ID)
        {
            Neighbors.Remove(ID);
        }

        public void AddNeighbours(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                AddNeighbour(node.NodeID, 1);
            }
        }
    }
}