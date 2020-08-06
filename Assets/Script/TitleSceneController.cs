using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField]
    GameObject volumeUI;

    // Start is called before the first frame update
    void Start()
    {
        FadeController.Instance.FadeIn(1.0f);
        volumeUI.SetActive(false);
        AudioManager.Instance.PlayBGM(Define.BGM.TITLE);
    }

    /// <summary>
    /// Startボタン
    /// </summary>
    public void OnClickStart()
    {
        FadeController.Instance.FadeOut(0.5f,()=> SceneManager.LoadScene(Define.GetSceneName(Define.SCENE_NAME.MAIN)));
    }

    /// <summary>
    /// Exitボタン
    /// </summary>
    public void OnClickExit()
    {
        Define.EndGame();
    }

    /// <summary>
    /// 音量調節ボタン
    /// </summary>
    public void OnClickVolumeButton()
    {
        volumeUI.SetActive(true);
    }

    /// <summary>
    /// 音量調節閉じる
    /// </summary>
    public void OnClickReturnVolumeUIButton()
    {
        volumeUI.SetActive(false);
    }
}
