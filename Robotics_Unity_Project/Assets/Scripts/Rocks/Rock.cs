// --------------------------------------------------------------
// MoonSim - Rock                                       4/05/2021
// Author(s): Cameron Carstens
// Contact: cameroncarstens@knights.ucf.edu
// --------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField]
    private Transform rockTransform;

    #endregion

    #region Monobehaviors 



    #endregion

    #region Public Methods

    public void SetRockSize(float size)
    {
        Vector3 localSize = new Vector3(size, size, size);

        transform.localScale = localSize;
    }

    public void SetRockRotation(int degrees)
    {
        Vector3 rot = new Vector3(0, 0, degrees);

        transform.eulerAngles = rot;
    }

    public Transform ReturnRockTransform()
    {
        return rockTransform;
    }

    #endregion
}
