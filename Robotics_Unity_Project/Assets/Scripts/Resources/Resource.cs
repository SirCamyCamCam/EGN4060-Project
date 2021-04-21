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

    #endregion

    #region Run-Time Fields

    private ResourceManager.ResourceType type;
    private int amount;
    private int emptyTime;
    private int reqEmptyTime;
    private float regenChance;

    

    #endregion

    #region Monobehaviors 



    #endregion

    #region Public Methods
    public void SetAmount()
    {
        amount = (int)(Random.value * 10.0);
    }
    public void SetReqEmptyTime(int reqEmptyTime)
    {
        this.reqEmptyTime = reqEmptyTime;
    }
    public void setRegenChance(float regenChance)
    {
        this.regenChance = regenChance;
    }
    public Transform ReturnResourceTransform() 
    {
        return resourceTransform;
    }
    public void SetRssSize(float size)
    {
        Vector3 rssSize = new Vector3(size, size, size);

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
            ResourceManager.main.RemoveResource(this);
        }
        // Remove 1 of however many iron/gold/whatever is at the resource
    }

    internal bool DoIRegen()
    {
        if (emptyTime < reqEmptyTime)
        {   
            return false;
        }
        else if (regenChance >= Random.value)
        {
            return true;
        }
        else 
        {
            regenChance = regenChance + (float)01;
            return false;
        }
    }

    public void UpdateEmptyTime()
    {
        emptyTime++;
    }

    #endregion
}

// Add counter 