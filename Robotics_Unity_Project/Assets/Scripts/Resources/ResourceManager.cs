// --------------------------------------------------------------
// MoonSim - ResourceManager                            4/26/2021
// Author(s): Cameron Carstens, Jonathan Frucht
// Contact: cameroncarstens@knights.ucf.edu, jonfrucht@knights.ucf.edu
// --------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    #region Inner Classes

    // Stuff

    #endregion

    #region Enum

    public enum ResourceType
    {
        IRON, COPPER, TITANIUM, GOLD 
    }

    #endregion

    #region Static Fields

    // Public
    public static ResourceManager main;

    // Private

    #endregion

    #region Inspector Fields
    
    [Header("Resourse Settings")]
    // Iniatial settings
    [SerializeField]
    private int minRssAmt;
    [SerializeField]
    private int maxRssAmt;
    [SerializeField]
    private float minDistanceBetweenObj;
    [SerializeField]
    private float maxDistanceBetweenObj;
     [SerializeField]
    private int xBorderMagnitude;
    [SerializeField]
    private int yBorderMagnitude;
    [SerializeField]
    private int maxSpawnAttempts;
    [SerializeField]
    private float minDistanceFromHome;
    [SerializeField]
    private AnimationCurve rssSizeCurve;
    // Spawn Rates
    [SerializeField]
    private float setIronSpawnRate;
    [SerializeField]
    private float setCopperSpawnRate;
    [SerializeField]
    private float setTitaniumSpawnRate;
    [SerializeField]
    private float setGoldSpawnRate;
    // Capacity settings
    [SerializeField]
    private int ironCapacity;
    [SerializeField]
    private int copperCapacity;
    [SerializeField]
    private int goldCapacity;
    [SerializeField]
    private int titaniumCapacity;
    // Prefabs
    [Header("Prefabs")]
    [SerializeField]
    private GameObject ironPrefab;
    [SerializeField]
    private GameObject copperPrefab;
    [SerializeField]
    private GameObject goldPrefab;
    [SerializeField]
    private GameObject titaniumPrefab;
    [SerializeField]
    private GameObject ironParent;
    [SerializeField]
    private GameObject copperParent;
    [SerializeField]
    private GameObject goldParent;
    [SerializeField]
    private GameObject titaniumParent;
    [SerializeField]
    private GameObject resourceParent;
    
    
    // Stuff

    #endregion

    #region Run-Time Fields
    // resource locations
    private List<Resource> resourceList;
    private List<Resource> ironList;
    private List<Resource> copperList;
    private List<Resource> goldList;
    private List<Resource> titaniumList;
    // Or 
    // private List<Transform> ironList;
    // private List<Transform> copperList;
    // private List<Transform> goldList;
    // private List<Transform> titaniumList;
    
    // stored resource amounts
    private float ironAMT;
    private float copperAMT;
    private float goldAMT;
    private float titaniumAMT;
    // 
    private bool rssSpawned;

    // Stuff

    #endregion

    #region Monobehaviors

    private void Awake()
    {
        main = this;
        // create empty lists
        ironList = new List<Resource>();
        copperList = new List<Resource>();
        goldList = new List<Resource>() ;
        titaniumList = new List<Resource>();
        resourceList = new List<Resource>();
        // create new storage amt
        ironAMT = 0;
        copperAMT = 0;
        goldAMT = 0;
        titaniumAMT = 0;
        // assign spawn rates

        


    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!rssSpawned)
        {
            rssSpawned = true;
            // Spawn Resources
            SpawnResources();
            Debug.Log("Num Iron : " + ironList.Count);
            Debug.Log("Num Copper : " + copperList.Count);
            Debug.Log("Num Gold : " + goldList.Count);
            Debug.Log("Num Titanium : " + titaniumList.Count);



        }
        
    }

    private void FixedUpdate()
    {
        
    }

    #endregion

    #region Private Methods
    // Init spawn
    private void SpawnResources()
    {
        int numRssToSpawnTot = Random.Range(minRssAmt,maxRssAmt);
        // numRssToSpawnIndv is an array to show how much of each rss will spawn
        // 0 - Iron , 1 - Copper, 2 - Gold, 3 - Titanium
        int[] numRssToSpawnIndv = new int[4];
        float ironSpawnRate = setIronSpawnRate;
        float copperSpawnRate = setCopperSpawnRate + ironSpawnRate;
        float goldSpawnRate = setGoldSpawnRate + copperSpawnRate;
        float titaniumSpawnRate = setTitaniumSpawnRate + goldSpawnRate;
        for (int i = 0; i < numRssToSpawnTot; i++)
        {
            // Randomize spawn amounts
            // Need to fix
            float Rand = Random.value;
            if (Rand < ironSpawnRate)
            {
                numRssToSpawnIndv[0]++;
                continue;
            }
            if (Rand < copperSpawnRate)
            {
                numRssToSpawnIndv[1]++;
                continue;
            }
            if (Rand < goldSpawnRate)
            {
                numRssToSpawnIndv[2]++;
                continue;
            }
            if (Rand < titaniumSpawnRate)
            {
                numRssToSpawnIndv[3]++;
                continue;
            }
        }
        Debug.Log("Amount of each Rss to spawn: " + numRssToSpawnIndv);
        int x = 0;
        for (int i = 0; i < numRssToSpawnTot; i++)
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

                foreach (Resource rss in resourceList)
                {
                    if (Vector3.Distance(transform.position, spawnPoint) <= minDistanceBetweenObj)
                    {
                        legalPoint = false;
                        break;
                    } 
                }

                // List<Transform> homes = WaypointManager.main.GetHomeTransforms();
                // foreach (Transform home in homes)
                // {
                //     if (Vector3.Distance(home.position, spawnPoint) <= minDistanceFromHome)
                //     {
                //         legalPoint = false;
                //         break;
                //     }
                // }

                if (spawnAttempt == maxSpawnAttempts)
                {
                    return;
                }
            }
            // Check if all of a resource has been spawned
            if (numRssToSpawnIndv[x] <= 0)
            {
                x++;
            }
            
            float rssScale = Random.value;
            rssScale = rssSizeCurve.Evaluate(rssScale);
            float rssRotation = Random.Range(0,359);

            switch (x)
            {
                case 0:
                    Debug.Log("Spawning Iron Rss");
                    GameObject newIron = Instantiate(ironPrefab, spawnPoint, Quaternion.Euler(0, 0, rssRotation), ironParent.transform);
                    Resource newIronResource = newIron.GetComponent<Resource>(); // replace with newiron .SetRssSize(rssScale);
                    newIronResource.SetRssSize(rssScale); // replace with newiron
                    ironList.Add(newIronResource);
                    newIronResource.AssignResourceType(ResourceType.IRON);
                    resourceList.Add(newIronResource);
                    numRssToSpawnIndv[0]--;
                    break;
                case 1:
                Debug.Log("Spawning Copper Rss");
                    GameObject newCopper = Instantiate(copperPrefab, spawnPoint, Quaternion.Euler(0, 0, rssRotation), copperParent.transform);
                    Resource newCopperResource = newCopper.GetComponent<Resource>(); // replace with newiron .SetRssSize(rssScale);
                    newCopperResource.SetRssSize(rssScale); // replace with newiron
                    newCopperResource.AssignResourceType(ResourceType.COPPER);
                    copperList.Add(newCopperResource);
                    resourceList.Add(newCopperResource);                                        
                    numRssToSpawnIndv[1]--;
                    break;
                case 2:
                Debug.Log("Spawning Gold Rss");
                    GameObject newGold = Instantiate(goldPrefab, spawnPoint, Quaternion.Euler(0, 0, rssRotation), goldParent.transform);
                    Resource newGoldResource = newGold.GetComponent<Resource>(); // replace with newiron .SetRssSize(rssScale);
                    newGoldResource.SetRssSize(rssScale); // replace with newiron
                    newGoldResource.AssignResourceType(ResourceType.GOLD);
                    goldList.Add(newGoldResource);
                    resourceList.Add(newGoldResource);
                    numRssToSpawnIndv[2]--;
                    break;
                case 3:
                Debug.Log("Spawning Titanium Rss");
                    GameObject newTitanium = Instantiate(titaniumPrefab, spawnPoint, Quaternion.Euler(0, 0, rssRotation), titaniumParent.transform);
                    Resource newTitaniumResource = newTitanium.GetComponent<Resource>(); // replace with newiron .SetRssSize(rssScale);
                    newTitaniumResource.SetRssSize(rssScale); // replace with newiron
                    newTitaniumResource.AssignResourceType(ResourceType.TITANIUM);
                    titaniumList.Add(newTitaniumResource);
                    resourceList.Add(newTitaniumResource);
                    numRssToSpawnIndv[3]--;

                    break;
                default:
                    Debug.Log("Error 'SpawnResources', X escaped the bounds of the array");
                    break;
                
            }


        }
       

    }
    // Add to storage
     private void AddIronToStorage(float amtToAdd) 
    {

        ironAMT += amtToAdd;
        Debug.Log(amtToAdd + " " + ironAMT + " added to storage");
        if (ironAMT > 1)
        {
            Debug.Log("Iron over capacity, " + amtToAdd + " has gone to waste");
            ironAMT = 1.0f;
        }    
    }
    private void AddCopperToStorage(float amtToAdd) 
    {
        copperAMT += amtToAdd;
        Debug.Log(amtToAdd + " " + copperAMT + " added to storage");
        if (copperAMT > 1)
        {
            Debug.Log("Copper over capacity, " + amtToAdd + " has gone to waste");
            copperAMT = 1.0f;
        } 
    }
    private void AddGoldToStorage (float amtToAdd) 
    {
        goldAMT += amtToAdd;
        Debug.Log(amtToAdd + " " + goldAMT + " added to storage");
        if (goldAMT > 1)
        {
            Debug.Log("Gold over capacity, " + amtToAdd + " has gone to waste");
            goldAMT = 1.0f;
        } 
    }
    private void AddTitaniumToStorage (float amtToAdd) 
    {
        titaniumAMT += amtToAdd;
        Debug.Log(amtToAdd + " " + titaniumAMT + " added to storage");
        if (titaniumAMT > 1)
        {
            Debug.Log("Titanium over capacity, " + amtToAdd + " has gone to waste");
            titaniumAMT = 1.0f;
        } 
    }
    // Stuff

    #endregion

    #region Public Methods
    // Add resource to storage
    public void AddRssToStorage(ResourceManager.ResourceType rssToAdd,float amtToAdd) 
    {
        Debug.Log("Adding " + amtToAdd + " to the " + rssToAdd + " storage");    
        switch (rssToAdd)
        {
            case (ResourceType.IRON) :
            {
                AddIronToStorage(amtToAdd);
                break;
            }
            case (ResourceType.COPPER) :
            {
                AddCopperToStorage(amtToAdd);
                break;
            }
            
            case (ResourceType.TITANIUM) :
            {
                AddGoldToStorage (amtToAdd);
                break;
            }
            
            case (ResourceType.GOLD) :
            {
                AddTitaniumToStorage (amtToAdd);
                break;
            }
            
            default:
                Debug.Log("Error: rssToAdd did not match any type in 'AddRssToStorage' in ResourceManager");
                break;
        }
    }

    public Resource ReturnNearestResource(ResourceManager.ResourceType type, Vector2 position)
    {
        return null;// for this type loop through every resource and find cloest to the position given
    }

    #endregion

    #region Coroutines



    #endregion
}

