// --------------------------------------------------------------
// Coloniant - WaypointManager                          2/23/2020
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
    [SerializeField]
    private AnimationCurve bounceCurve;

    [Header("Settings")]
    [SerializeField]
    private float bounceRate;
    [SerializeField]
    private float bounceHeight;

    #endregion

    #region Runt-Time Fields

    private WaypointManager.WaypointType type;
    [HideInInspector]
    public List<Waypoint> connectedWaypoints;
    private float originalHeight;
    // Depreciated
    private float bounceTime;

    #endregion

    #region Monobehavior

    private void Awake()
    {
        connectedWaypoints = new List<Waypoint>();
    }

    // Use this for initialization
    void Start()
    {
        if (type == WaypointManager.WaypointType.TRANSITION)
        {
            spriteRenderer.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
        }

        originalHeight = spriteRenderer.transform.position.y;

        //StartCoroutine(BounceUp());
    }

    // Update is called once per frame
    void Update () {
        spriteRenderer.transform.position = new Vector3
            (
            spriteRenderer.transform.position.x,
            originalHeight + (Mathf.Sin(Time.time * bounceRate) * bounceHeight),
            spriteRenderer.transform.position.z
            );
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

    #endregion

    #region Private Methods

    private void CallDown()
    {
        StartCoroutine(BounceDown());
    }

    private void CallUp()
    {
        StartCoroutine(BounceUp());
    }

    #endregion

    #region Coroutines

    private IEnumerator BounceUp()
    {
        float startTime = Time.time;
        Vector3 targetPos = new Vector3
            (
            spriteRenderer.transform.position.x,
            originalHeight + (bounceHeight / 2),
            spriteRenderer.transform.position.z
            );
        float randomAddition = Random.Range(0, 0.3f);
        //randomAddition = 0;

        while ((Time.time - startTime) < (bounceTime + randomAddition))
        {
            float t = bounceCurve.Evaluate((Time.time - startTime) / (bounceTime + randomAddition));

            spriteRenderer.transform.position = Vector3.Lerp(spriteRenderer.transform.position, targetPos, t);
        }

        yield return new WaitForSeconds(0);

        CallDown();
    }

    private IEnumerator BounceDown()
    {
        float startTime = Time.time;
        Vector3 targetPos = new Vector3
            (
            spriteRenderer.transform.position.x,
            originalHeight - (bounceHeight / 2),
            spriteRenderer.transform.position.z
            );
        float randomAddition = Random.Range(0, 0.3f);
        //randomAddition = 0;

        while ((Time.time - startTime) < (bounceTime  + randomAddition))
        {
            float t = bounceCurve.Evaluate((Time.time - startTime) / (bounceTime + randomAddition));

            spriteRenderer.transform.position = Vector3.Lerp(spriteRenderer.transform.position, targetPos, t);
        }

        yield return new WaitForSeconds(0);

        CallUp();
    }

    #endregion
}