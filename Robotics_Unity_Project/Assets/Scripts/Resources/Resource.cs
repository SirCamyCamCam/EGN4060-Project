////////////


// --------------------------------------------------------------
// MoonSim - Resource                            4/26/2021
// Author(s): Jonathan Frucht
// Contact:  jonfrucht@knights.ucf.edu
// --------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField]
    private Transform resourceTransform;

    #endregion

    #region Monobehaviors 



    #endregion

    #region Public Methods
    public Resource ReturnNearestResource(ResourceManager.ResourceType type, Vector2 position) {
        return null;// for this type loop through every resource and find cloest to the position given
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

    #endregion
}
