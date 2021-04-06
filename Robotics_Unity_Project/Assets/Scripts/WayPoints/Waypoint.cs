// --------------------------------------------------------------
// MoonSim - Waypoint                                   4/05/2021
// Author(s): Cameron Carstens
// Contact: cameroncarstens@knights.ucf.edu
// --------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
    
    #region Inspector Fields

    [Header("Dependencies")]
    [SerializeField]
    private GameObject attachedGameObject;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    #endregion

    #region Run-Time Fields

    private WaypointManager.WaypointType type;
    private List<Waypoint> connectedWaypoints;

    #endregion

    #region Monobehavior

    private void Awake()
    {
        connectedWaypoints = new List<Waypoint>();
    }

    // Use this for initialization
    void Start()
    {

    }

    #endregion

    #region Public methods

    // Sets the types for the waypoint and connected waypoints
    public void SetUpWaypointTypes(WaypointManager.WaypointType typeToSet)
    {
        type = typeToSet;
    }

    // Adds a new waypoint
    public void AddAConnectedWaypoint(Waypoint newWaypoint)
    {
        connectedWaypoints.Add(newWaypoint);
    }

    // Removes a waypoint from connected
    public void RemoveAConnectedWaypoint(Waypoint removedWaypoint)
    {
        connectedWaypoints.Remove(removedWaypoint);
    }

    // Returns the type
    public WaypointManager.WaypointType ReturnWaypointType()
    {
        return type;
    }

    // Returns the gameobject
    public GameObject ReturnWaypointGameObject()
    {
        return attachedGameObject;
    }

    // Assigns the sprite
    public void AssignSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    // Returns connected waypoints
    public List<Waypoint> GetConnectedWaypoints()
    {
        return connectedWaypoints;
    }

    #endregion

    #region Private Methods


    #endregion

    #region Coroutines


    #endregion
}