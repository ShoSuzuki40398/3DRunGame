using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリア制御
/// </summary>
public class AreaController : MonoBehaviour
{
    // 最大表示エリア数
    [SerializeField, Range(3, 10)]
    private int maxAreaCount = 3;

    // エリアオブジェクト
    [SerializeField]
    private GameObject areaRootPrefab;

    // エリア管理リスト
    private List<Area> areaList;

    /// <summary>
    /// エリアオブジェクト作成
    /// </summary>
    /// <returns>Areaコンポーネント</returns>
    private Area CreateArea()
    {
        return Instantiate(areaRootPrefab).GetComponent<Area>();
    }

    /// <summary>
    /// エリアオブジェクト追加
    /// </summary>
    public void AddArea()
    {
        var area = CreateArea();
        areaList.Add(area);

        if(areaList.Count != 0)
        {
            var latestArea = areaList[areaList.Count - 1];

        }
    }

    /// <summary>
    /// エリアオブジェクト削除
    /// </summary>
    public void RemoveArea()
    {

    }
}
