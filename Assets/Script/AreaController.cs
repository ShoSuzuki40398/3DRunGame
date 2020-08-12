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

    // エリアの最大幅
    [SerializeField]
    private int maxAreaWidth = 3;

    // エリアオブジェクト
    [SerializeField]
    private List<GameObject> easyAreaPrefabs = new List<GameObject>();
    [SerializeField]
    private List<GameObject> normalAreaPrefabs = new List<GameObject>();
    [SerializeField]
    private List<GameObject> hardAreaPrefabs = new List<GameObject>();

    // エリア管理リスト
    private List<Area> areaList = new List<Area>();

    // スタートエリア
    [SerializeField]
    private Area startArea;

    // プレイヤー生成位置
    [SerializeField]
    private Transform playerSpawnPoint;

    // プレイヤー制御
    private Player player;

    // プレイヤーが生成されるエリアラインのインデックス
    // 左端のラインを１として、右に行くたびに1加算した値にする
    [SerializeField]
    private int playerSpawnLineIdx = 2;

    // エリアの難易度が変わるエリア数
    // プレイヤーが指定数走破した後でエリアの難易度を変える
    [SerializeField]
    private const int levelChangeInterval = 2;

    // プレイヤーが走破したエリア数
    private int currentLevelChangeAreaCount = 0;

    // エリア難易度
    enum AREA_LEVEL
    {
        EASY,
        NORAML,
        HARD
    }
    private AREA_LEVEL currentAreaLevel = AREA_LEVEL.EASY;

    // スカイボックス制御
    [SerializeField]
    private SkyboxController skyboxController;

    [SerializeField]
    private Color easySkyColor;

    [SerializeField]
    private Color normalSkyColor;

    [SerializeField]
    private Color hardSkyColor;

    /// <summary>
    /// エリアオブジェクト作成
    /// </summary>
    /// <returns>Areaコンポーネント</returns>
    private Area CreateArea(AREA_LEVEL level)
    {
        int index = 0;
        Area area;
        switch (level)
        {
            case AREA_LEVEL.EASY:
                index = Random.Range(0, easyAreaPrefabs.Count);
                area = Instantiate(easyAreaPrefabs[index]).GetComponent<Area>();
                break;
            case AREA_LEVEL.NORAML:
                index = Random.Range(0, normalAreaPrefabs.Count);
                area = Instantiate(normalAreaPrefabs[index]).GetComponent<Area>();
                break;
            case AREA_LEVEL.HARD:
                index = Random.Range(0, hardAreaPrefabs.Count);
                area = Instantiate(hardAreaPrefabs[index]).GetComponent<Area>();
                break;
            default:
                index = Random.Range(0, easyAreaPrefabs.Count);
                area = Instantiate(easyAreaPrefabs[index]).GetComponent<Area>();
                break;
        }

        return area;
    }

    /// <summary>
    /// 最後尾のエリアを取得
    /// ない場合はスタートエリアを取得
    /// </summary>
    /// <returns></returns>
    private Area GetLatestArea()
    {
        return areaList.Count == 0 ? startArea : areaList[areaList.Count - 1];
    }

    /// <summary>
    /// 先頭のエリアを取得
    /// ない場合はスタートエリアを取得
    /// </summary>
    /// <returns></returns>
    private Area GetLeadArea()
    {
        return areaList.Count == 0 ? startArea : areaList[0];
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

    /// <summary>
    /// エリアの難易度を上げる
    /// </summary>
    private void AreaLevelUp()
    {
        switch (currentAreaLevel)
        {
            case AREA_LEVEL.EASY:
                currentAreaLevel = AREA_LEVEL.NORAML;
                skyboxController.ChangeSkyColor(normalSkyColor);
                player.ChangeEffectColor(normalSkyColor);
                break;
            case AREA_LEVEL.NORAML:
                currentAreaLevel = AREA_LEVEL.HARD;
                skyboxController.ChangeSkyColor(hardSkyColor);
                player.ChangeEffectColor(hardSkyColor);
                break;
            case AREA_LEVEL.HARD:
                currentAreaLevel = AREA_LEVEL.HARD;
                skyboxController.ChangeSkyColor(hardSkyColor);
                player.ChangeEffectColor(easySkyColor);
                break;
            default:
                currentAreaLevel = AREA_LEVEL.EASY;
                skyboxController.ChangeSkyColor(easySkyColor);
                player.ChangeEffectColor(easySkyColor);
                break;
        }
    }

    /// <summary>
    /// 走破エリア数カウントアップ
    /// 難易度上昇があった時はtrueを返す
    /// </summary>
    public bool CountUpRunningArea()
    {
        currentLevelChangeAreaCount += 1;
        // 指定エリア数走破したとき難易度上昇
        if(currentLevelChangeAreaCount > levelChangeInterval)
        {
            currentLevelChangeAreaCount = 0;
            AreaLevelUp();
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// プレイヤー作成
    /// </summary>
    public Player CreatePlayer(GameObject prefab)
    {
        var obj = Instantiate(prefab, playerSpawnPoint.position, Quaternion.identity);

        // スタート地点の上に立たせる
        // レイでスタート地点の面の位置を取得
        // オブジェクトの高さで位置調整
        var ray = new Ray(playerSpawnPoint.position, -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10.0f))
        {
            // 軌跡が地面に埋まって見づらかったのでちょっとだけ浮かせる
            var objPositionOffset = obj.GetComponent<Renderer>().bounds.size.y * 0.05f;
            obj.transform.position = new Vector3(hit.point.x, hit.point.y + objPositionOffset, hit.point.z);
        }

        // 自身をプレイヤーに設定
        var player = obj.ForceGetComponent<Player>();
        player.SetAreaController(this);
        player.SetCurrentAreaLineIndex(playerSpawnLineIdx);
        player.ChangeEffectColor(easySkyColor);
        this.player = player;
        return player;
    }

    /// <summary>
    /// エリア初期化
    /// スタートエリアに設定した数のエリアを作成して連結
    /// </summary>
    public void Initialize()
    {
        currentLevelChangeAreaCount = 0;
        currentAreaLevel = AREA_LEVEL.EASY;
        skyboxController.ChangeSkyColor(easySkyColor);

        // スタートエリア初期化
        startArea.Initialize();

        // エリアを作成
        for (int i = 0; i < maxAreaCount; ++i)
        {
            AddArea();
        }

        // エリア一つ目の入り口判定は無効化する
        // ※プレイヤーが侵入したときに2つ目以降の
        //   エリアから判定させたいため。
        areaList[0].DisableEntrance();
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    public void Finalized()
    {
        // 全てのエリアを削除
        AllRemoveArea();
    }

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
        var area = CreateArea(currentAreaLevel);
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
    /// 全てのエリア削除
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
    /// 先頭のエリアを削除
    /// </summary>
    public void RemoveLeadArea()
    {
        RemoveArea(GetLeadArea());
    }

    /// <summary>
    /// エリア最大幅取得
    /// </summary>
    /// <returns></returns>
    public int GetMaxAreaWidth()
    {
        return maxAreaWidth;
    }
}
