// --------------------------------------------------------------
// MoonSim - Fade Transition                            4/20/2021
// Author(s): Cameron Carstens
// Contact: cameroncarstens@knights.ucf.edu
// --------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeTransition : MonoBehaviour {

    #region Inspector Fields

    [Header("Dependencies")]
    [SerializeField]
    private Text loadingText;
    [SerializeField]
    private Image backgroundImage;
    [Header("Settings")]
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private float waitToFadeInTime;

    #endregion

    #region Run-Time Fields

    private Color32 clear;
    private Color32 filled;

    #endregion

    #region Monohehaviors

    // Use this for initialization
    void Start () {
        clear = new Color32(0, 0, 0, 0);
        filled = new Color32(0, 0, 0, 255);

        StartCoroutine(FadeOutRoutine());
	}

    #endregion

    #region Public Methods

    // Call to fade in
    public void FadeIn(string newScene)
    {
        StartCoroutine(FadeInRoutine(newScene));
    }

    #endregion

    #region Coroutines

    private IEnumerator FadeInRoutine(string newScene)
    {
        float startTime = Time.time;
        backgroundImage.enabled = true;
        while (Time.time - startTime <= fadeTime)
        {
            backgroundImage.color = Color.Lerp(clear, filled, (Time.time - startTime) / fadeTime);
            yield return 0;
        }

        backgroundImage.color = filled;
        SceneManager.LoadScene(newScene);
    }

    private IEnumerator FadeOutRoutine()
    {
        yield return new WaitForSeconds(waitToFadeInTime);
        float startTime = Time.time;
        Color originalTextColor = loadingText.color;
        Color originalPanelColor = backgroundImage.color;
        while (Time.time - startTime <= fadeTime)
        {
            loadingText.color = Color.Lerp(originalTextColor, Color.clear, (Time.time - startTime) / fadeTime);
            backgroundImage.color = Color.Lerp(filled, clear, (Time.time - startTime) / fadeTime);
            yield return 0;
        }

        loadingText.color = Color.clear;
        backgroundImage.color = clear;
        loadingText.enabled = false;
        backgroundImage.enabled = false;
    }

    #endregion
}
