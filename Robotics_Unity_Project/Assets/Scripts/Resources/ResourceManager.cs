// --------------------------------------------------------------
// MoonSim - ResourceManager                            4/26/2021
// Author(s): Cameron Carstens, Jonathan Frucht
// Contact: cameroncarstens@knights.ucf.edu, jonfrucht@knights.ucf.edu
// --------------------------------------------------------------


// ADD RssCapacities to addtoStorage
// Add Decay
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
    
    [Header("Resource Settings")]
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
    [SerializeField]
    private int resourceEmptyTime; // Rename 
    [SerializeField]
    private float resourceRegenChance;
    [SerializeField]
    private int ironNodeMinReq;
    [SerializeField]
    private int copperNodeMinReq;
    [SerializeField]
    private int goldNodeMinReq;
    [SerializeField]
    private int titaniumNodeMinReq;
    
    [Space(20)]

    [Header("Spawn Rate Settings")]
    private int waitTime;
    [SerializeField]
    private float setIronSpawnRate;
    [SerializeField]
    private float setCopperSpawnRate;
    
    [SerializeField]
    private float setGoldSpawnRate;
    [SerializeField]
    private float setTitaniumSpawnRate;
    [Header("Depletion Settings")]
    [SerializeField]
    private float ironDepeleteRate;
    [SerializeField]
    private float copperDepeleteRate;
    [SerializeField]
    private float goldDepeleteRate;
    [SerializeField]
    private float titaniumDepeleteRate;

    [Header("Resource Capacity Settings")]
    // Minimum required amount
    private float ironMinAmt;
    [SerializeField]
    private float copperMinAmt;
    [SerializeField]
    private float goldMinAmt;
    [SerializeField]
    private float titaniumMinAmt;
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
    // List for empty Rss     
     private List<Resource> emptyIronList;
    private List<Resource> emptyCopperList;
    private List<Resource> emptyGoldList;
    private List<Resource> emptyTitaniumList;  
    private List<Resource> emptyResourceList;    

    private int frame = 0;

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
        // create rss lists
        ironList = new List<Resource>();
        copperList = new List<Resource>();
        goldList = new List<Resource>();
        titaniumList = new List<Resource>();
        resourceList = new List<Resource>();
        // create empty lists
        emptyIronList = new List<Resource>();
        emptyCopperList = new List<Resource>();
        emptyGoldList = new List<Resource>() ;
        emptyTitaniumList = new List<Resource>();
        emptyResourceList = new List<Resource>();
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
        if (frame < 2)
        {
            frame++;
        }
        else if (!rssSpawned)
        {
            rssSpawned = true;
            // Spawn Resources
            SpawnResources();
            Debug.Log("Num Iron : " + ironList.Count);
            Debug.Log("Num Copper : " + copperList.Count);
            Debug.Log("Num Gold : " + goldList.Count);
            Debug.Log("Num Titanium : " + titaniumList.Count);
            // Start Coroutine here
            // return? to avoid the below activating
        }
        else 
        {
            // Decay the amount of resources in storage 
            ironAMT -= ironDepeleteRate;
            copperAMT -= copperDepeleteRate;
            goldAMT -= goldDepeleteRate;
            titaniumAMT -= titaniumDepeleteRate;
            // Check if under min reqs
            if (ironAMT < ironMinAmt)
            {
                // Enque iron 
                RobotManager.main.AddToResourceQueue(ResourceType.IRON);
            }
            if (copperAMT < copperMinAmt)
            {
                // Enque iron 
                RobotManager.main.AddToResourceQueue(ResourceType.COPPER);
            }
            if (goldAMT < goldMinAmt)
            {
                // Enque iron 
                RobotManager.main.AddToResourceQueue(ResourceType.GOLD);
            }
            if (titaniumAMT < titaniumMinAmt)
            {
                // Enque iron 
                RobotManager.main.AddToResourceQueue(ResourceType.TITANIUM);
            }
            // enque to rssque 

        }
        // Rss decay 

        // If Rss is low, add X amount of RssType to queue 
        // 
    }
    // If IronAmtofRocks < 6 -> 
    // TimeEmpty = StartTime, After X seconds, { DoIRegen? -> Random #}
    // Rand : 1/100 -> +1, 2/100 
    private void FixedUpdate()
    {
        // Ask cam if x frames == 1 second
    }

    #endregion

    #region Coroutines

   

    private IEnumerator CheckSpawnedResourceAmount()
    {
        yield return new WaitForSeconds(waitTime);
        if (ironList.Count > ironNodeMinReq)
        {
            bool regen;
            foreach (Resource rss in emptyIronList)
            {
                regen = rss.DoIRegen();
                if (regen)
                {
                    emptyIronList.Remove(rss);
                    emptyResourceList.Remove(rss);
                    ironList.Add(rss);
                    resourceList.Add(rss);
                }
            }
        }
        if (copperList.Count > copperNodeMinReq)
        {
            bool regen;
            foreach (Resource rss in emptyCopperList)
            {
                regen = rss.DoIRegen();
                if (regen)
                {
                    emptyCopperList.Remove(rss);
                    emptyResourceList.Remove(rss);
                    copperList.Add(rss);
                    resourceList.Add(rss);
                }
            }
        }
        if (goldList.Count > goldNodeMinReq)
        {
            bool regen;
            foreach (Resource rss in emptyGoldList)
            {
                regen = rss.DoIRegen();
                if (regen)
                {
                    emptyGoldList.Remove(rss);
                    emptyResourceList.Remove(rss);
                    goldList.Add(rss);
                    resourceList.Add(rss);
                }
            }
        }
        if (titaniumList.Count > titaniumNodeMinReq)
        {
            bool regen;
            foreach (Resource rss in emptyTitaniumList)
            {
                regen = rss.DoIRegen();
                if (regen)
                {
                    emptyTitaniumList.Remove(rss);
                    emptyResourceList.Remove(rss);
                    titaniumList.Add(rss);
                    resourceList.Add(rss);
                }
            }
        } 
    }

    private IEnumerator UpdateResourceEmptyTime()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            foreach(Resource rss in emptyResourceList)
            {
                rss.UpdateEmptyTime();
            }
        }
        
    }
    // If Rss is below the req min amout of active nodes
    // when node is empty, start timer, Which allows refil in time limit
    // if (timerReq is hit or SpecialFlag is true)
    // if (specialflagtimer is within limit )
    // else specical is false
    // when rss node total is below min, and its past the timeEmpty req
    // -> 1/100 > 2/100 


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
                    newIronResource.SetAmount();
                    newIronResource.SetReqEmptyTime(resourceEmptyTime);
                    newIronResource.setRegenChance(resourceRegenChance);
                    resourceList.Add(newIronResource);
                    numRssToSpawnIndv[0]--;
                    break;
                case 1:
                Debug.Log("Spawning Copper Rss");
                    GameObject newCopper = Instantiate(copperPrefab, spawnPoint, Quaternion.Euler(0, 0, rssRotation), copperParent.transform);
                    Resource newCopperResource = newCopper.GetComponent<Resource>(); // replace with newiron .SetRssSize(rssScale);
                    newCopperResource.SetRssSize(rssScale); // replace with newiron
                    newCopperResource.AssignResourceType(ResourceType.COPPER);
                    newCopperResource.SetAmount();
                    newCopperResource.SetReqEmptyTime(resourceEmptyTime);
                    newCopperResource.setRegenChance(resourceRegenChance);
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
                    newGoldResource.SetAmount();
                    newGoldResource.SetReqEmptyTime(resourceEmptyTime);
                    newGoldResource.setRegenChance(resourceRegenChance);
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
                    newTitaniumResource.SetAmount();
                    newTitaniumResource.SetReqEmptyTime(resourceEmptyTime);
                    newTitaniumResource.setRegenChance(resourceRegenChance);
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
        if (ironAMT > 10)
        {
            Debug.Log("Iron over capacity, " + ( ironAMT-amtToAdd) + " has gone to waste");
            ironAMT = 10.0f;
        }    
    }
    private void AddCopperToStorage(float amtToAdd) 
    {
        copperAMT += amtToAdd;
        Debug.Log(amtToAdd + " " + copperAMT + " added to storage");
        if (copperAMT > 10)
        {
            Debug.Log("Copper over capacity, " + ( copperAMT-amtToAdd) + " has gone to waste");
            copperAMT = 10.0f;
        } 
    }
    private void AddGoldToStorage (float amtToAdd) 
    {
        goldAMT += amtToAdd;
        Debug.Log(amtToAdd + " " + goldAMT + " added to storage");
        if (goldAMT > 10)
        {
            Debug.Log("Gold over capacity, " + ( goldAMT-amtToAdd) + " has gone to waste");
            goldAMT = 10.0f;
        } 
    }
    private void AddTitaniumToStorage (float amtToAdd) 
    {
        titaniumAMT += amtToAdd;
        Debug.Log(amtToAdd + " " + titaniumAMT + " added to storage");
        if (titaniumAMT > 10)
        {
            Debug.Log("Titanium over capacity, " + ( titaniumAMT-amtToAdd) + " has gone to waste");
            titaniumAMT = 10.0f;
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
        // return null;// for this type loop through every resource and find cloest to the position given vector2.distance
        float minDistance = 0;
        Resource nearestRss = default(Resource);
        switch (type)
        {
            case (ResourceType.IRON) :
            {
                if(ironList.Count != 0)
                {                
                    foreach(Resource rss in ironList)
                    {
                        Transform transformResource = rss.ReturnResourceTransform();
                        Vector2 rssPosition = transformResource.position;
                        float compareDist = Vector2.Distance(position,rssPosition);
                        if (compareDist < minDistance)
                        {
                            minDistance = compareDist;
                            nearestRss = rss;
                        }
                    }
                }
                return nearestRss;
            }
            case (ResourceType.COPPER) :
            {
                if(copperList.Count != 0)
                {
                 
                    foreach(Resource rss in copperList)
                    {
                        Transform transformResource = rss.ReturnResourceTransform();
                        Vector2 rssPosition = transformResource.position;
                        float compareDist = Vector2.Distance(position,rssPosition);
                        if (compareDist < minDistance)
                        {
                            minDistance = compareDist;
                            nearestRss = rss;
                        }
                    }
                }
                return nearestRss;
                
            }
            
            case (ResourceType.TITANIUM) :
            {
                if(titaniumList.Count != 0)
                {
                 
                    foreach(Resource rss in titaniumList)
                    {
                        Transform transformResource = rss.ReturnResourceTransform();
                        Vector2 rssPosition = transformResource.position;
                        float compareDist = Vector2.Distance(position,rssPosition);
                        if (compareDist < minDistance)
                        {
                            minDistance = compareDist;
                            nearestRss = rss;
                        }
                    }
                }
                return nearestRss;
                
            }
            
            case (ResourceType.GOLD) :
            {
                if(goldList.Count != 0)
                {
                 
                    foreach(Resource rss in goldList)
                    {
                        Transform transformResource = rss.ReturnResourceTransform();
                        Vector2 rssPosition = transformResource.position;
                        float compareDist = Vector2.Distance(position,rssPosition);
                        if (compareDist < minDistance)
                        {
                            minDistance = compareDist;
                            nearestRss = rss;
                        }
                    }
                }
                return nearestRss;
            }
            // default:
            // {Debug.Log("Error: Rss type did not match in 'ReturnNearestResource' ");
            // break;}
        }
        return nearestRss;
    
    }

    public void RemoveResource(Resource resource_to_remove ) // Call returnclosestwaypoint to that resource & call removerememberd path 
    {
        ResourceManager.ResourceType resource_type = resource_to_remove.ReturnResourceType();
        // GameObject tr = resource_to_remove.GetComponent<Resource>
        Transform transformResource = resource_to_remove.ReturnResourceTransform();
        Waypoint closestWaypoint = WaypointManager.main.ReturnClosestWaypoint(transformResource.position);
        WaypointManager.main.RemoveRememberedPath(resource_type, closestWaypoint );
        // Remove from Resource list & add to empty resource list
        resourceList.Remove(resource_to_remove);
        emptyResourceList.Add(resource_to_remove);
        // Toggle Empty Sprite

        // Remove from specific list
        switch (resource_type)
        {
            case ResourceType.IRON :
            {
                ironList.Remove(resource_to_remove);
                emptyIronList.Add(resource_to_remove);
                break;
            }
            case ResourceType.COPPER :
            {
                copperList.Remove(resource_to_remove);
                emptyCopperList.Add(resource_to_remove);
                break;
            }
            case ResourceType.GOLD :
            {
                goldList.Remove(resource_to_remove);
                emptyGoldList.Add(resource_to_remove);
                break;
            }
            case ResourceType.TITANIUM :
            {
                titaniumList.Remove(resource_to_remove);
                emptyTitaniumList.Add(resource_to_remove);
                break;
            }
            
        }


    }
    #endregion

   
}

