using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリア制御
/// </summary>
public class AreaController : MonoBehaviour
{
    // 最大表示エリア数
    [SerializeField, Range(1, 10)]
    private int maxAreaCount = 1;

    // エリアオブジェクト
    [SerializeField]
    private List<GameObject> areaPrefabs;

    // エリア管理リスト
    private List<Area> areaList = new List<Area>();

    // スタートエリア
    [SerializeField]
    private Area startArea;

    // プレイヤー生成位置
    [SerializeField]
    private Transform playerSpawnPoint;

    /// <summary>
    /// プレイヤー作成
    /// </summary>
    public Player CreatePlayer(GameObject prefab)
    {
        var obj = Instantiate(prefab, playerSpawnPoint.position,Quaternion.identity);

        // スタート地点の上に立たせる
        // レイでスタート地点の面の位置を取得
        // オブジェクトの高さで位置調整
        var ray = new Ray(playerSpawnPoint.position, -Vector3.up);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, 10.0f))
        {
            var halfObjHeight = obj.GetComponent<Renderer>().bounds.size.y * 0.5f;
            obj.transform.position = new Vector3(hit.point.x, hit.point.y + halfObjHeight, hit.point.z);
        }
        return obj.ForceGetComponent<Player>();
    }

    /// <summary>
    /// エリア初期化
    /// スタートエリアに設定した数のエリアを作成して連結
    /// </summary>
    public void Initialize()
    {
        // 全てのエリアを削除
        AllRemoveArea();

        // スタートエリア初期化
        startArea.Initialize();

        // エリアを作成
        for(int i = 0;i < maxAreaCount; ++i)
        {
            AddArea();
        }
    }

    /// <summary>
    /// エリアオブジェクト作成
    /// </summary>
    /// <returns>Areaコンポーネント</returns>
    private Area CreateArea()
    {
        int index = Random.Range(0, areaPrefabs.Count);
        var area = Instantiate(areaPrefabs[index]).GetComponent<Area>();
        return area;
    }

    int count = 1;
    /// <summary>
    /// エリアオブジェクト追加
    /// </summary>
    public void AddArea()
    {
        // エリア作成前に最後尾のエリアをあらかじめ取得
        // ※エリアの新規作成とListへの追加を同時に行っているため
        //   Listを検索すると新規作成したエリアを最後尾として
        //   取得してしまうので、新規作成前に最後尾を取得しておく。
        var latestArea = GetLatestArea();

        // 作成
        var area = CreateArea();
        area.name = count.ToString();
        count++;
        area.Initialize();
        areaList.Add(area);
        area.transform.parent = transform;

        // 連結
        ConnectArea(latestArea, area);

        // マス目座標を再計算
        // ※エリアの座標が変更される度にマス目の座標を再計算する必要があります。
        area.CalcGridPosition();
    }

    /// <summary>
    /// エリアオブジェクト削除
    /// </summary>
    public void RemoveArea(Area area)
    {
        Destroy(area.gameObject);
        areaList.Remove(area);
    }

    /// <summary>
    /// // 全てのエリア削除
    /// </summary>
    public void AllRemoveArea()
    {
        for (int i = 0; i < areaList.Count; ++i)
        {
            Destroy(areaList[i].gameObject);
        }
        areaList.Clear();
    }

    /// <summary>
    /// 最後尾のエリアを取得
    /// </summary>
    /// <returns></returns>
    private Area GetLatestArea()
    {
        return areaList.Count == 0 ? startArea : areaList[areaList.Count - 1];
    }
    
    /// <summary>
    /// エリアを繫げる
    /// firstに指定したエリアの後ろにsecondに指定したエリアを繫げる
    /// </summary>
    /// <param name="first">末尾を繫げるエリア</param>
    /// <param name="second">先端を繫げるエリア</param>
    private void ConnectArea(Area first, Area second)
    {
        second.transform.position = first.marginEndPosition;
    }
}
