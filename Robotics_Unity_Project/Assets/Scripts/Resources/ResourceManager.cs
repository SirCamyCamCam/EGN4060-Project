// --------------------------------------------------------------
// MoonSim - ResourceManager                            4/26/2021
// Author(s): Cameron Carstens, Jonathan Frucht
// Contact: cameroncarstens@knights.ucf.edu, jonfrucht@knights.ucf.edu
// --------------------------------------------------------------


// ADD RssCapacities to addtoStorage
// Add Decay
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    #region Enum

    public enum ResourceType
    {
        IRON, COPPER, TITANIUM, GOLD 
    }

    #endregion

    #region Static Fields

    public static ResourceManager main;

    #endregion

    #region Inspector Fields
    
    [Header("Spawn Setting")]
    // Iniatial settings
    [SerializeField]
    private int minRssAmt;
    [SerializeField]
    private int maxRssAmt;
    [SerializeField]
    private float minDistanceBetweenResources;
    [SerializeField]
    private float minDistanceBetweenRocks;
    [SerializeField]
    private float minDistanceBetweenWaypoints;
    [SerializeField]
    private float minDistanceFromHome;
    [SerializeField]
    private int xBorderMagnitude;
    [SerializeField]
    private int yBorderMagnitude;
    [SerializeField]
    private int maxSpawnAttempts;

    [Space(10)]
    [Header("Resource Settings")]
    [SerializeField]
    private AnimationCurve rssSizeCurve;
    [SerializeField]
    private float resourceRegenChance;
    [SerializeField]
    private int resourceMinEmptyTime;
    [SerializeField]
    private float fetchResourceWaitTime;

    [Space(10)]
    [Header("ResourceUI")]
    public GameObject Panel;
    public TMP_Text resourceUITitle;
    public TMP_Text ironTitle;
    public TMP_Text copperTitle;
    public TMP_Text goldTitle;
    public TMP_Text titaniumTitle;
    public TMP_Text ironText;
    public TMP_Text copperText;
    public TMP_Text goldText;
    public TMP_Text titaniumText;


    [Space(20)]
    [Header("Resource Capacity Settings")]
    [SerializeField]
    private int resourceScale;
    [SerializeField]
    private float ironMinRequiredAmt;
    [SerializeField]
    private int ironStorageCapacity;
    [SerializeField]
    private float ironDeletionRate;
    [SerializeField]
    private float ironDeletionTime;
    [SerializeField]
    private float copperMinRequiredAmt;
    [SerializeField]
    private int copperStorageCapacity;
    [SerializeField]
    private float copperDeletionRate;
    [SerializeField]
    private float copperDeletionTime;
    [SerializeField]
    private float goldMinRequiredAmt;
    [SerializeField]
    private int goldStorageCapacity;
    [SerializeField]
    private float goldDeletionRate;
    [SerializeField]
    private float goldDeletionTime;
    [SerializeField]
    private float titaniumMinRequiredAmt;
    [SerializeField]
    private int titaniumStorageCapacity;
    [SerializeField]
    private float titaniumDeletionRate;
    [SerializeField]
    private float titaniumDeletionTime;

    // Prefabs
    [Space(15)]
    [Header("Dependencies")]
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

    #endregion

    #region Run-Time Fields

    private List<Transform> rssTransformList;

    private List<Resource> resourceList;
    private List<Resource> ironList;
    private List<Resource> copperList;
    private List<Resource> goldList;
    private List<Resource> titaniumList;
    
    private List<Resource> emptyIronList;
    private List<Resource> emptyCopperList;
    private List<Resource> emptyGoldList;
    private List<Resource> emptyTitaniumList;  
    private List<Resource> emptyResourceList;    

    private int frame = 0;
    
    private float ironStoredAmount;
    private float copperStoredAmount;
    private float goldStoredAmount;
    private float titaniumStoredAmount;

    private bool rssSpawned;

    #endregion

    #region Monobehaviors

    private void Awake()
    {
        main = this;
        ironList = new List<Resource>();
        copperList = new List<Resource>();
        goldList = new List<Resource>();
        titaniumList = new List<Resource>();
        resourceList = new List<Resource>();
        emptyIronList = new List<Resource>();
        emptyCopperList = new List<Resource>();
        emptyGoldList = new List<Resource>() ;
        emptyTitaniumList = new List<Resource>();
        emptyResourceList = new List<Resource>();
        ironStoredAmount = 0;
        copperStoredAmount = 0;
        goldStoredAmount = 0;
        titaniumStoredAmount = 0;
        
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
        else if (!rssSpawned)
        {
            rssSpawned = true;
            SpawnResources();
            Debug.Log("Total Iron : " + ironList.Count);
            Debug.Log("Total Copper : " + copperList.Count);
            Debug.Log("Total Gold : " + goldList.Count);
            Debug.Log("Total Titanium : " + titaniumList.Count);
            StartCoroutine(DetermineResourceToGet());
            StartCoroutine(DepleteCopper());
            StartCoroutine(DepleteGold());
            StartCoroutine(DepleteIron());
            StartCoroutine(DepleteTitanium());
            copperText.text = "0 / " + copperStorageCapacity;
            ironText.text = "0 / " + ironStorageCapacity;
            goldText.text = "0 / " + goldStorageCapacity;
            titaniumText.text = "0 / "  + titaniumStorageCapacity;

        }


    }

    #endregion

    #region Private Methods

    private void SpawnResources()
    {
        int numRssToSpawnTot = Random.Range(minRssAmt,maxRssAmt);
        int[] numRssToSpawnIndv = new int[4];

        for (int i = 0; i < numRssToSpawnTot; i++)
        {
            int rssTypeToSpawn = Random.Range(1, 101);
            if (rssTypeToSpawn < 41 )
            {
                numRssToSpawnIndv[0]++;
            }
            else if (rssTypeToSpawn < 71)
            {
                numRssToSpawnIndv[1]++;
            }
            else if (rssTypeToSpawn < 91)
            {
                numRssToSpawnIndv[2]++;
            }
            else if (rssTypeToSpawn <= 101 )
            {
                numRssToSpawnIndv[3]++;
            }
        }

        Debug.Log("Total Resources: " + numRssToSpawnTot);
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
                    if (Vector3.Distance(transform.position, spawnPoint) <= minDistanceBetweenResources)
                    {
                        legalPoint = false;
                        break;
                    } 
                }

                List<Transform> rocks = RockManager.main.rocks;
                foreach (Transform rock in rocks)
                {
                    if (Vector3.Distance(rock.position, spawnPoint) <= minDistanceBetweenRocks)
                    {
                        legalPoint = false;
                        break;
                    }
                }

                List<Transform> waypoints = WaypointManager.main.GetWaypointTransforms();
                foreach (Transform waypoint in waypoints)
                {
                    if (Vector3.Distance(waypoint.position, spawnPoint) <= minDistanceBetweenWaypoints)
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

            // Check if all of a resource has been spawned
            if (numRssToSpawnIndv[x] <= 0)
            {
                if (numRssToSpawnIndv.Length == (x + 1))
                {
                    return;
                }
                x++;
            }

            float rssScale = Random.value;
            rssScale = rssSizeCurve.Evaluate(rssScale);
            float rssRotation = Random.Range(0,359);

            switch (x)
            {
                case 0:
                    GameObject newIron = Instantiate(ironPrefab, spawnPoint, Quaternion.Euler(0, 0, rssRotation), ironParent.transform);
                    Resource newIronResource = newIron.GetComponent<Resource>();
                    newIronResource.SetResourceScale(resourceScale);
                    newIronResource.SetRssSize(rssScale);
                    newIronResource.AssignResourceType(ResourceType.IRON);
                    newIronResource.SetAmount(rssScale);
                    newIronResource.SetReqEmptyTime(resourceMinEmptyTime);
                    newIronResource.SetRegenChance(resourceRegenChance);
                    resourceList.Add(newIronResource);
                    ironList.Add(newIronResource);
                    numRssToSpawnIndv[0]--;
                    break;
                case 1:
                    GameObject newCopper = Instantiate(copperPrefab, spawnPoint, Quaternion.Euler(0, 0, rssRotation), copperParent.transform);
                    Resource newCopperResource = newCopper.GetComponent<Resource>();
                    newCopperResource.SetResourceScale(resourceScale);
                    newCopperResource.SetRssSize(rssScale);
                    newCopperResource.AssignResourceType(ResourceType.COPPER);
                    newCopperResource.SetAmount(rssScale);
                    newCopperResource.SetReqEmptyTime(resourceMinEmptyTime);
                    newCopperResource.SetRegenChance(resourceRegenChance);
                    copperList.Add(newCopperResource);
                    resourceList.Add(newCopperResource);
                    numRssToSpawnIndv[1]--;
                    break;
                case 2:
                    GameObject newGold = Instantiate(goldPrefab, spawnPoint, Quaternion.Euler(0, 0, rssRotation), goldParent.transform);
                    Resource newGoldResource = newGold.GetComponent<Resource>();
                    newGoldResource.SetResourceScale(resourceScale);
                    newGoldResource.SetRssSize(rssScale); // replace with newiron
                    newGoldResource.AssignResourceType(ResourceType.GOLD);
                    newGoldResource.SetAmount(rssScale);
                    newGoldResource.SetReqEmptyTime(resourceMinEmptyTime);
                    newGoldResource.SetRegenChance(resourceRegenChance);
                    goldList.Add(newGoldResource);
                    resourceList.Add(newGoldResource);
                    numRssToSpawnIndv[2]--;
                    break;
                case 3:
                    GameObject newTitanium = Instantiate(titaniumPrefab, spawnPoint, Quaternion.Euler(0, 0, rssRotation), titaniumParent.transform);
                    Resource newTitaniumResource = newTitanium.GetComponent<Resource>();
                    newTitaniumResource.SetResourceScale(resourceScale);
                    newTitaniumResource.SetRssSize(rssScale); 
                    newTitaniumResource.AssignResourceType(ResourceType.TITANIUM);
                    newTitaniumResource.SetAmount(rssScale);
                    newTitaniumResource.SetReqEmptyTime(resourceMinEmptyTime);
                    newTitaniumResource.SetRegenChance(resourceRegenChance);
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
    
    private void AddIronToStorage(int amtToAdd) 
    {

        ironStoredAmount += amtToAdd;
        ironText.text = ironStoredAmount.ToString() + "/" + ironStorageCapacity.ToString();
        if (ironStoredAmount > ironStorageCapacity)
        {
            ironStoredAmount = ironStorageCapacity;
        }    
    }

    private void AddCopperToStorage(int amtToAdd) 
    {
        copperStoredAmount += amtToAdd;
        copperText.text = copperStoredAmount.ToString() + "/" + copperStorageCapacity.ToString();

        if (copperStoredAmount > copperStorageCapacity)
        {
            copperStoredAmount = copperStorageCapacity;
        } 
    }

    private void AddGoldToStorage (int amtToAdd) 
    {
        goldStoredAmount += amtToAdd;
        goldText.text = goldStoredAmount.ToString() + "/" + goldStorageCapacity.ToString();

        if (goldStoredAmount > goldStorageCapacity)
        {
            goldStoredAmount = goldStorageCapacity;
        } 
    }

    private void AddTitaniumToStorage (int amtToAdd) 
    {
        titaniumStoredAmount += amtToAdd;
        titaniumText.text = titaniumStoredAmount.ToString() + "/" + titaniumStorageCapacity.ToString();

        if (titaniumStoredAmount > titaniumStorageCapacity)
        {
            titaniumStoredAmount = titaniumStorageCapacity;
        } 
    }

    #endregion

    #region Public Methods

    public void AddRssToStorage(ResourceManager.ResourceType rssToAdd, int amtToAdd) 
    {
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
        float minDistance = float.MaxValue;
        Resource nearestRss = default(Resource);
        switch (type)
        {
            case (ResourceType.IRON):
            {
                if(ironList.Count != 0)
                {                
                    foreach(Resource rss in ironList)
                    {
                        Transform transformResource = rss.ReturnResourceTransform();
                        Vector2 rssPosition = transformResource.position;
                        float compareDist = Vector2.Distance(position, rssPosition);
                        if (compareDist < minDistance)
                        {
                            minDistance = compareDist;
                            nearestRss = rss;
                        }
                    }
                }
                break;
            }
            case (ResourceType.COPPER):
            {
                if(copperList.Count != 0)
                {
                 
                    foreach(Resource rss in copperList)
                    {
                        Transform transformResource = rss.ReturnResourceTransform();
                        Vector2 rssPosition = transformResource.position;
                        float compareDist = Vector2.Distance(position, rssPosition);
                        if (compareDist < minDistance)
                        {
                            minDistance = compareDist;
                            nearestRss = rss;
                        }
                    }
                }
                break;
            }
            
            case (ResourceType.TITANIUM):
            {
                if(titaniumList.Count != 0)
                {
                    foreach(Resource rss in titaniumList)
                    {
                        Transform transformResource = rss.ReturnResourceTransform();
                        Vector2 rssPosition = transformResource.position;
                        float compareDist = Vector2.Distance(position, rssPosition);
                        if (compareDist < minDistance)
                        {
                            minDistance = compareDist;
                            nearestRss = rss;
                        }
                    }
                }
                break;
            }
            
            case (ResourceType.GOLD):
            {
                if(goldList.Count != 0)
                {
                    foreach(Resource rss in goldList)
                    {
                        Transform transformResource = rss.ReturnResourceTransform();
                        Vector2 rssPosition = transformResource.position;
                        float compareDist = Vector2.Distance(position, rssPosition);
                        if (compareDist < minDistance)
                        {
                            minDistance = compareDist;
                            nearestRss = rss;
                        }
                    }
                }
                break;
            }
        }
        return nearestRss;
    }

    public void RemoveResource(Resource resource_to_remove )
    {
        ResourceManager.ResourceType resource_type = resource_to_remove.ReturnResourceType();
        Transform transformResource = resource_to_remove.ReturnResourceTransform();

        WaypointManager.main.RemoveRememberedPath(resource_type, resource_to_remove);

        resourceList.Remove(resource_to_remove);
        emptyResourceList.Add(resource_to_remove);

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

    public void Regenerated(Resource regened)
    {
        ResourceType type = regened.ReturnResourceType();

        emptyResourceList.Remove(regened);

        switch (type)
        {
            case ResourceType.IRON:
                ironList.Add(regened);
                resourceList.Add(regened);
                emptyIronList.Remove(regened);
                break;
            case ResourceType.COPPER:
                copperList.Add(regened);
                resourceList.Add(regened);
                emptyCopperList.Remove(regened);
                break;
            case ResourceType.GOLD:
                goldList.Add(regened);
                resourceList.Add(regened);
                emptyGoldList.Remove(regened);
                break;
            case ResourceType.TITANIUM:
                titaniumList.Add(regened);
                resourceList.Add(regened);
                emptyTitaniumList.Remove(regened);
                break;
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator DetermineResourceToGet()
    {
        if (ironStoredAmount < ironMinRequiredAmt)
        {
            RobotManager.main.AddToResourceQueue(ResourceType.IRON);
        }
        if (copperStoredAmount < copperMinRequiredAmt)
        {
            RobotManager.main.AddToResourceQueue(ResourceType.COPPER);
        }
        if (titaniumStoredAmount < titaniumMinRequiredAmt)
        {
            RobotManager.main.AddToResourceQueue(ResourceType.TITANIUM);
        }
        if (goldStoredAmount < goldMinRequiredAmt)
        {
            RobotManager.main.AddToResourceQueue(ResourceType.GOLD);
        }

        yield return new WaitForSeconds(fetchResourceWaitTime);
        StartCoroutine(DetermineResourceToGet());
    }

    private IEnumerator DepleteIron()
    {
        yield return new WaitForSeconds(ironDeletionTime);

        ironStoredAmount -= ironDeletionRate;
        // ironText.text = ironStoredAmount.ToString() + "/" + ironStorageCapacity.ToString();
        StartCoroutine(DepleteIron());
    }

    private IEnumerator DepleteCopper()
    {
        yield return new WaitForSeconds(copperDeletionTime);

        copperStoredAmount -= copperDeletionRate;

        StartCoroutine(DepleteCopper());
    }

    private IEnumerator DepleteGold()
    {
        yield return new WaitForSeconds(goldDeletionTime);

        goldStoredAmount -= goldDeletionRate;

        StartCoroutine(DepleteGold());
    }

    private IEnumerator DepleteTitanium()
    {
        yield return new WaitForSeconds(titaniumDeletionTime);

        titaniumStoredAmount -= titaniumDeletionRate;

        StartCoroutine(DepleteTitanium());
    }

    #endregion
}