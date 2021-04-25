// --------------------------------------------------------------
// MoonSim - ResourceUI                            4/26/2021
// Author(s): Jonathan Frucht
// Contact:  jonfrucht@knights.ucf.edu
// --------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    #region Static Fields

    public static ResourceUI main;

    #endregion

    #region  Inspector fields
    //public Panel ResourcePannel;
    [Header("ResourceUI")]
    public GameObject bgPanel;
    public TMP_Text resourceUITitle;
    public TMP_Text ironTitle;
    public TMP_Text copperTitle;
    public TMP_Text goldTitle;
    public TMP_Text titaniumTitle;
    public TMP_Text ironText;
    public TMP_Text copperText;
    public TMP_Text goldText;
    public TMP_Text titaniumText;
    #endregion
    #region Run-Time Fields

    #endregion
    #region Monobehaviors
    public void Start()
    {
            
    }
    private void Awake()
    {
        //if (ResourceManager.main.isActiveAndEnabled)
        //{

            copperText.text = "0 / C";
            ironText.text = "0 / I";
            goldText.text = "0 / G";
            titaniumText.text = "0 / T ";
            StartCoroutine(UpdateResourceUI());
        //bgPanel.enabled
        //Canvas myCanvas = this.GetComponent<Canvas>();
        //myCanvas.renderMode = RenderMode.WorldSpace;
        //myCanvas.worldCamera = Camera.main;
        //}
    }
    #endregion
    #region Coroutines
    private IEnumerator UpdateResourceUI()
    {
        yield return new WaitForSeconds(1);
        copperText.text = ResourceManager.main.copperStoredAmount.ToString() + " / " + ResourceManager.main.copperStorageCapacity.ToString();
        ironText.text = ResourceManager.main.ironStoredAmount.ToString() + " / " + ResourceManager.main.ironStorageCapacity.ToString();
        goldText.text = ResourceManager.main.goldStoredAmount.ToString() + " / " + ResourceManager.main.goldStorageCapacity.ToString();
        titaniumText.text = ResourceManager.main.titaniumStoredAmount.ToString() + " / " + ResourceManager.main.titaniumStorageCapacity.ToString();
        StartCoroutine(UpdateResourceUI());

    }
    #endregion

}