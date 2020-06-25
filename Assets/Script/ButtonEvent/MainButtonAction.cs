using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Mainシーンボタンイベント
/// </summary>
public class MainButtonAction : MonoBehaviour
{
    public void OnClickReturnTitle()
    {
        SceneManager.LoadScene(Define.GetSceneName(Define.SceneName.Title));
    }
}
