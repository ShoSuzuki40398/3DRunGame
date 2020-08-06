using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeAdjust : MonoBehaviour
{
    // 一度に変動する量
    [SerializeField,Range(0.1f,1.0f)]
    private float volmeChangeValue = 0.1f;

    [SerializeField]
    private Text masterVolumeText;

    [SerializeField]
    private Text bgmVolumeText;

    [SerializeField]
    private Text seVolumeText;

    private void Start()
    {
        masterVolumeText.text = GetDisplayVolumeText(AudioManager.Instance.Volume);
        bgmVolumeText.text = GetDisplayVolumeText(AudioManager.Instance.BgmVolume);
        seVolumeText.text = GetDisplayVolumeText(AudioManager.Instance.SeVolume);
    }

    /// <summary>
    /// 指定のボリュームを表示用の文字列に置き換える
    /// 1を100%,0を0%として表す
    /// </summary>
    /// <returns></returns>
    private string GetDisplayVolumeText(float value)
    {
        float per = 100;
        string volStr = Mathf.Round((value * per)).ToString();
        return volStr + " %";
    }

    /// <summary>
    /// マスター音量を上げる
    /// </summary>
    public void MasterVolumeUp()
    {
        AudioManager.Instance.Volume += volmeChangeValue;
        masterVolumeText.text = GetDisplayVolumeText(AudioManager.Instance.Volume);
    }

    /// <summary>
    /// マスター音量を下げる
    /// </summary>
    public void MasterVolumeDown()
    {
        AudioManager.Instance.Volume -= volmeChangeValue;
        masterVolumeText.text = GetDisplayVolumeText(AudioManager.Instance.Volume);
    }

    /// <summary>
    /// BGM音量を上げる
    /// </summary>
    public void BgmVolumeUp()
    {
        AudioManager.Instance.BgmVolume += volmeChangeValue;
        bgmVolumeText.text = GetDisplayVolumeText(AudioManager.Instance.BgmVolume);
    }

    /// <summary>
    /// BGM音量を下げる
    /// </summary>
    public void BgmVolumeDown()
    {
        AudioManager.Instance.BgmVolume -= volmeChangeValue;
        bgmVolumeText.text = GetDisplayVolumeText(AudioManager.Instance.BgmVolume);
    }

    /// <summary>
    /// SE音量を上げる
    /// </summary>
    public void SeVolumeUp()
    {
        AudioManager.Instance.SeVolume += volmeChangeValue;
        seVolumeText.text = GetDisplayVolumeText(AudioManager.Instance.SeVolume);
    }

    /// <summary>
    /// SE音量を下げる
    /// </summary>
    public void SeVolumeDown()
    {
        AudioManager.Instance.SeVolume -= volmeChangeValue;
        seVolumeText.text = GetDisplayVolumeText(AudioManager.Instance.SeVolume);
    }
}
