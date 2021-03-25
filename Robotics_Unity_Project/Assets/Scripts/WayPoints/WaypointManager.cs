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
        TRANSITION
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
    private GameObject waypointPrefab;

    [Header("Settings")]
    [SerializeField]
    private int maxRobots;
    [SerializeField]
    private float maxSpawnDistance;
    [SerializeField]
    private float minAllowedBetweenDistance;
    [SerializeField]
    private float maxLeafSpawnDistance;
    [SerializeField]
    private float minLeafBetweenDistance;
    [SerializeField]
    private float maxLeafConnectedDistance;
    [SerializeField]
    private bool spawnLeavesRandomly;

    [Space(10)]
    [Header("Sprite Renderers")]
    [SerializeField]
    private Sprite homeSprite;
    [SerializeField]
    private Sprite transitionSprite;

    [Space(5)]
    [Header("Dependencies")]

    #endregion

    #region Run-Time Fields

    public List<Waypoint> allWaypoints;
    private List<Waypoint> homeWaypoints;
    private List<Waypoint> transitionWaypoints;
    private HashSet<WaypointBridge> bridgeSet;
    private Dictionary<WaypointBridge, LineRenderer> waypointLines;

    #endregion

    #region Monobehaviors

    private void Awake()
    {
        main = this;
        allWaypoints = new List<Waypoint>();
        waypointLines = new Dictionary<WaypointBridge, LineRenderer>();
        bridgeSet = new HashSet<WaypointBridge>();

        allWaypoints = new List<Waypoint>();
        homeWaypoints = new List<Waypoint>();
        transitionWaypoints = new List<Waypoint>();
}

    // Use this for initialization
    void Start () {
        SpawnAllOriginalWaypoints();
	}

    #endregion

    #region Private Methods

    // Spawns all the original waypoints to start the sim
    private void SpawnAllOriginalWaypoints()
    {
        
    }
    
    private List<GameObject> SearchWaypointPathRecursiveType(
        Waypoint currentWaypoint, 
        WaypointType waypointTypeToFind, 
        List<GameObject> list,
        List<Waypoint> visited)
    {
        // Check for base case
        if (currentWaypoint.ReturnWaypointType() == waypointTypeToFind)
        {
            return list;
        }

        // int i = 0;

        // check for other posibilties
        foreach (Waypoint w in currentWaypoint.connectedWaypoints)
        {
            //Debug.Log(i);
            //i++;
            if (visited.Contains(w) == false)
            {
                visited.Add(w);
                list.Add(w.ReturnWaypointGameObject());
                SearchWaypointPathRecursiveType(w, waypointTypeToFind, list, visited);

                if (list[list.Count - 1].GetComponent<Waypoint>().ReturnWaypointType() == waypointTypeToFind)
                {
                    return list;
                }

                list.Remove(w.ReturnWaypointGameObject());
            }
        }

        return list;
    }

    private List<GameObject> SearchWaypointPathRecursiveTarget(
        Waypoint currentWaypoint, 
        Waypoint targetWaypoint, 
        List<GameObject> list, 
        List<Waypoint> visited)
    {
        // Base Case
        if (currentWaypoint == targetWaypoint)
        {
            return list;
        }

        // check for other posibilties
        foreach (Waypoint w in currentWaypoint.connectedWaypoints)
        {
            //Debug.Log(i);
            //i++;
            if (visited.Contains(w) == false)
            {
                visited.Add(w);
                list.Add(w.ReturnWaypointGameObject());
                SearchWaypointPathRecursiveTarget(w, targetWaypoint, list, visited);

                if (list[list.Count - 1].GetComponent<Waypoint>() == targetWaypoint)
                {
                    return list;
                }

                list.Remove(w.ReturnWaypointGameObject());
            }
        }

        return list;
    }

    #endregion

    #region Public Methods

    // Spawns a new Waypoint
    public Waypoint SpawnWaypoint(
        WaypointManager.WaypointType waypointType,  
        List<Waypoint> connectedWaypoints, 
        Vector3 spawnLocation)
    {
        // Spawn it
        GameObject newWaypointGameObject = Instantiate(waypointPrefab, spawnLocation, new Quaternion(0, 0, 0, 0), transform);
        if (newWaypointGameObject == null)
        {
            Debug.Log("Failed to spawn a " + waypointType);
            return null;
        }

        // Get Waypoint object
        Waypoint newWaypointClass = newWaypointGameObject.GetComponent<Waypoint>();
        if (newWaypointClass == null)
        {
            Debug.Log("Failed to get waypoint class");
            return null;
        }

        // Assign connected waypoints
        newWaypointClass.SetUpWaypointTypes(waypointType);

        // Connect everything else
        if (connectedWaypoints.Count > 0)
        {
            foreach (Waypoint w in connectedWaypoints)
            {
                if (w == null)
                {
                    Debug.Log("Failed to get connected waypoints");
                    return null;
                }
                w.AddAConnectedWaypoint(newWaypointClass);
                newWaypointClass.AddAConnectedWaypoint(w);
                WaypointBridge newBridge = new WaypointBridge(w, newWaypointClass);
                if (newBridge == null)
                {
                    Debug.Log("Failed to create a new bridge for " + waypointType);
                    return null;
                }
                bridgeSet.Add(newBridge);

                LineRenderer newLine = new GameObject().AddComponent<LineRenderer>();
                newLine.transform.parent = transform;
                newLine.gameObject.name = "LineRenderer";
                newLine.material = lineMaterial;
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.numCapVertices = 5;
                newLine.SetPosition(0, w.GetComponent<Transform>().position);
                newLine.SetPosition(1, newWaypointGameObject.transform.position);
                newLine.sortingOrder = -1;
                newLine.enabled = false;
               
                waypointLines.Add(newBridge, newLine);
            }
        }

        // Add to type list
        switch(waypointType)
        {
            case WaypointType.TRANSITION:
                transitionWaypoints.Add(newWaypointClass);
                newWaypointClass.AssignSprite(transitionSprite);
                break;
            case WaypointType.HOME:
                homeWaypoints.Add(newWaypointClass);
                newWaypointClass.AssignSprite(homeSprite);
                break;
            default:
                Debug.Log("Failed to assign waypoint type");
                return null;
        }

        // Add to all
        allWaypoints.Add(newWaypointClass);

        return newWaypointClass;
    }

    

    public List<GameObject> SearchPathUnknownTarget(Waypoint initalWaypoint, WaypointType waypointTypeToFind)
    {
        List<GameObject> path = new List<GameObject>();
        List<Waypoint> visited = new List<Waypoint>();

        // BFS Style

        /*List<Waypoint> visited = new List<Waypoint>();
        Queue<Waypoint> q = new Queue<Waypoint>();

        q.Enqueue(initalWaypoint);
        visited.Add(initalWaypoint);

        while (q.Count != 0)
        {
            Waypoint waypoint = q.Dequeue();

            if (waypoint.ReturnWaypointType() == waypointTypeToFind)
            {
                path.Add(waypoint.ReturnWaypointGameObject());
                return path;
            }

            foreach (Waypoint w in waypoint.connectedWaypoints)
            {
                if (!visited.Contains(w))
                {
                    visited.Add(w);
                    q.Enqueue(w);
                }
            }
        }*/

        path.Add(initalWaypoint.ReturnWaypointGameObject());
        visited.Add(initalWaypoint);
        path = SearchWaypointPathRecursiveType(initalWaypoint, waypointTypeToFind, path, visited);

        return path;
    }

    public List<GameObject> SearchPathKnownTarget(Waypoint initalWaypoint, Waypoint targetWaypoint)
    {
        List<GameObject> path = new List<GameObject>();
        List<Waypoint> visited = new List<Waypoint>();

        path.Add(initalWaypoint.ReturnWaypointGameObject());
        visited.Add(initalWaypoint);

        path = SearchWaypointPathRecursiveTarget(initalWaypoint, targetWaypoint, path, visited);

        return path;
    }

    public GameObject ReturnTransitionGameObject()
    {
        return transitionWaypoints[0].gameObject;
    }

    public List<Waypoint> ReturnHomeList()
    {
        return homeWaypoints;
    }

    public Waypoint ReturnRandomWaypoint()
    {
        int random = Random.Range(0, allWaypoints.Count - 1);

        return allWaypoints[random];
    }

    #endregion

    #region Coroutines


    #endregion
}
