using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// エリア情報
/// </summary>
public class Area : MonoBehaviour
{
    // エリア入り口
    [SerializeField]
    private GameObject areaEntrance;

    // 先頭位置
    [HideInInspector]
    public Vector3 leadPosition;
    // 先頭位置からマス目一つ分手前にずれた座標
    [HideInInspector]
    public Vector3 marginLeadPosition;

    // 末尾位置
    [HideInInspector]
    public Vector3 endPosition;
    // 末尾位置からマス目一つ分奥にずれた座標
    [HideInInspector]
    public Vector3 marginEndPosition;

    // ライン情報
    private List<AreaLine> areaLines;

    /// <summary>
    /// エリアの中で中央にあるラインを取得
    /// </summary>
    /// <returns></returns>
    private AreaLine GetCenterLine()
    {
        // X座標で整列して
        var sortList = areaLines.OrderBy(line => line.transform.position.x);

        int size = sortList.Count();
        int mid = size / 2;
        AreaLine center = sortList.ElementAt(mid);
        return center;
    }

    /// <summary>
    /// 指定のラインの中で一番先頭のマス目の座標を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetLeadGridPosition(AreaLine line)
    {
        return line.leadGrid.gridPosition;
    }

    /// <summary>
    /// 指定のラインの中で一番末尾のマス目の座標を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetEndGridPosition(AreaLine line)
    {
        return line.endGrid.gridPosition;
    }

    /// <summary>
    /// 指定のラインの中で先頭のマス目の手前の座標を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMarginLeadGridPosition(AreaLine line)
    {
        var gridPosition = line.leadGrid.gridPosition;
        var result = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z - line.leadGrid.gridSize.z);
        return result;
    }

    /// <summary>
    /// 指定のラインの中で末尾のマス目の奥の座標を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMarginEndGridPosition(AreaLine line)
    {
        var gridPosition = line.endGrid.gridPosition;
        var result = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z + line.endGrid.gridSize.z);
        return result;
    }


    /// <summary>
    /// 全てラインの中から一番先頭に近いマス目の位置を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetLeadPositionFromLines()
    {
        List<AreaGrid> leadGrids = new List<AreaGrid>();
        for (int i = 0; i < areaLines.Count; ++i)
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
    /// 全てラインの中から一番末尾に近いマス目の位置を取得
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

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        // ライン取得
        areaLines = new List<AreaLine>(transform.GetComponentsInChildren<AreaLine>());

        // ライン初期化
        for(int i = 0; i < areaLines.Count; ++i)
        {
            areaLines[i].Initialize(transform);
        }

        // マス目座標計算
        CalcGridPosition();
    }

    /// <summary>
    /// マス目座標を計算
    /// </summary>
    public void CalcGridPosition()
    {
        // 中央のラインで計算
        var centerLine = GetCenterLine();

        // 先頭座標
        leadPosition = GetLeadGridPosition(centerLine);
        marginLeadPosition = GetMarginLeadGridPosition(centerLine);

        // 末尾座標
        endPosition = GetEndGridPosition(centerLine);
        marginEndPosition = GetMarginEndGridPosition(centerLine);
    }

    /// <summary>
    /// エリア入り口有効化
    /// </summary>
    public void EnableEntrance()
    {
        if(areaEntrance != null)
        {
            areaEntrance.SetActive(true);
        }
    }

    /// <summary>
    /// エリア入り口無効化
    /// </summary>
    public void DisableEntrance()
    {
        if (areaEntrance != null)
        {
            areaEntrance.SetActive(false);
        }
    }
}
