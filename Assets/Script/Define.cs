using System;

public static class Define
{
    // ScriptableObject
    public const string scriptablePath = "MyScriptable/";

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
		UnityEngine.Application.OpenURL("https://unityroom.com/");
#else
		UnityEngine.Application.Quit();
#endif
    }

    // BGMパス
    public const string bgmPath = "Sound/bgm/";
    // SEパス
    public const string sePath = "Sound/se/";
    
    // BGM定義
    public enum BGM
    {
        TITLE,
        MAIN
    }

    // SE定義
    public enum SE
    {
        PLAYER_SHIFT,
        PLAYER_ATTACK_HIT,
        PLAYER_BURST,
        ENEMY_BURST,
        BUTTON_CLICK,
        BUTTON_HIGHLIGHT,
        SCORE_DISPLAY
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
    public const float pawnScore = 10;
    public const float rookScore = 20;
    public const float bishopScore = 30;

    // スキン用シェーダー名
    public const string shaderPathStandard = "Standard";
    public const string shaderPathClearIce = "Custom/ClearIce";
    public const string shaderPathBlockNoise = "Custom/BlockNoise";
    public const string shaderPathRimLighting = "Custom/RimLighting";
    public const string shaderPathCircleAnimation = "Custom/CircleAnimation";
    public const string shaderPathToon = "Custom/Toon";
}
