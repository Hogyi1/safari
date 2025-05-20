using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoadManager : MonoBehaviour, IStructureManager
{
    // https://en.wikipedia.org/wiki/A*_search_algorithm
#warning Ezeket mindenképpen be kell állítani, ha meg van, hogy a Griden hol helyezkedik a ki és bejárat
    /// <summary>
    /// A GameObject, amely a garázs bejáratát jelöli a jelenetben.
    /// </summary>
    [SerializeField] private GameObject firstCell;

    /// <summary>
    /// A GameObject, amely az első útelem a garázs után.
    /// </summary>
    [SerializeField] private GameObject secondCell;

    /// <summary>
    /// A GameObject, amely a kijáratot jelöli a jelenetben.
    /// </summary>
    [SerializeField] private GameObject lastCell;

    /// <summary>
    /// Singleton példány a RoadManagerből.
    /// </summary>
    public static RoadManager Instance;

    /// <summary>
    /// Igaz, ha létezik érvényes útvonal a garázstól a kijáratig.
    /// </summary>
    public bool HasRoute = false;

    /// <summary>
    /// Esemény, amely akkor hívódik meg, ha egy út törlésre kerül.
    /// </summary>
    public event Action OnRoadRemoved;

    /// <summary>
    /// Minden regisztrált út csomópont pozíció és a hozzá tartozó Node.
    /// </summary>
    private Dictionary<Vector2Int, Node> Nodes = new Dictionary<Vector2Int, Node>();

    /// <summary>
    /// Az összes aktív út modell.
    /// </summary>
    public List<RoadModel> ActiveRoads = new List<RoadModel>();

    /// <summary>
    /// A garázshoz tartozó node.
    /// </summary>
    private Node GarageNode;

    /// <summary>
    /// A kijárathoz tartozó node.
    /// </summary>
    private Node ExitNode;

    /// <summary>
    /// A garázs előtti első cellához tartozó node.
    /// </summary>
    private Node DrivewayNode;

    /// <summary>
    /// Az aktív útszakaszok száma.
    /// </summary>
    public int NodeCount => ActiveRoads.Count;

    /// <summary>
    /// Az útkeresésnél használt irányvektorok (fel, le, bal, jobb).
    /// </summary>
    private static readonly List<Vector2Int> directions = new()
    {
    new Vector2Int(0, 1),   // fel
    new Vector2Int(0, -1),  // le
    new Vector2Int(-1, 0),  // bal
    new Vector2Int(1, 0)    // jobb
    };


    /// <summary>
    /// Inicializálja a singleton példányt és biztosítja, hogy ne legyen belőle duplikált példány.
    /// </summary>
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


    /// <summary>
    /// Converts world positions of the garage, driveway, and exit cells into nodes,
    /// and registers them as key points in the road network.
    /// </summary>
    private void Start()
    {
        Vector2Int garagePos = PlacementManager.Instance.GetRoadCellByPosition(firstCell.transform.position);
        Vector2Int drivewayPos = PlacementManager.Instance.GetRoadCellByPosition(secondCell.transform.position);
        Vector2Int exitPos = PlacementManager.Instance.GetRoadCellByPosition(lastCell.transform.position);

        GarageNode = AddNode(garagePos, 0);
        ExitNode = AddNode(exitPos, 1);
        DrivewayNode = AddNode(drivewayPos, 2);
    }


    /// <summary>
    /// Creates a new road structure and adds it to the node network at the given position.
    /// </summary>
    /// <param name="data">The building data for the road.</param>
    /// <param name="ID">The unique identifier of the road.</param>
    /// <param name="nodePosition">The grid position where the node is added.</param>
    /// <returns>The created road model.</returns>
    public Structure AddStructure(BuildingData data, int ID, Vector2Int nodePosition)
    {
        RoadModel road = new RoadModel(data, ID);
        if (road == null) throw new Exception("Nem sikerült léterhozni a következőt: Road");

        ActiveRoads.Add(road);
        AddNode(nodePosition, ID);
        HasRoute = SearchForPath();

        return road;
    }


    /// <summary>
    /// Registers a road structure from saved data and adds its node to the graph.
    /// </summary>
    /// <param name="data">The building data for the road.</param>
    /// <param name="saveData">Saved structure information.</param>
    /// <param name="gridPosition">The position of the road on the grid.</param>
    /// <returns>The reconstructed road model.</returns>
    public Structure RegisterStructure(BuildingData data, StructureSaveData saveData, Vector2Int gridPosition)
    {
        RoadModel road = new RoadModel(data, saveData);
        if (road == null) throw new Exception("Nem sikerült léterhozni a következőt: Road");

        ActiveRoads.Add(road);
        AddNode(gridPosition, saveData.UniqueID);
        HasRoute = SearchForPath();

        return road;
    }


    /// <summary>
    /// Removes a road structure and its associated node from the graph.
    /// Triggers a path search to update connectivity.
    /// </summary>
    /// <param name="ID">The unique ID of the road to remove.</param>
    public void RemoveStructure(int ID)
    {
        RoadModel road = ActiveRoads.Find(t => t.GetID() == ID);
        if (road == null) return;

        ActiveRoads.Remove(road);
        RemoveNode(ID);

        HasRoute = SearchForPath();
        OnRoadRemoved?.Invoke();
    }


    /// <summary>
    /// Currently unused. Intended to associate a visual view with a road model.
    /// </summary>
    /// <param name="view">The visual representation of the structure.</param>
    /// <param name="ID">The structure's unique ID.</param>
    public void SetView(IPlaceable view, int ID) { }


    /// <summary>
    /// Uses a randomized depth-first search to find a path from garage to exit.
    /// Returns a list of world positions along the path.
    /// </summary>
    public List<Vector3> SearchForRandomPath()
    {
        List<Node> nodes = RandomDFS(GarageNode, ExitNode);

        if (nodes.Count == 0) { Debug.Log("Nincsen út"); return new(); }

        return GetRoadPositions(nodes);
    }


    /// <summary>
    /// Finds a path from a specific world position to either the garage or exit using A*.
    /// </summary>
    /// <param name="from">The starting world position.</param>
    /// <param name="toExit">True to go to the exit; false to go to the garage.</param>
    /// <returns>List of world positions along the path.</returns>
    public List<Vector3> FindNewPath(Vector3 from, bool toExit)
    {
        Nodes.TryGetValue(PlacementManager.Instance.GetRoadCellByPosition(from), out Node fromNode);
        if (fromNode == null) return new();
        Node dest = toExit ? ExitNode : GarageNode;
        List<Node> nodes = AStar(fromNode, dest);

        if (nodes.Count == 0) { Debug.Log("Nincsen út"); return new(); }

        return GetRoadPositions(nodes);
    }


    /// <summary>
    /// Tries to find a valid path between garage and exit using A*.
    /// </summary>
    /// <returns>True if a path exists, false otherwise.</returns>
    public bool SearchForPath()
    {
        List<Node> nodes = AStar(GarageNode, ExitNode);
        return nodes.Count > 0;
    }


    /// <summary>
    /// Converts a list of nodes into a list of world positions representing road centers.
    /// </summary>
    /// <param name="nodes">The nodes along the path.</param>
    /// <returns>List of world positions.</returns>
    private List<Vector3> GetRoadPositions(List<Node> nodes)
    {
        List<Vector3> positions = new();

        foreach (var node in nodes)
        {
            if (node == DrivewayNode) { positions.Add(firstCell.transform.position); continue; }
            else if (node == ExitNode || node == GarageNode) continue;
            positions.Add(StructureManager.Instance.GetCorrespondingView(node.NodeID).GetGameObject().transform.Find("Middle").position);
        }

        return positions;
    }


    /// <summary>
    /// A* pathfinding algorithm for shortest path between two nodes.
    /// </summary>
    /// <param name="start">The starting node.</param>
    /// <param name="goal">The goal node.</param>
    /// <returns>List of nodes forming the optimal path.</returns>
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


    /// <summary>
    /// Performs a randomized depth-first search to find a path from start to goal.
    /// </summary>
    /// <param name="start">Starting node.</param>
    /// <param name="goal">Goal node.</param>
    /// <returns>List of nodes forming the found path (if any).</returns>
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


    /// <summary>
    /// Reconstructs the path from end to start using the parent map from pathfinding.
    /// </summary>
    /// <param name="parentMap">A dictionary of node parents.</param>
    /// <param name="current">The destination node.</param>
    /// <returns>Ordered list of nodes representing the full path.</returns>
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


    /// <summary>
    /// Retrieves a node object by its unique ID.
    /// </summary>
    /// <param name="nodeId">The ID of the node to retrieve.</param>
    /// <returns>The matching node, or null if not found.</returns>
    public Node GetNodeById(int nodeId)
    {
        return Nodes.Values.FirstOrDefault(node => node.NodeID == nodeId);
    }


    /// <summary>
    /// Calculates Manhattan distance between two nodes for A* heuristic.
    /// </summary>
    /// <param name="current">The current node.</param>
    /// <param name="goal">The destination node.</param>
    /// <returns>The estimated cost to reach the goal.</returns>
    private double Heuristic(Node current, Node goal)
    {
        return Math.Abs(current.Position.x - goal.Position.x) + Math.Abs(current.Position.y - goal.Position.y);
    }


    /// <summary>
    /// Adds a node at the given grid position and connects it to its neighbors.
    /// </summary>
    /// <param name="position">The position on the grid.</param>
    /// <param name="ID">The structure's unique identifier.</param>
    /// <returns>The newly added node.</returns>
    public Node AddNode(Vector2Int position, int ID)
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


    /// <summary>
    /// Uses a randomized depth-first search to find a path from garage to exit.
    /// Returns a list of world positions along the path.
    /// </summary>
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


    /// <summary>
    /// Returns a list of neighboring nodes for the given grid position.
    /// </summary>
    /// <param name="position">The center node's grid position.</param>
    /// <returns>List of connected neighbor nodes.</returns>
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


    /// <summary>
    /// Represents a single point in the road graph used for pathfinding.
    /// Stores neighbor connections and positional data.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// The unique identifier of this node (matches the associated road structure ID).
        /// </summary>
        public int NodeID;


        /// <summary>
        /// The grid-based position of the node in the road network.
        /// </summary>
        public Vector2Int Position;


        /// <summary>
        /// Dictionary mapping neighbor node IDs to connection weights.
        /// Used for graph traversal and pathfinding algorithms.
        /// </summary>
        public Dictionary<int, double> Neighbors = new();


        /// <summary>
        /// Constructs a new Node with the given ID and grid position.
        /// </summary>
        /// <param name="nodeID">Unique ID of the node.</param>
        /// <param name="gridPosition">Grid position of the node.</param>
        public Node(int nodeID, Vector2Int gridPosition)
        {
            NodeID = nodeID;
            Position = gridPosition;
        }


        /// <summary>
        /// Adds or updates a neighbor connection with the specified weight.
        /// </summary>
        /// <param name="ID">Neighbor node's ID.</param>
        /// <param name="weight">Cost or weight of the connection.</param>
        public void AddNeighbour(int ID, double weight)
        {
            Neighbors[ID] = weight;
        }


        /// <summary>
        /// Removes a neighbor connection by its node ID.
        /// </summary>
        /// <param name="ID">Neighbor node's ID to remove.</param>
        public void RemoveNeighbour(int ID)
        {
            Neighbors.Remove(ID);
        }


        /// <summary>
        /// Adds multiple nodes as neighbors with default weight (1.0).
        /// </summary>
        /// <param name="nodes">List of nodes to connect as neighbors.</param>
        public void AddNeighbours(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                AddNeighbour(node.NodeID, 1);
            }
        }
    }

}