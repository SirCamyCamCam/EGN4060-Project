// --------------------------------------------------------------
// MoonSim - ResourceManager                            4/26/2021
// Author(s): 
// Contact: 
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
        NONE
    }

    #endregion

    #region Static Fields

    // Public
    public static ResourceManager main;

    // Private

    #endregion

    #region Inspector Fields

    // Stuff

    #endregion

    #region Run-Time Fields

    // Stuff

    #endregion

    #region Monobehaviors

    private void Awake()
    {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {

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

    #endregion

    #region Public Methods


    #endregion

    #region Coroutines



    #endregion
}
