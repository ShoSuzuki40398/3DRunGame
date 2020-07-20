using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    private float point;

    // 最大値、最大値
    private float max;
    private float min;

    public Score(float min = 0,float max = 9999)
    {
        this.min = min;
        this.max = max;
    }

    /// <summary>
    /// 加点
    /// </summary>
    /// <param name="value"></param>
    public float Add(float value)
    {
        point += value;
        Clamp(point);
        return point;
    }

    /// <summary>
    /// 減点
    /// </summary>
    public float Sub(float value)
    {
        point -= value;
        Clamp(point);
        return point;
    }

    /// <summary>
    /// 点数取得
    /// </summary>
    /// <returns></returns>
    public float GetScore()
    {
        return point;
    }

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset()
    {
        point = min;
    }

    /// <summary>
    /// 範囲内制限
    /// </summary>
    private float Clamp(float value)
    {
        var result = Mathf.Clamp(value, min, max);
        point = result;
        return result;
    }
}
