using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Titleシーンボタンイベント
/// </summary>
public class TitleButtonEvent : MonoBehaviour
{
    /// <summary>
    /// Startボタン
    /// </summary>
    public void OnClickStart()
    {
        SceneManager.LoadScene(Define.GetSceneName(Define.SceneName.Main));
    }

    /// <summary>
    /// Exitボタン
    /// </summary>
    public void OnClickExit()
    {
        Define.EndGame();
    }
}
