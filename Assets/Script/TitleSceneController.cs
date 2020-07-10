using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FadeController.Instance.FadeIn(1.0f);
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
}
