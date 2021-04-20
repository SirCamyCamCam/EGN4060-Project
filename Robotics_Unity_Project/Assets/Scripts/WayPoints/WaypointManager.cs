// --------------------------------------------------------------
// MoonSim - WaypointManager                          4/26/2021
// Author(s): Cameron Carstens
// Contact: cameroncarstens@knights.ucf.edu
// --------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WaypointManager : MonoBehaviour {

    #region Inner Classes

    [System.Serializable]
    public class WaypointBridge
    {
        public Waypoint waypoint1;
        public Waypoint waypoint2;

        public WaypointBridge(Waypoint firstWaypoint, Waypoint secondWaypoint)
        {
            waypoint1 = firstWaypoint;
            waypoint2 = secondWaypoint;
        }
    }

    #endregion

    #region Enum

    public enum WaypointType
    {
        HOME,
        TRANSITION,
        CHARGING
    }

    #endregion

    #region Static Fields

    public static WaypointManager main;

    #endregion

    #region Inspector Fields

    [Header("Line Renderer")]
    public Material lineMaterial;
    public float lineWidth;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject homePrefab;
    [SerializeField]
    private GameObject transitionPrefab;
    [SerializeField]
    private GameObject chargingPrefab;

    [Space(10)]
    [Header("Spawn Settings")]
    [SerializeField]
    private float minDistanceBetweenWaypoints;
    [SerializeField]
    private int minNumWaypoints;
    [SerializeField]
    private int maxNumWaypoints;
    [SerializeField]
    private int xBorderMagnitude;
    [SerializeField]
    private int yBorderMagnitude;
    [SerializeField]
    private int maxSpawnAttempts;
    [SerializeField]
    private int maxChargingPoints;
    [SerializeField]
    private int minChargingPoints;
    [SerializeField]
    private float maxCharingPointDistance;

    [Header("Connection Settings")]
    [SerializeField]
    private bool closestMethod;
    [SerializeField]
    private float maxConnectionDistance;
    [SerializeField]
    private int maxConnections;
    [SerializeField]
    private int minConnections;
    [SerializeField]
    private float connectChance;

    [Space(10)]
    [Header("Sprite Renderers")]
    [SerializeField]
    private Sprite homeSprite;
    [SerializeField]
    private Sprite transitionSprite;
    [SerializeField]
    private Sprite chargingSprite;

    [Space(5)]
    [Header("Dependencies")]
    [SerializeField]
    private GameObject[] homeWaypoint;
    [SerializeField]
    private GameObject chargingWaypointsParent;
    [SerializeField]
    private GameObject transitionWaypointsParent;
    [SerializeField]
    private GameObject lineRenderersParent;

    #endregion

    #region Run-Time Fields

    private List<Waypoint> allWaypoints;
    private List<Waypoint> homeWaypoints;
    private List<Waypoint> transitionWaypoints;
    private List<Waypoint> chargingWaypoints;
    private List<Transform> waypointTransforms;
    private List<Transform> homeTransforms;
    private HashSet<WaypointBridge> bridgeSet;
    private Dictionary<WaypointBridge, LineRenderer> waypointLines;
    private Dictionary<(ResourceManager.ResourceType, int), List<Waypoint>> rememberedPaths;
    private Dictionary<ResourceManager.ResourceType, List<int>> rememberedPathsKeys;
    private int numChargingWaypoints;

    #endregion

    #region Monobehaviors

    private void Awake()
    {
        main = this;
        waypointLines = new Dictionary<WaypointBridge, LineRenderer>();
        rememberedPaths = new Dictionary<(ResourceManager.ResourceType, int), List<Waypoint>>();
        rememberedPathsKeys = new Dictionary<ResourceManager.ResourceType, List<int>>();
        bridgeSet = new HashSet<WaypointBridge>();

        allWaypoints = new List<Waypoint>();
        homeWaypoints = new List<Waypoint>();
        transitionWaypoints = new List<Waypoint>();
        chargingWaypoints = new List<Waypoint>();

        waypointTransforms = new List<Transform>();
        homeTransforms = new List<Transform>();


        foreach (GameObject homeWaypointObject in homeWaypoint)
        {
            homeTransforms.Add(homeWaypointObject.transform);
            allWaypoints.Add(homeWaypointObject.GetComponent<Waypoint>());
            homeWaypoints.Add(homeWaypointObject.GetComponent<Waypoint>());
            waypointTransforms.Add(homeWaypointObject.transform);
            homeWaypointObject.GetComponent<Waypoint>().SetUpWaypointTypes(WaypointType.HOME);
        }

        numChargingWaypoints = 0;
    }

    // Use this for initialization
    void Start () {
        Debug.Log("Spawning");
        SpawnAllOriginalWaypoints();
        Debug.Log("Num waypoint: " + allWaypoints.Count);
        Debug.Log("Minimum");
        EnsureMinimumChargingPoints();
        Debug.Log("Connections");
        ConnectAllWaypoints();
        Debug.Log("Rocks");
        DeleteConnectionsThroughRocks();
        Debug.Log("Delete");
        DeleteUnconnectedWaypoints();
        Debug.Log("Second Connection");
        //EnsureMinimumConnections();
    }

    #endregion

    #region Private Methods

    private void EnsureMinimumConnections()
    {
        foreach (Waypoint waypoint1 in allWaypoints)
        {
            if (waypoint1.GetConnectedWaypoints().Count >= minConnections)
            {
                continue;
            }

            int connectionsNeeded = minConnections - waypoint1.GetConnectedWaypoints().Count;
            List<Waypoint> waypointsTested = new List<Waypoint>();
            while (connectionsNeeded != 0)
            {
                List<Waypoint> closestConnection = new List<Waypoint>();
                foreach (Waypoint waypoint2 in allWaypoints)
                {
                    if (waypoint1 == waypoint2)
                    {
                        continue;
                    }

                    List<Waypoint> connections = waypoint1.GetConnectedWaypoints();
                    foreach (Waypoint connection in connections)
                    {
                        if (waypoint2 == connection)
                        {
                            continue;
                        }
                    }

                    foreach (Waypoint tested in waypointsTested)
                    {
                        if (waypoint2 == tested)
                        {
                            continue;
                        }
                    }

                    if (closestConnection.Count < connectionsNeeded)
                    {
                        closestConnection.Add(waypoint2);
                        continue;
                    }

                    foreach (Waypoint waypointInList in closestConnection)
                    {
                        if (Vector3.Distance(waypoint1.ReturnWaypointGameObject().transform.position, waypoint2.ReturnWaypointGameObject().transform.position) <
                          Vector3.Distance(waypoint1.ReturnWaypointGameObject().transform.position, waypointInList.ReturnWaypointGameObject().transform.position))
                        {
                            closestConnection.Remove(waypointInList);
                            closestConnection.Add(waypoint2);
                            waypointsTested.Add(waypoint2);
                            break;
                        }
                    }
                }

                foreach (Waypoint connection in closestConnection)
                {
                    // Test no rocks
                    RaycastHit2D hit;
                    Vector2 waypointPos = waypoint1.ReturnWaypointTransform().position;
                    Vector2 connectionPos = connection.ReturnWaypointTransform().position;
                    Vector2 direction = connectionPos - waypointPos;
                    float distance = direction.magnitude;
                    hit = Physics2D.Raycast(waypoint1.ReturnWaypointTransform().position, direction, distance);

                    if (hit.collider != null)
                    {
                        if (hit.transform.tag != "rock")
                        {
                            CreateWaypointBridge(waypoint1, connection);
                            connectionsNeeded -= 1;
                        }
                    }
                }
            }
        }
    }

    private void DeleteUnconnectedWaypoints()
    {
        bool restart = true;
        while (restart)
        {
            restart = false;
            foreach (Waypoint waypoint in allWaypoints)
            {
                if (waypoint.GetConnectedWaypoints().Count == 0)
                {
                    allWaypoints.Remove(waypoint);
                    waypointTransforms.Remove(waypoint.ReturnWaypointTransform());
                    switch (waypoint.ReturnWaypointType())
                    {
                        case WaypointType.CHARGING:
                            chargingWaypoints.Remove(waypoint);
                            break;
                        case WaypointType.TRANSITION:
                            transitionWaypoints.Remove(waypoint);
                            break;
                        case WaypointType.HOME:
                            homeTransforms.Remove(waypoint.ReturnWaypointTransform());
                            homeWaypoints.Remove(waypoint);
                            break;
                    }
                    Destroy(waypoint.ReturnWaypointGameObject());
                    restart = true;
                    break;
                }
            }
        }
    }

    private void DeleteConnectionsThroughRocks()
    {
        foreach (Waypoint waypoint in allWaypoints)
        {
            bool restart = true;
            while (restart)
            {
                restart = false;
                foreach (Waypoint connection in waypoint.GetConnectedWaypoints())
                {
                    RaycastHit2D hit;
                    Vector2 waypointPos = waypoint.ReturnWaypointTransform().position;
                    Vector2 connectionPos = connection.ReturnWaypointTransform().position;
                    Vector2 direction = connectionPos - waypointPos;
                    float distance = direction.magnitude; 
                    hit = Physics2D.Raycast(waypoint.ReturnWaypointTransform().position, direction, distance);

                    if (hit.collider != null)
                    {
                        if (hit.transform.tag == "rock")
                        {
                            foreach (WaypointBridge bridge in bridgeSet)
                            {
                                if (bridge.waypoint1 == waypoint && bridge.waypoint2 == connection ||
                                    bridge.waypoint1 == connection && bridge.waypoint2 == waypoint)
                                {
                                    LineRenderer waypointLine = waypointLines[bridge];
                                    waypointLines.Remove(bridge);
                                    Destroy(waypointLine);
                                    bridgeSet.Remove(bridge);
                                    waypoint.RemoveAConnectedWaypoint(connection);
                                    connection.RemoveAConnectedWaypoint(waypoint);
                                    restart = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (restart)
                    {
                        break;
                    }
                }
            }
        }
    }

    private void ConnectAllWaypoints()
    {
        if (closestMethod)
        {
            foreach (Waypoint waypoint1 in allWaypoints)
            {
                int waypointConnections = Random.Range(minConnections, maxConnections);
                List<Waypoint> closestWaypoints = new List<Waypoint>();

                foreach (Waypoint waypoint2 in allWaypoints)
                {
                    if (waypoint1 == waypoint2)
                    {
                        continue;
                    }

                    if (closestWaypoints.Count < waypointConnections)
                    {
                        closestWaypoints.Add(waypoint2);
                        continue;
                    }

                    foreach (Waypoint waypointInList in closestWaypoints)
                    {
                        if (Vector3.Distance(waypoint1.ReturnWaypointGameObject().transform.position, waypoint2.ReturnWaypointGameObject().transform.position) <
                            Vector3.Distance(waypoint1.ReturnWaypointGameObject().transform.position, waypointInList.ReturnWaypointGameObject().transform.position))
                        {
                            closestWaypoints.Remove(waypointInList);
                            closestWaypoints.Add(waypoint2);
                            break;
                        }
                    }
                }

                foreach (Waypoint closeWaypoint in closestWaypoints)
                {
                    CreateWaypointBridge(waypoint1, closeWaypoint);
                }
            }
        }
        // Random Method
        else
        {
            // Connect them
            foreach (Waypoint waypoint1 in allWaypoints)
            {
                foreach (Waypoint waypoint2 in allWaypoints)
                {
                    if (waypoint1 == waypoint2)
                    {
                        continue;
                    }

                    if (waypoint1.GetConnectedWaypoints().Count >= maxConnections || waypoint2.GetConnectedWaypoints().Count >= maxConnections)
                    {
                        break;
                    }

                    bool connectWaypoints = false;
                    if (Vector3.Distance(waypoint1.ReturnWaypointTransform().position, waypoint2.ReturnWaypointTransform().position) < maxConnectionDistance)
                    {
                        connectWaypoints = (Random.value <= connectChance);
                    }

                    if (connectWaypoints)
                    {
                        // Check if there is a rock between

                        CreateWaypointBridge(waypoint1, waypoint2);
                    }
                }
            }

            // Ensure there is a minimum number of connections
            foreach (Waypoint waypoint1 in allWaypoints)
            {
                int waypointConnections = waypoint1.GetConnectedWaypoints().Count;
                if (waypointConnections < minConnections + 1)
                {
                    List<Waypoint> closestWaypoint = new List<Waypoint>();

                    foreach (Waypoint waypoint2 in allWaypoints)
                    {
                        if (waypoint1 == waypoint2)
                        {
                            continue;
                        }
                        if (closestWaypoint.Count < (minConnections - waypointConnections))
                        {
                            closestWaypoint.Add(waypoint2);
                            continue;
                        }

                        foreach (Waypoint waypointInList in closestWaypoint)
                        {
                            if (Vector3.Distance(waypoint1.ReturnWaypointGameObject().transform.position, waypoint2.ReturnWaypointGameObject().transform.position) <
                                Vector3.Distance(waypoint1.ReturnWaypointGameObject().transform.position, waypointInList.ReturnWaypointGameObject().transform.position))
                            {
                                closestWaypoint.Remove(waypointInList);
                                closestWaypoint.Add(waypoint2);
                                break;
                            }
                        }
                    }

                    foreach (Waypoint closeWaypoint in closestWaypoint)
                    {
                        CreateWaypointBridge(waypoint1, closeWaypoint);
                    }
                }
            }
        }
    }

    private void CreateWaypointBridge(Waypoint waypoint1, Waypoint waypoint2)
    {
        waypoint1.AddAConnectedWaypoint(waypoint2);
        waypoint2.AddAConnectedWaypoint(waypoint1);

        WaypointBridge newBridge = new WaypointBridge(waypoint1, waypoint2);
        bridgeSet.Add(newBridge);

        LineRenderer newLine = new GameObject().AddComponent<LineRenderer>();
        newLine.transform.parent = lineRenderersParent.transform;
        newLine.gameObject.name = "LineRenderer";
        newLine.material = lineMaterial;
        newLine.startWidth = lineWidth;
        newLine.endWidth = lineWidth;
        newLine.numCapVertices = 5;
        newLine.SetPosition(0, waypoint1.ReturnWaypointGameObject().transform.position);
        newLine.SetPosition(1, waypoint2.ReturnWaypointGameObject().transform.position);
        //newLine.sortingOrder = -1;
        waypointLines.Add(newBridge, newLine);
    }

    // Spawns all the original waypoints to start the sim
    private void SpawnAllOriginalWaypoints()
    {
        int numWaypointsToSpawn = Random.Range(minNumWaypoints, maxNumWaypoints);

        for (int i = 0; i < numWaypointsToSpawn; i++)
        {
            bool legalPoint = false;
            float xPoint;
            float yPoint;
            int spawnAttempt = 0;
            Vector3 spawnPoint = new Vector3(0, 0, 0);

            // Find a legal spawn point
            while (legalPoint == false)
            {
                spawnAttempt += 1;
                xPoint = Random.Range(-xBorderMagnitude, xBorderMagnitude);
                yPoint = Random.Range(-yBorderMagnitude, yBorderMagnitude);
                spawnPoint = new Vector3(xPoint, yPoint, 0);
                legalPoint = true;

                foreach (Transform waypoint in waypointTransforms)
                {
                    if (Vector3.Distance(waypoint.position, spawnPoint) <= minDistanceBetweenWaypoints)
                    {
                        legalPoint = false;
                        break;
                    }
                }

                if (spawnAttempt == maxSpawnAttempts)
                {
                    return;
                }
            }

            // Determine if it should maybe be spawned as a charge point
            bool spawnAsChargePoint = false;
            if (numChargingWaypoints < maxChargingPoints)
            {
                foreach (Transform homeWaypoint in homeTransforms)
                {
                    if (Vector3.Distance(homeWaypoint.position, spawnPoint) <= maxCharingPointDistance)
                    {
                        spawnAsChargePoint = (Random.value > 0.7f);
                    }
                }
            }

            // Spawn the waypoint
            if (spawnAsChargePoint)
            {
                GameObject newChargePoint = Instantiate(chargingPrefab, spawnPoint, Quaternion.Euler(0, 0, 0), chargingWaypointsParent.transform);
                allWaypoints.Add(newChargePoint.GetComponent<Waypoint>());
                chargingWaypoints.Add(newChargePoint.GetComponent<Waypoint>());
                waypointTransforms.Add(newChargePoint.transform);
                newChargePoint.GetComponent<Waypoint>().SetUpWaypointTypes(WaypointType.CHARGING);
                numChargingWaypoints += 1;
            }
            else
            {
                GameObject newTransitionPoint = Instantiate(transitionPrefab, spawnPoint, Quaternion.Euler(0, 0, 0), transitionWaypointsParent.transform);
                allWaypoints.Add(newTransitionPoint.GetComponent<Waypoint>());
                transitionWaypoints.Add(newTransitionPoint.GetComponent<Waypoint>());
                waypointTransforms.Add(newTransitionPoint.transform);
                newTransitionPoint.GetComponent<Waypoint>().SetUpWaypointTypes(WaypointType.TRANSITION);
            }
        }
    }

    private void EnsureMinimumChargingPoints()
    {
        float newMaxCharingPointDistance = 0;
        float itteration = 0;
        bool restart = true;
        // If we don't have enough charging waypoints, force it!
        while (numChargingWaypoints < minChargingPoints && restart)
        {
            restart = false;
            newMaxCharingPointDistance = maxCharingPointDistance + itteration;
            foreach (Waypoint home in homeWaypoints)
            {
                foreach (Waypoint waypoint in allWaypoints)
                {
                    if (waypoint.ReturnWaypointType() == WaypointType.TRANSITION
                        && Vector3.Distance(home.ReturnWaypointGameObject().transform.position, waypoint.ReturnWaypointTransform().position) < newMaxCharingPointDistance)
                    {
                        Vector3 position = waypoint.ReturnWaypointTransform().position;
                        allWaypoints.Remove(waypoint);
                        transitionWaypoints.Remove(waypoint);
                        waypointTransforms.Remove(waypoint.ReturnWaypointTransform());
                        Destroy(waypoint.ReturnWaypointGameObject());

                        GameObject newChargePoint = Instantiate(chargingPrefab, position, Quaternion.Euler(0, 0, 0), chargingWaypointsParent.transform);
                        allWaypoints.Add(newChargePoint.GetComponent<Waypoint>());
                        chargingWaypoints.Add(newChargePoint.GetComponent<Waypoint>());
                        waypointTransforms.Add(newChargePoint.transform);
                        numChargingWaypoints += 1;
                        restart = true;
                        break;
                    }
                    if (restart)
                    {
                        break;
                    }
                }
            }
            itteration += 1f;
        }
    }

    private List<Waypoint> Dijkstra(Waypoint start, Waypoint target)
    {
        // Initalize
        Dictionary<Waypoint, List<Waypoint>> shortestPath = new Dictionary<Waypoint, List<Waypoint>>();
        Dictionary<Waypoint, float> distancePath = new Dictionary<Waypoint, float>();
        List<Waypoint> unvisited = new List<Waypoint>();
        int totalWaypoints = allWaypoints.Count;
        foreach (Waypoint waypoint in allWaypoints)
        {
            distancePath.Add(waypoint, float.MaxValue);
        }
        distancePath[start] = 0;
        List<Waypoint> currentList = new List<Waypoint>();
        currentList.Add(start);
        shortestPath.Add(start, currentList);

        // Start
        Waypoint current = start;
        while (shortestPath.Count != totalWaypoints)
        {
            List<Waypoint> connections = current.GetConnectedWaypoints();
            foreach(Waypoint connection in connections)
            {
                if (shortestPath.ContainsKey(connection))
                {
                    float distanceBetween = Vector2.Distance(current.ReturnWaypointTransform().position, connection.ReturnWaypointTransform().position);
                    float totalDistance = distanceBetween + distancePath[connection];

                    foreach(Waypoint connection2 in connections)
                    {
                        if (connection == connection2)
                        {
                            continue;
                        }

                        float distanceBetween2 = Vector2.Distance(current.ReturnWaypointTransform().position, connection2.ReturnWaypointTransform().position);
                        float totalDistance2 = distanceBetween2 + distancePath[connection2];

                        if (totalDistance < totalDistance2)
                        {
                            List<Waypoint> newList = new List<Waypoint>();
                            foreach (Waypoint waypointToAdd in shortestPath[connection])
                            {
                                newList.Add(waypointToAdd);
                            }
                            newList.Add(current);

                            shortestPath.Add(current, newList);
                            distancePath[current] = totalDistance;
                        }
                    }
                }
                else
                {
                    unvisited.Add(connection);
                }
            }
            current = unvisited[0];
            unvisited.Remove(current);
        }

        return shortestPath[target];
    }

    #endregion

    #region Public Methods

    // Returns a list of waypoints to follow back home
    public List<Waypoint> ReturnToHome(Waypoint start)
    {
        return Dijkstra(start, homeWaypoints[0]);
    }

    public List<Waypoint> PathToWaypoint(Waypoint start, Waypoint end)
    {
        return Dijkstra(start, end);
    }

    public List<Waypoint> PathToChargingWaypoint(Waypoint start)
    {
        int random = Random.Range(0, chargingWaypoints.Count - 1);
        if (random < 0)
        {
            return null;
        }

        return Dijkstra(start, chargingWaypoints[random]);
    }

    public void AddtoRememberedPaths(List<Waypoint> listOfWaypoints, ResourceManager.ResourceType typeAtEndOfPath)
    {
        listOfWaypoints.Reverse();
        List<int> intList = rememberedPathsKeys[typeAtEndOfPath];
        if (intList == null)
        {
            intList = new List<int>();
        }

        bool validInt = false;
        int random = 0;
        while (!validInt)
        {
            random = Random.Range(int.MinValue, int.MaxValue);
            if (!intList.Contains(random))
            {
                validInt = true;
            }
        }
        intList.Add(random);
        rememberedPathsKeys[typeAtEndOfPath] = intList;

        rememberedPaths.Add((typeAtEndOfPath, random), listOfWaypoints);
    }

    public List<Waypoint> GetRememberedPath(ResourceManager.ResourceType rescouceToFind)
    {
        List<int> returnedList = rememberedPathsKeys[rescouceToFind];
        int random = Random.Range(0, returnedList.Count - 1);
        if (random < 0)
        {
            return null;
        }

        return rememberedPaths[(rescouceToFind, returnedList[random])];
    }

    public void RemoveRememberedPath(ResourceManager.ResourceType typeToRemove, List<Waypoint> pathToRemove)
    {
        List<int> returnedIntList = rememberedPathsKeys[typeToRemove];

        foreach (int i in returnedIntList)
        {
            if (rememberedPaths[(typeToRemove, i)] == pathToRemove)
            {
                rememberedPaths.Remove((typeToRemove, i));
            }
        }
    }

    public Waypoint GetHomeWaypoint()
    {
        if (homeWaypoints.Count == 0)
        {
            return null;
        }

        int random = Random.Range(0, homeWaypoints.Count - 1);

        return homeWaypoints[random];
    }

    public Waypoint GetChargingWaypoint()
    {
        if (chargingWaypoints.Count == 0)
        {
            return null;
        }

        int random = Random.Range(0, chargingWaypoints.Count - 1);

        return chargingWaypoints[random];
    }

    public List<Transform> GetHomeTransforms()
    {
        List<Transform> homes = new List<Transform>();
        foreach (GameObject home in homeWaypoint)
        {
            homes.Add(home.transform);
        }
        return homes;
    }

    #endregion

    #region Coroutines


    #endregion
}
