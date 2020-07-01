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
    private List<GameObject> areaPrefabs;

    // エリア管理リスト
    private List<Area> areaList;

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
    /// エリアオブジェクト作成
    /// </summary>
    /// <returns>Areaコンポーネント</returns>
    private Area CreateArea()
    {
        int index = Random.Range(0, areaPrefabs.Count);
        return Instantiate(areaPrefabs[index]).GetComponent<Area>();
    }

    /// <summary>
    /// エリアオブジェクト追加
    /// </summary>
    public void AddArea()
    {
        // 作成
        var area = CreateArea();
        areaList.Add(area);
        area.transform.parent = transform;
        
        // 連結
        ConnectArea(area);
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
    /// エリアを最後尾に繫げる
    /// </summary>
    /// <param name="area"></param>
    private void ConnectArea(Area area)
    {
        var latestArea = areaList.Count == 0 ? startArea : areaList[areaList.Count - 1];

        area.transform.position = latestArea.endPosition;
    }
}
