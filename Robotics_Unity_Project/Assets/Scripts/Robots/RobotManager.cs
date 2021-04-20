// --------------------------------------------------------------
// MoonSim - RobotManager                            4/26/2021
// Author(s): 
// Contact: 
// --------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    #region Inner Classes

    // Stuff

    #endregion

    #region Enum

    // Do we have any enums?

    #endregion

    #region Static Fields

    // Public
    public static RobotManager main;

    // Private

    #endregion

    #region Inspector Fields

    // Stuff
    [Header("Robot Settings")]
    public float spawnRate;
    [SerializeField]
    private float defaultRobotSpeed;
    [SerializeField]
    private float defaultIdleNoise;
    [SerializeField]
    private float defaultRotationSpeed;
    [SerializeField]
    private float defaultIdleDistance;
    [SerializeField]
    private float defaultMovingNoise;
    [SerializeField]
    private float defaultMovingWaypointDistance;

    [Header("Battery Consumption Settings")]
    // do we need custom battery consumption?
    //[SerializeField]
    //private float collectorConsumptionRate;
    //[SerializeField]
    //private float unassignedConsumptionRate;
    [SerializeField]
    private float defaultConsumptionRate;
    [SerializeField]
    private float defaultChargeRate;
    [SerializeField]
    private float chargeWaitTime;

    [Header("Resource Collection Settings")]
    [SerializeField]
    private float collectorCollectionRate;
    [SerializeField]
    private float unassignedCollectionRate;
    [SerializeField]
    private float collectionWaitTime;

    [Header("Dependencies")]
    [SerializeField]
    private GameObject robotPrefab;

    #endregion

    #region Run-Time Fields

    // Robot counts and lists
    private int collectorRobotCount = 0;
    private int unassignedRobotCount = 0;
    private List<Robot> robotList;
    [HideInInspector]
    private List<Robot> collectorRobots;
    private List<Robot> unassignedRobots;
    // Battery 
    private float currentBatteryLevel;
    private float currentBatteryConsumption;
    // Resources
    [SerializeField]
    private float currentResources;
    // Add queue for resources

    #endregion

    #region Monobehaviors

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(this);
        }

        robotList = new List<Robot>();
        collectorRobots = new List<Robot>();
        unassignedRobots = new List<Robot>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentBatteryLevel = defaultChargeRate;

        // How to correctly spawn robots?
        GameObject homeBase = WaypointManager.main.ReturnHomeGameObject();
        GameObject newRobot = Instantiate(robotPrefab, homeBase.transform.position, new Quaternion(0, 0, 0, 0), transform);
        robotList.Add(newRobot.GetComponent<Robot>());

        // ???

        AddRobotsToSpawn(Robot.RobotType.COLLECTOR);
        AddRobotsToSpawn(Robot.RobotType.UNASSIGNED);
        UpdateResourceStatus();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    #endregion

    #region Private Methods

    // Stuff
    private void UpdateResourceStatus()
    {
        float tempCollection = 0;

        foreach(Robot r in robotList)
        {
            switch(r.robotType)
            {
                case Robot.RobotType.COLLECTOR:
                    tempCollection += collectorCollectionRate;
                    break;
                case Robot.RobotType.UNASSIGNED:
                    tempCollection += unassignedCollectionRate;
                    break;
                default:
                    Debug.Log("No robot type matched to calculate current resource collection!!!");
                    break;
            }
        }
        currentResources = tempCollection;

        StartCoroutine(WaitToCountCollection());
    }

    private void UpdateBatteryStatus(Robot robot)
    {
        RemoveBatteryLife(robot);
    }

    private IEnumerator RemoveBatteryLife(Robot robot)
    {
        yield return new WaitForSeconds(1);
        currentBatteryLevel -= 0.1;
        if (currentBatteryLevel < 0)
        {
            KillRobot(robot); 
            yield return;
        }
    }

    private void DestroyRandomRobot()
    {
        int rnd = Random.Range(0, robotList.Count - 1);

        if (GetTotalRobotCount == 1)
        {
            return;
        }
        KillRobot(robotList[rnd]);
    }

        
    #endregion

    #region Public Methods

        // Stuff
        // Returns the default speed of the robot
        public float DefaultRobotSpeed()
    {
        return defaultRobotSpeed;
    }

    // Returns the default idle noise for the robot
    public float DefaultIdleNoise()
    {
        return defaultIdleNoise;
    }

    // Returns the default rotation speed
    public float DefaultRotationSpeed()
    {
        return defaultRotationSpeed;
    }

    // Returns the default idle distance
    public float DefaultIdleDistance()
    {
        return defaultIdleDistance;
    }

    // Returns the default moving noise
    public float DefaultMovingNoise()
    {
        return defaultMovingNoise;
    }

    // Returns the default moving distance to a waypoint to switch waypoints
    public float DefaultMovingWaypointDistance()
    {
        return defaultMovingWaypointDistance;
    }

    // Adds to collector robot count
    public void AddToCollectorCount(Robot robotToAdd)
    {
        if (robotToAdd == null)
        {
            Debug.LogError("Tried to add a null Collector Robot!!!");
            return;
        }
        collectorRobotCount++;
        robotList.Add(robotToAdd);
    }

    // Adds to unassigned robot count
    public void AddToUnassignedCount(Robot robotToAdd)
    {
        if (robotToAdd == null)
        {
            Debug.LogError("Tried to add a null Unassigned Robot!!!");
            return;
        }
        unassignedRobotCount++;
        robotList.Add(robotToAdd);
    }

    // Removes from collector robot count
    public void RemoveFromCollectorCount(Robot robotToRemove)
    {
        if (robotToRemove == null)
        {
            Debug.LogError("Tried to remove a null Collector Robot!!!");
            return;
        }
        robotList.Remove(robotToRemove);
        if (collectorRobotCount > 0)
        {
            collectorRobotCount--;
        }
        else
        {
            collectorRobotCount = 0;
        }
    }

    // Removes from unassigned robot count
    public void RemoveFromUnassignedCount(Robot robotToRemove)
    {
        if (robotToRemove == null)
        {
            Debug.LogError("Tried to remove a null Unassigned Robot!!!");
            return;
        }
        robotList.Remove(robotToRemove);
        if (unassignedRobotCount > 0)
        {
            unassignedRobotCount--;
        }
        else
        {
            unassignedRobotCount = 0;
        }
    }

    // Returns collector robot count
    public int GetCollectorCount()
    {
        return collectorRobotCount;
    }

    // Returns unassigned robot count
    public int GetUnassignedCount()
    {
        return unassignedRobotCount;
    }

    // Returns the total robot count
    public int GetTotalRobotCount()
    {
        return collectorRobotCount + unassignedRobotCount;
    }

    // Adds a robot to the spawning counter
    public void AddRobotsToSpawn(Robot.RobotType type)
    {
        switch (type)
        {
            case Robot.RobotType.COLLECTOR:
                collectorRobots[0].GetComponent<CollectorRobot>().AddRobotToSpawn(collectorRobots.Robots.COLLECTOR, 1);
                break;
            case Robot.RobotType.UNASSIGNED:
                unassignedRobots[0].GetComponent<UnassignedRobot>().AddRobotToSpawn(unassignedRobots.Robots.UNASSIGNED, 1);
                break;
        }
    }

    public void AddToResources(float amount)
    {
        if (amount < 0)
        {
            return;
        }
        currentResources += amount;
    }

    // How is this going to be used?
    public void RemoveFromResources(float amount)
    {
        if (amount < 0)
        {
            return;
        }
        currentResources -= amount;
    }

    public float ReturnCurrentResourceCollection()
    {
        return currentResources;
    }

    public void RemoveRobotFromList(Robot robot)
    {
        if (robot == null)
        {
            Debug.Log("Robot is null in RemoveRobotFromList in RobotManager");
        }

        robotList.Remove(robot);

        switch (robot.robotType)
        {
            case Robot.RobotType.COLLECTOR:
                collectorRobots.Remove(robot);
                RemoveFromCollectorCount(robot);
                break;
            case Robot.RobotType.UNASSIGNED:
                unassignedRobots.Remove(robot);
                RemoveFromUnassignedCount(robot);
                break;
        }
    }

    public void KillRobot(Robot robot)
    {
        Destroy(robot);
    }

    // Do we need to randomly kill a % of population?
    public void KillPercentageRobot(float percent)
    {
        int numToKill = (int)((float)GetTotalRobotCount() * percent);

        for (int i = 0; i < numToKill; i++)
        {
            DestroyRandomRobot();
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator WaitToCountCollection()
    {
        yield return new WaitForSeconds(collectionWaitTime);
        UpdateResourceStatus();
    }

    #endregion
}
