// --------------------------------------------------------------
// MoonSim - RockManager                                4/05/2021
// Author(s): Cameron Carstens
// Contact: cameroncarstens@knights.ucf.edu
// --------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour
{

    #region Inspector Fields

    [Header("Setting")]
    [SerializeField]
    private int maxNumRocks;
    [SerializeField]
    private int minNumRocks;
    [SerializeField]
    private int maxSpawnAttempts;
    [SerializeField]
    private float minDistanceBetweenRocks;
    [SerializeField]
    private float minDistanceFromHome;
    [SerializeField]
    private AnimationCurve rockSizeCurve;
    [SerializeField]
    private int xBorderMagnitude;
    [SerializeField]
    private int yBorderMagnitude;

    [Header("Dependencies")]
    [SerializeField]
    private GameObject rock1;
    [SerializeField]
    private GameObject rock2;
    [SerializeField]
    private GameObject rock3;
    [SerializeField]
    private GameObject rock4;
    [SerializeField]
    private GameObject rockParent;
    
    #endregion

    #region Run-Time Fields

    private List<Transform> rocks;

    #endregion

    #region Monobehaviors

    // Start is called before the first frame update
    void Awake()
    {
        rocks = new List<Transform>();
        SpawnRocks();
    }

    #endregion

    #region Private Methods

    private void SpawnRocks()
    {
        int numRocksToSpawn = Random.Range(minNumRocks, maxNumRocks);

        for (int i = 0; i < numRocksToSpawn; i++)
        {
            bool legalPoint = false;
            float xPoint;
            float yPoint;
            int spawnAttempt = 0;
            Vector3 spawnPoint = new Vector3(0,0,0);

            while (legalPoint == false)
            {
                spawnAttempt += 1;
                xPoint = Random.Range(-xBorderMagnitude, xBorderMagnitude);
                yPoint = Random.Range(-yBorderMagnitude, yBorderMagnitude);
                spawnPoint = new Vector3(xPoint, yPoint, 0);
                legalPoint = true;

                foreach (Transform rock in rocks)
                {
                    if (Vector3.Distance(rock.position, spawnPoint) <= minDistanceBetweenRocks)
                    {
                        legalPoint = false;
                        break;
                    }
                }

                List<Transform> homes = WaypointManager.main.GetHomeTransforms();
                foreach (Transform home in homes)
                {
                    if (Vector3.Distance(home.position, spawnPoint) <= minDistanceFromHome)
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

            int typeToSpawn = Random.Range(1, 4);
            float rockScale = Random.value;
            rockScale = rockSizeCurve.Evaluate(rockScale);
            float rockRotation = Random.Range(0, 359);

            switch (typeToSpawn)
            {
                case 1:
                    GameObject newRock1 = Instantiate(rock1, spawnPoint, Quaternion.Euler(0, 0, rockRotation), rockParent.transform);
                    newRock1.GetComponent<Rock>().SetRockSize(rockScale);
                    rocks.Add(newRock1.transform);
                    break;
                case 2:
                    GameObject newRock2 = Instantiate(rock2, spawnPoint, Quaternion.Euler(0, 0, rockRotation), rockParent.transform);
                    newRock2.GetComponent<Rock>().SetRockSize(rockScale);
                    rocks.Add(newRock2.transform);
                    break;
                case 3:
                    GameObject newRock3 = Instantiate(rock3, spawnPoint, Quaternion.Euler(0, 0, rockRotation), rockParent.transform);
                    newRock3.GetComponent<Rock>().SetRockSize(rockScale);
                    rocks.Add(newRock3.transform);
                    break;
                case 4:
                    GameObject newRock4 = Instantiate(rock4, spawnPoint, Quaternion.Euler(0, 0, rockRotation), rockParent.transform);
                    newRock4.GetComponent<Rock>().SetRockSize(rockScale);
                    rocks.Add(newRock4.transform);
                    break;

            }
        }
    }

    #endregion
}
