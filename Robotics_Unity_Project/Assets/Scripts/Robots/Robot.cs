using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Robot : MonoBehaviour {
    #region enum

    public enum RobotState
    {
        LOCATE, GATHER, RETURN, IDLE, DEADBATTERY, CHARGING, DESTROYED,MOVING

    };
    public enum RobotType
    {
         COLLECTOR,UNASSIGNED
    };

    #endregion

    #region Inspector Fields

    #endregion

    #region RunTime Fields
    // Robot fields
    private RobotState robotState;
    public RobotType robotType;

    // World relation fields
    private GameObject prevWaypoint;
    private GameObject nextWaypoint;
    private GameObject targetWaypoint;
    private List<GameObject> waypointPath;
    private int currWaypoint;
    private float currSpeed;
    private int batteryConsumptionRate;
    private int batteryChargeRate;
    // Noise  --Did we decide on adding this?
    private float xDir;
    private float yDir;
    // Idle Actions
    // Locate actions
    // Gather actions
    #endregion

    #region Monobehaviors
    
    private void Awake() // Called during initialization
    { 
        robotState = RobotState.IDLE;
    }
    // Initialization
    void Start() 
    {
        switch(robotType)
        {
            case RobotType.COLLECTOR:
                // Add to collector count
                break;
            case RobotType.UNASSIGNED:
                // Add to unassigned
                break;

            default:
                Debug.LogError("Undefined robot type in Start()");
                break;                

        }
    }

   void Update() // Called once per frame 
    {
        if (targetWaypoint == null)
        {
            return;
        }

        if (robotState == RobotState.IDLE)
        {
            //IdleRobot();
        }
        else if (robotState == RobotState.MOVING)
        {
            //MovingRobot();
            //CheckWaypointDistance();
        }

    }


    #endregion

    #region Public Methods
    // Waypoint related methods
    public void AssignTargetWaypoint(GameObject target) 
    {
        prevWaypoint = targetWaypoint;
        targetWaypoint = target;
        if (prevWaypoint != null && targetWaypoint.GetComponent<Waypoint>() != null && prevWaypoint.GetComponent<Waypoint>() != null)
        {
            // Robot Moves
        }
    }
    public void AssignWaypointList(List<GameObject> waypointList) 
    {
        waypointPath = waypointList;
        AssignRobotState(RobotState.MOVING);
        AssignTargetWaypoint(waypointPath[0]);
        //currentWaypoint = 0;
    }
    public GameObject ReturnCurrWaypoint() 
    {
        return null;
        //return currWaypoint;
    }
    public GameObject ReturnTargetWaypoint() 
    {
        return targetWaypoint;
    }

    // Robot state methods
    public RobotState GetRobotState() 
    {
        return robotState;
    }

    public void AssignRobotState(RobotState newState)
    {
        robotState = newState;
    }
    // Robot Type methods
    public RobotType GetRobotType()
    {
        return robotType;
    } 

    public void AssignRobotType(RobotType newType)
    {
        robotType = newType;
    }
    #endregion


    #region Private Methods
    // RobotLocate sets the robot to randomly wander to locate resources
    // -- Should this contain a random locate & a take by list locate?
    private void RobotLocate(){}
    // RobotGather sets the robot to Gather a resource
    private void RobotGather(){}
    // RobotReturn sets the robot to return to the start point
    private void RobotReturn(){}
    // RobotIdle sets the robot to idle
    private void RobotIdle(){}
    // RobotCharge sets the robot to charge its battery
    private void RobotCharge(){} 
    #endregion
    

}


