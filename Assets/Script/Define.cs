using System;

public static class Define
{
    /// <summary>
    /// シーン名定義
    /// </summary>
    public enum SCENE_NAME
    {
        TITLE,
        MAIN
    }

    /// <summary>
    /// シーン名取得
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetSceneName(SCENE_NAME name)
    {
        return Enum.GetName(typeof(SCENE_NAME), name);
    }

    //　ゲーム終了ボタンを押したら実行する
    public static void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		Application.OpenURL("http://www.yahoo.co.jp/");
#else
		Application.Quit();
#endif
    }

    // BGMパス
    public const string bgmPath = "Sound/bgm/";
    // SEパス
    public const string sePath = "Sound/se/";
    
    // BGM定義
    public enum BGM
    {
        BGM_1
    }

    // SE定義
    public enum SE
    {
        SE_1,
        SE_2
    }

    // マウスボタン定義
    public const int leftButton = 0;
    public const int rightButton = 1;

    // タグ
    public const string TagAreaEntrance = "AreaEntrance";
    public const string TagStartAreaExit = "StartAreaExit";
    public const string TagEnemy = "Enemy";

    // 敵パス
    public const string enemyPath = "Enemy/";

    // 敵の点数
    public const float pawnScore = 100;
    public const float rookScore = 200;
    public const float bishopScore = 400;
}
