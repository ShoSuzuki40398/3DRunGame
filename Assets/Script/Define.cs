using System;

public static class Define
{
    /// <summary>
    /// シーン名定義
    /// </summary>
    public enum SceneName
    {
        Title,
        Main
    }

    /// <summary>
    /// シーン名取得
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetSceneName(SceneName name)
    {
        return Enum.GetName(typeof(SceneName), name);
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

    public const string bgmPath = "Sound/bgm/";
    public const string sePath = "Sound/se/";

    public enum BGM
    {
        BGM_1
    }

    public enum SE
    {
        SE_1,
        SE_2
    }
}
