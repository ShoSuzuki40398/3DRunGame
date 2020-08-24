using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleSceneController : MonoBehaviour
{
    // 音量調節UI
    [SerializeField]
    private GameObject volumeUI;

    // スキン変更UI
    [SerializeField]
    private GameObject skinUI;

    [SerializeField]
    private Canvas mainCanvas;

    [SerializeField]
    private GameObject messageUI;

    [SerializeField]
    private GameObject messageButton;

    [SerializeField]
    private GameData gameData;
    
    // Start is called before the first frame update
    void Start()
    {
        FadeController.Instance.FadeIn(1.0f);
        volumeUI.SetActive(false);
        skinUI.SetActive(false);
        messageUI.SetActive(false);
        CheckActiveMessage();
        DOTween.Clear(true);
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
    /// メッセージボタン表示確認
    /// </summary>
    private void CheckActiveMessage()
    {
        if(gameData.currentHighScore >= gameData.releaseMessageScoreBorder)
        {
            messageButton.SetActive(true);
        }
        else
        {
            messageButton.SetActive(false);
        }
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

    /// <summary>
    /// スキンコンフィグボタン
    /// </summary>
    public void OnClickSkinConfigButton()
    {
        skinUI.SetActive(true);
        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
    }

    /// <summary>
    /// スキンUI閉じる
    /// </summary>
    public void OnClickReturnSkinUIButton()
    {
        skinUI.SetActive(false);
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    /// <summary>
    /// メッセージUIボタン
    /// </summary>
    public void OnClickMessageUIButton()
    {
        messageUI.SetActive(true);
    }

    /// <summary>
    /// メッセージUI閉じる
    /// </summary>
    public void OnClickReturnMessageUIButton()
    {
        messageUI.SetActive(false);
    }
}
