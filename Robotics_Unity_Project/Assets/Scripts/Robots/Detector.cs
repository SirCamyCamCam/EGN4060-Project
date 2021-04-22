// --------------------------------------------------------------
// MoonSim - Detector                                   4/20/2021
// Author(s): Cameron Carstens
// Contact: cameroncarstens@knights.ucf.edu
// --------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    #region Run-Time Fields



    #endregion

    #region Inspector Fields

    [SerializeField]
    private Robot robot;

    #endregion

    #region Monobehaviors

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "robot")
        {
            robot.AddtoNearbyRobots(collision.transform);
        }

        if (collision.tag == "rock")
        {
            robot.AddToNearbyRocks(collision.transform);
        }

        if (collision.tag == "resource")
        {
            robot.FoundResource(collision.transform);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "robot")
        {
            robot.RemoveNearbyRobots(collision.transform);
        }

        if (collision.tag == "rock")
        {
            robot.RemoveNearbyRocks(collision.transform);
        }

        if (collision.tag == "resource")
        {

        }
    }

    #endregion
}
