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


    #endregion

    #region Static Fields

    // Public
    public static RobotManager main;

    // Private

    #endregion

    #region Inspector Fields

    // Stuff
    [Header("Robot Settings")]
    [SerializeField]
    private float spawnRate;
    [SerializeField]
    private int maxRobots;
    [SerializeField]
    private int minRobots;

    [Header("Dependencies")]
    [SerializeField]
    private GameObject robotPrefab;
    [SerializeField]
    private GameObject robotManager;

    #endregion

    #region Run-Time Fields

    private List<Robot> robotList;
    private int finalSpawnRobotNum;
    // Resources
    private float currentResources;
    private Queue<ResourceManager.ResourceType> resourceQueue;

    private int frame = 0;
    private bool stopSpawning = false;

    #endregion

    #region Monobehaviors

    private void Awake()
    {
        main = this;
        robotList = new List<Robot>();
        resourceQueue = new Queue<ResourceManager.ResourceType>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (frame < 3)
        {
            frame++;
        }
        else if (!stopSpawning)
        {
            stopSpawning = true;
            SpawnInitialRobots();
            StartCoroutine(SpawnRobotsAtSetRate());
        }

        if (resourceQueue.Count != 0)
        {
            RemoveFromResourceQueue();
        }
    }

    private void FixedUpdate()
    {

    }

    #endregion

    #region Private Methods

    private void RemoveFromResourceQueue()
    {
        foreach (Robot robot in robotList)
        {
            if (robot.GetRobotState() == Robot.RobotState.IDLE)
            {
                ResourceManager.ResourceType typeToGet = resourceQueue.Dequeue();
                TellRobotToGetResource(robot, typeToGet);
            }
        }
    }

    private void TellRobotToGetResource(Robot robot, ResourceManager.ResourceType type)
    {
        Waypoint closestToResource = WaypointManager.main.GetRememberedPath(type);
        Waypoint closetToRobot = WaypointManager.main.ReturnClosestWaypoint(robot.ReturnRobotTransform().position);
        List<Waypoint> path = WaypointManager.main.PathToWaypoint(closetToRobot, closestToResource);
        WaypointManager.main.SelectLines(path);

        robot.AssignWaypointList(path);
        robot.AssignTargetResourceType(type);
        robot.AssignRobotGoal(Robot.RobotGoal.RESOURSE);
        robot.AssignRobotState(Robot.RobotState.TRAVEL);
    }

    private void SpawnInitialRobots()
    {
        finalSpawnRobotNum = Random.Range(minRobots, maxRobots);

        for (int i = 0; i < finalSpawnRobotNum; i++)
        {
            SpawnRobot();
        }
    }

    private void MaintainRobotPopulation()
    {
        if (finalSpawnRobotNum > robotList.Count)
        {
            SpawnRobot();
        }
    }

    private void SpawnRobot()
    {
        List<Transform> homeList = WaypointManager.main.GetHomeTransforms();
        GameObject newRobot = Instantiate(robotPrefab, homeList[0].position, Quaternion.identity, robotManager.transform);
        Robot robotScript = newRobot.GetComponent<Robot>();
        robotList.Add(robotScript);
    }

    private void DestroyRandomRobot()
    {
        int rnd = Random.Range(0, robotList.Count - 1);
        int currentCount = robotList.Count;
        if (currentCount == 1)
        {
            return;
        }
        KillRobot(robotList[rnd]);
    }

    #endregion

    #region Public Methods

    public void RobotDEATH(Robot robot)
    {
        if (robot == null)
        {
            return;
        }
        robotList.Remove(robot);
    }

    public void AddToResourceQueue(ResourceManager.ResourceType type)
    {
        resourceQueue.Enqueue(type);
    }

    public void KillRobot(Robot robot)
    {
        if (robot == null)
        {
            return;
        }
        robotList.Remove(robot);
        Destroy(robot);
    }

    // Do we need to randomly kill a % of population?
    public void KillPercentageRobot(float percent)
    {
        int numToKill = (int)((float)robotList.Count * percent);

        for (int i = 0; i < numToKill; i++)
        {
            DestroyRandomRobot();
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator SpawnRobotsAtSetRate()
    {
        yield return new WaitForSeconds(spawnRate);

        MaintainRobotPopulation();

        StartCoroutine(SpawnRobotsAtSetRate());
    }

    #endregion
}
