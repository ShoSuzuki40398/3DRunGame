using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// エリア情報
/// </summary>
public class Area : MonoBehaviour
{
    // 長さ
    //private float areaLength

    // 先頭位置
    private Vector3 leadPosition;

    // 末尾位置
    private Vector3 endPosition;

    // ライン情報
    private List<AreaLine> areaLines;

    private void Start()
    {
        // ライン取得
        areaLines = new List<AreaLine>(transform.GetComponentsInChildren<AreaLine>());

        // 先頭・末尾位置取得
        leadPosition = GetLeadPositionFromLines();
        endPosition = GetEndPositionFromLines();
    }

    /// <summary>
    /// ラインの中から一番先頭に近いマス目の位置を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetLeadPositionFromLines()
    {
        List<AreaGrid> leadGrids = new List<AreaGrid>();
        for(int i = 0; i < areaLines.Count; ++i)
        {
            leadGrids.Add(areaLines[i].leadGrid);
        }

        if (leadGrids.Count == 0)
        {
            return Vector3.zero;
        }

        var result = leadGrids.OrderBy(grid => grid.gridPosition.z).FirstOrDefault();
        
        return result.gridPosition;
    }

    /// <summary>
    /// ラインの中から一番末尾に近いマス目の位置を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetEndPositionFromLines()
    {
        List<AreaGrid> endGrids = new List<AreaGrid>();
        for (int i = 0; i < areaLines.Count; ++i)
        {
            endGrids.Add(areaLines[i].endGrid);
        }

        if (endGrids.Count == 0)
        {
            return Vector3.zero;
        }

        var result = endGrids.OrderByDescending(grid => grid.gridPosition.z).FirstOrDefault();

        return result.gridPosition;
    }
}
