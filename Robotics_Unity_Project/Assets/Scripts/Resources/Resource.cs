////////////


// --------------------------------------------------------------
// MoonSim - Resource                            4/26/2021
// Author(s): Jonathan Frucht
// Contact:  jonfrucht@knights.ucf.edu
// --------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Ask cam which fields empty time and spawn go into
public class Resource : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField]
    private Transform resourceTransform;
    [SerializeField]
    private float resourceSizeScale;

    #endregion

    #region Run-Time Fields

    private ResourceManager.ResourceType type;
    private Waypoint waypoint;
    private int amount;
    private float emptyTime;
    private float reqEmptyTime;
    private float regenChance;
    private float checkRegenTime;
    private int resourceScale;
    private float intialSize;

    #endregion

    #region Monobehaviors 

    private void Start()
    {
        emptyTime = 0;
    }

    #endregion

    #region Private Methods

    private void Regenerate()
    {

    }

    private void DecreaseSize()
    {

    }

    #endregion

    #region Public Methods

    public void AddWaypoint(Waypoint newwaypoint)
    {
        waypoint = newwaypoint;
    }

    public Waypoint ReturnWaypoint()
    {
        return waypoint;
    }

    public void SetResourceScale(int scale)
    {
        resourceScale = scale;
    }

    public void SetAmount(float value)
    {
        amount = (int)(value * resourceScale) + 1;
        if (amount < 1)
        {
            amount = 1;
        }
    }

    public void SetReqEmptyTime(int newreqEmptyTime)
    {
        reqEmptyTime = newreqEmptyTime;
    }

    public void SetRegenChance(float newregenChance)
    {
        regenChance = newregenChance;
    }

    public Transform ReturnResourceTransform() 
    {
        return resourceTransform;
    }

    public void SetRssSize(float size)
    {
        intialSize = size;
        Vector3 rssSize = new Vector3(size / resourceSizeScale, size / resourceSizeScale, size / resourceSizeScale);

        resourceTransform.localScale = rssSize;
    }

    public void SetRssRotation(int degrees)
    {
        Vector3 rssRotation = new Vector3(0, 0, degrees);

        transform.eulerAngles = rssRotation;
    }

    public ResourceManager.ResourceType ReturnResourceType()
    {
        return type;
    }

    public void AssignResourceType(ResourceManager.ResourceType newType)
    {
        type = newType;
    }

    public void RemoveUnit()
    {
        amount--;
        if(amount <= 0)
        {
            amount = 0;
            ResourceManager.main.RemoveResource(this);
            emptyTime = Time.time;
            StartCoroutine(DetermineRegen());
            SetRssSize(0);
        }

        float size = ((float)amount - 1) / (float)resourceScale;
        SetRssSize(size);
    }

    public int ReturnResourceAmount()
    {
        return amount;
    }

    #endregion

    #region Corutines

    private IEnumerator DetermineRegen()
    {
        yield return new WaitForSeconds(checkRegenTime);

        if (emptyTime >= reqEmptyTime)
        {
            float random = Random.value;

            if (random < regenChance)
            {
                Regenerate();
                ResourceManager.main.Regenerated(this);
            }
        }
        else
        {
            StartCoroutine(DetermineRegen());
        }
    }

    #endregion
}

// Add counter 