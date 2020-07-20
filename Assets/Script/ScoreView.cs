using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;


    /// <summary>
    /// スコア表示リセット
    /// </summary>
    public void ResetScore()
    {
        UpdateScore(0);
    }

    /// <summary>
    /// スコア表示更新
    /// </summary>
    /// <param name="value"></param>
    public void UpdateScore(float value)
    {
        string format = "Score:{0:D6}";
        scoreText.text = string.Format(format, (int)value);
    }
}
