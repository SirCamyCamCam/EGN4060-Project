// --------------------------------------------------------------
// MoonSim - WaypointManager                          4/20/2021
// Author(s): Cameron Carstens
// Contact: cameroncarstens@knights.ucf.edu
// --------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{

    #region enum

    public enum RobotState
    {
        IDLE,
        DEADBATTERY,
        CHARGING,
        SEARCH,
        TRAVEL,
        COLLECT
    };

    public enum RobotGoal
    {
        RESOURSE,
        CHARGE,
        NONE
    }

    #endregion

    #region Inspector Fields

    [Header("Robot Setting")]
    [SerializeField]
    private float nearbyRobotWeight;

    [Space(15)]
    [Header("Battery Settings")]
    [SerializeField]
    private bool chargeMethod;
    [SerializeField]
    private float drainRate;
    [SerializeField]
    private float drainTime;
    [SerializeField]
    private float chargeRate;
    [SerializeField]
    private float chargeTime;
    [SerializeField]
    private float critialBattery;

    [Space(15)]
    [Header("Travel Setting")]
    [SerializeField]
    private float travelSwitchDistance;
    [SerializeField]
    private float travelNoise;
    [SerializeField]
    private float travelSpeed;
    [SerializeField]
    private float travelRotationSpeed;

    [Space(15)]
    [Header("Search Settings")]
    [SerializeField]
    private float searchSwitchDistance;
    [SerializeField]
    private float searchNoise;
    [SerializeField]
    private float searchSpeed;
    [SerializeField]
    private float searchRotationSpeed;
    [SerializeField]
    private float nearbyRockWeight;

    [Space(15)]
    [Header("Idle Settings")]
    [SerializeField]
    private float idleDistance;
    [SerializeField]
    private float idleSpeed;
    [SerializeField]
    private float idleRotationSpeed;
    [SerializeField]
    private float idleNoise;

    [Space(15)]
    [Header("Dependencies")]
    [SerializeField]
    private GameObject robotGameObject;
    [SerializeField]
    private Transform robotTransform;

    #endregion

    #region RunTime Fields

    private RobotState robotState;
    private RobotGoal robotGoal;
    private ResourceManager.ResourceType targetResourceType;

    private Waypoint targetWaypoint;
    private Waypoint prevWaypoint;
    private Resource targetResource;
    private List<Waypoint> waypointPath;

    // Noise
    private float xDirection;
    private float yDirection;
    private bool isReturningToWaypoint;

    private float battery;
    private Coroutine drainBattery;
    private Coroutine chargeBattery;
    private bool goToCharge;

    private List<Transform> nearbyRobots;
    private List<Transform> nearbyRocks;

    #endregion

    #region Monobehaviors

    private void Awake()
    {
        robotState = RobotState.IDLE;
        robotGoal = RobotGoal.NONE;
        nearbyRobots = new List<Transform>();
    }

    // Initialization
    void Start()
    {
        battery = 1;
        drainBattery = StartCoroutine(DrainBattery());
        targetWaypoint = WaypointManager.main.GetHomeWaypoint();
    }

    void Update()
    {
        DetermineGoal();
        DetermineAction();
        CheckBatteryLevel();
    }

    #endregion

    #region Private Methods

    private void DetermineGoal()
    {
        // Traveling
        if (robotState == RobotState.TRAVEL)
        {
            if (waypointPath != null && waypointPath.Count != 0 && targetWaypoint == null)
            {
                GetNextTargetWaypoint();
            }
            if (targetWaypoint != null)
            {
                if (Vector2.Distance(robotTransform.position, targetWaypoint.ReturnWaypointTransform().position) < travelSwitchDistance)
                {
                    GetNextTargetWaypoint();
                }
            }
            else if (robotGoal == RobotGoal.RESOURSE && prevWaypoint.ReturnWaypointType() != WaypointManager.WaypointType.HOME)
            {
                targetResource = ResourceManager.main.ReturnNearestResource(targetResourceType, robotTransform.position);
                robotState = RobotState.COLLECT;
            }
            else if (robotGoal == RobotGoal.RESOURSE && prevWaypoint.ReturnWaypointType() == WaypointManager.WaypointType.HOME)
            {
                if (goToCharge)
                {
                    GoChargeRobot();
                }
                else
                {
                    robotState = RobotState.IDLE;
                    robotGoal = RobotGoal.NONE;
                }
                // Add to resource manager
            }
            else if (robotGoal == RobotGoal.CHARGE)
            {
                robotState = RobotState.CHARGING;
            }
            else if (robotGoal == RobotGoal.NONE)
            {
                if (goToCharge)
                {
                    GoChargeRobot();
                }
                robotState = RobotState.IDLE;
            }
        }
        // Collecting
        else if (robotState == RobotState.COLLECT)
        {
            if (Vector2.Distance(robotTransform.position, targetResource.ReturnResourceTransform().position) < searchSwitchDistance)
            {
                PickUpResource();
            }
        }
        else if (robotState == RobotState.CHARGING)
        {
            if (CheckIfBatteryFull())
            {
                robotState = RobotState.IDLE;
            }
        }
    }

    private void DetermineAction()
    {
        if (robotState == RobotState.IDLE)
        {
            IdleRobot();
        }
        else if (robotState == RobotState.DEADBATTERY)
        {
            DeadBattery();
        }
        else if (robotState == RobotState.CHARGING)
        {
            if (drainBattery != null)
            {
                StopCoroutine(drainBattery);
            }
            else if (chargeBattery == null)
            {
                chargeBattery = StartCoroutine(ChargeBattery());
            }
            ChargeRobot();
        }
        else if (robotState == RobotState.SEARCH)
        {
            SearchForResource();
        }
        else if (robotState == RobotState.TRAVEL)
        {
            TravelToWaypoint();
        }
        else if (robotState == RobotState.COLLECT)
        {
            TravelToResource();
        }
    }

    private bool CheckIfBatteryFull()
    {
        if (battery == 1)
        {
            return true;
        }
        return false;
    }

    private void AssignTargetWaypoint(Waypoint target)
    {
        prevWaypoint = targetWaypoint;
        targetWaypoint = target;
    }

    private void IdleRobot()
    {
        // Determine whether to turn around
        if (Vector2.Distance(robotTransform.position, targetWaypoint.ReturnWaypointTransform().position) > idleDistance && isReturningToWaypoint == false)
        {
            isReturningToWaypoint = true;
        }
        else if (Vector2.Distance(robotTransform.position, targetWaypoint.ReturnWaypointTransform().position) < idleDistance - 1 && isReturningToWaypoint == true)
        {
            isReturningToWaypoint = false;
        }

        // Noise stuff
        xDirection += 1 * Random.value;
        yDirection += 1 * Random.value;

        if (xDirection == int.MaxValue || yDirection == int.MaxValue)
        {
            xDirection = 0;
            yDirection = 0;
        }

        if (isReturningToWaypoint == false)
        {
            // Apply noise
            float randomVal = Mathf.PerlinNoise(
                xDirection * idleNoise,
                yDirection * idleNoise);
            float angle = Mathf.Lerp(-10, 10, randomVal);
            robotTransform.eulerAngles = new Vector3(0, 0, robotTransform.eulerAngles.z + angle);
        }
        else
        {
            // Turn to waypoint
            Vector3 direction = (targetWaypoint.ReturnWaypointTransform().position - robotTransform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.down);
            robotTransform.rotation = Quaternion.Slerp(robotTransform.rotation, lookRotation, Time.deltaTime * idleRotationSpeed);
            robotTransform.eulerAngles = new Vector3(0, 0, robotTransform.eulerAngles.z);

        }
        // Go forward
        robotTransform.position += robotTransform.up * Time.deltaTime * idleSpeed;
    }

    private void ChargeRobot()
    {
        // Should it do stuff?
    }

    private void TravelToWaypoint()
    {
        if (targetWaypoint != null)
        {
            return;
        }

        // Face direction of target waypoint
        Vector3 direction = (targetWaypoint.ReturnWaypointTransform().position - robotTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.back);
        robotTransform.rotation = Quaternion.Slerp(robotTransform.rotation, lookRotation, Time.deltaTime * travelRotationSpeed);
        // Noise
        xDirection += 1 * Random.value;
        yDirection += 1 * Random.value;

        if (xDirection == int.MaxValue || yDirection == int.MaxValue)
        {
            xDirection = 0;
            yDirection = 0;
        }

        float randomVal = Mathf.PerlinNoise(
                xDirection * travelNoise,
                yDirection * travelNoise);
        float angle = Mathf.Lerp(-2, 2, randomVal);
        robotTransform.eulerAngles = new Vector3(0, 0, robotTransform.eulerAngles.z + angle);

        robotTransform.position += robotTransform.up * Time.deltaTime * travelSpeed;
    }

    private void TravelToResource()
    {
        if (targetResource == null)
        {
            return;
        }

        // Face direction of target waypoint
        Vector3 direction = (targetResource.ReturnResourceTransform().position - robotTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.back);
        robotTransform.rotation = Quaternion.Slerp(robotTransform.rotation, lookRotation, Time.deltaTime * travelRotationSpeed);
        // Noise
        xDirection += 1 * Random.value;
        yDirection += 1 * Random.value;

        if (xDirection == int.MaxValue || yDirection == int.MaxValue)
        {
            xDirection = 0;
            yDirection = 0;
        }

        float randomVal = Mathf.PerlinNoise(
                xDirection * travelNoise,
                yDirection * travelNoise);
        float angle = Mathf.Lerp(-2, 2, randomVal);
        robotTransform.eulerAngles = new Vector3(0, 0, robotTransform.eulerAngles.z + angle);

        robotTransform.position += robotTransform.up * Time.deltaTime * travelSpeed;
    }

    private void GetNextTargetWaypoint()
    {
        prevWaypoint = targetWaypoint;

        if (waypointPath.Count == 0)
        {
            targetWaypoint = null;
            return;
        }

        targetWaypoint = waypointPath[0];
        waypointPath.Remove(targetWaypoint);
        if (prevWaypoint != null)
        {
            List<Waypoint> linesToUnselect = new List<Waypoint>();
            linesToUnselect.Add(targetWaypoint);
            linesToUnselect.Add(prevWaypoint);
            WaypointManager.main.UnselectLines(linesToUnselect);
        }
    }

    private void CheckBatteryLevel()
    {
        if (battery < critialBattery)
        {
            if (!chargeMethod)
            {
                GoChargeRobot();
            }
            else
            {
                goToCharge = true;
            }
        }
        else if (battery <= 0)
        {
            DeadBattery();
        }
    }

    private void SearchForResource()
    {
        // Add all inputs together
        Vector3 robotDirection = robotTransform.right;
        Vector3 nearbyRobotDirection = new Vector3(0, 0, 0);
        Vector3 nearbyRockDirection = new Vector3(0, 0, 0);

        foreach (Transform robot in nearbyRobots)
        {
            nearbyRobotDirection += (robot.position - robotTransform.position).normalized * nearbyRockWeight;
        }
        foreach (Transform rock in nearbyRocks)
        {
            nearbyRockDirection += (rock.position - robotTransform.position).normalized * nearbyRobotWeight;
        }
        Vector3 combinedDirection = (nearbyRobotDirection + nearbyRockDirection).normalized;
        Vector3 direction = (combinedDirection - robotTransform.position).normalized;
    
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.back);
        robotTransform.rotation = Quaternion.Slerp(robotTransform.rotation, lookRotation, Time.deltaTime * searchRotationSpeed);
        // Noise
        xDirection += 1 * Random.value;
        yDirection += 1 * Random.value;

        if (xDirection == int.MaxValue || yDirection == int.MaxValue)
        {
            xDirection = 0;
            yDirection = 0;
        }

        float randomVal = Mathf.PerlinNoise( xDirection * travelNoise, yDirection * travelNoise);
        float angle = Mathf.Lerp(-2, 2, randomVal);
        robotTransform.eulerAngles = new Vector3(0, 0, robotTransform.eulerAngles.z + angle);

        robotTransform.position += robotTransform.up * Time.deltaTime * searchSpeed;
    }
    
    private void DeadBattery()
    {
        RobotManager.main.RobotDEATH(this);
        Destroy(robotGameObject);
    }

    private void PickUpResource()
    {
        Resource resource = ResourceManager.main.ReturnNearestResource(targetResourceType, robotTransform.position);
        resource.RemoveUnit();
        robotState = RobotState.TRAVEL;
        Waypoint nearest = WaypointManager.main.ReturnClosestWaypoint(robotTransform.position);
        List<Waypoint> list = WaypointManager.main.ReturnToHome(nearest);
        AssignWaypointList(list);
        WaypointManager.main.SelectLines(list);

        ResourceManager.main.AddRssToStorage(targetResourceType, 1);
    }

    private void GoChargeRobot()
    {
        robotGoal = RobotGoal.CHARGE;
        if (robotGoal == RobotGoal.RESOURSE)
        {
            RobotManager.main.AddToResourceQueue(targetResourceType);
        }

        Waypoint closestWaypointToRobot = WaypointManager.main.ReturnClosestWaypoint(robotTransform.position);
        List<Waypoint> pathToCharger = WaypointManager.main.PathToChargingWaypoint(closestWaypointToRobot);
        AssignWaypointList(pathToCharger);
        robotState = RobotState.TRAVEL;
        WaypointManager.main.SelectLines(pathToCharger);
    }

    #endregion

    #region Public Methods

    public void AssignWaypointList(List<Waypoint> waypointList)
    {
        waypointPath = waypointList;
    }

    public RobotState GetRobotState()
    {
        return robotState;
    }

    public RobotGoal GetRobotGoal()
    {
        return robotGoal;
    }

    public void AssignRobotGoal(RobotGoal newGoal)
    {
        robotGoal = newGoal;
    }

    public void AssignRobotState(RobotState newState)
    {
        robotState = newState;
    }

    public void AssignTargetResource(Resource resource)
    {
        targetResource = resource;
    }

    public void AssignTargetResourceType(ResourceManager.ResourceType type)
    {
        targetResourceType = type;
    }

    public void AddtoNearbyRobots(Transform robot)
    {
        nearbyRobots.Add(robot);
    }

    public void RemoveNearbyRobots(Transform robot)
    {
        nearbyRobots.Remove(robot);
    }

    public void AddToNearbyRocks(Transform rock)
    {
        nearbyRocks.Add(rock);
    }

    public void RemoveNearbyRocks(Transform rock)
    {
        nearbyRocks.Remove(rock);
    }

    public void FoundResource(Transform resourceTransform)
    {
        Resource resource = resourceTransform.GetComponent<Resource>();

        if (resource.ReturnResourceType() == targetResourceType)
        {
            resource.RemoveUnit();
            robotState = RobotState.TRAVEL;
            Waypoint closeWaypoint = WaypointManager.main.ReturnClosestWaypoint(robotTransform.position);
            List<Waypoint> list = WaypointManager.main.ReturnToHome(closeWaypoint);
            AssignWaypointList(list);
            WaypointManager.main.AddtorememberedWaypoints(closeWaypoint, resource.ReturnResourceType());
            WaypointManager.main.SelectLines(list);
        }
    }

    public Transform ReturnRobotTransform()
    {
        return robotTransform;
    }

    #endregion

    #region Coroutines

    private IEnumerator DrainBattery()
    {
        yield return new WaitForSeconds(drainTime);

        battery -= drainTime;

        if (battery < 0)
        {
            battery = 0;
        }

        drainBattery = StartCoroutine(DrainBattery());
    }

    private IEnumerator ChargeBattery()
    {
        yield return new WaitForSeconds(chargeTime);

        battery += chargeRate;

        if (battery > 1)
        {
            battery = 1;
        }

        chargeBattery = StartCoroutine(ChargeBattery());
    }

    #endregion
}

