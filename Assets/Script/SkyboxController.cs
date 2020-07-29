using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField]
    private Material skyboxMaterial;

    [SerializeField]
    private Color startSkyColor;

    private Color currentSkyColor;

    private Color nextSkyColor;

    readonly string skyColorName = "_SkyTint";
    readonly string groundColorName = "_GroundColor";

    private float changeDuration = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentSkyColor = startSkyColor;
        nextSkyColor = startSkyColor;
        skyboxMaterial.SetColor(skyColorName, startSkyColor);
        RenderSettings.skybox = skyboxMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        SkyboxUpdate();
    }

    void SkyboxUpdate()
    {
        changeDuration = Mathf.Clamp01(changeDuration);
        skyboxMaterial.SetColor(skyColorName, Color.Lerp(currentSkyColor, nextSkyColor, changeDuration));
        Color currentGroundColor = new Color(currentSkyColor.r, currentSkyColor.g, currentSkyColor.b);
        Color nextGroundColor = new Color(nextSkyColor.r, nextSkyColor.g, nextSkyColor.b);
        currentGroundColor *= 0.9f;
        nextGroundColor *= 0.9f;
        skyboxMaterial.SetColor(groundColorName, Color.Lerp(currentGroundColor, nextGroundColor, changeDuration));
    }

    /// <summary>
    /// スカイボックスに色変更
    /// </summary>
    public void ChangeSkyColor(Color color)
    {
        nextSkyColor = color;
        StartCoroutine(ChangeSkyColorDuration());
    }

    /// <summary>
    /// スカイボックスの色を徐々に変更
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeSkyColorDuration()
    {
        while(changeDuration < 1.0f)
        {
            changeDuration += Time.deltaTime;
            yield return null;
        }
        currentSkyColor = nextSkyColor;
        changeDuration = 0;
    }
}
