using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinChange : MonoBehaviour
{
    // スキン種類定義
    public enum SKIN_TYPE
    {
        NORMAL,
        RIM,
        NOISE,
        CLEAR,
    }

    // スキン変更対象
    [SerializeField]
    private GameObject skinTarget;

    // スキン変更ボタンリスト
    [SerializeField]
    private List<Button> skinChangeButtons;

    // スキン解放条件スコア
    [SerializeField]
    private List<Text> skinReleaseBorderText;

    [SerializeField]
    private GameData gameData;

    private void OnEnable()
    {
        CheckReleaseSkin();
    }

    /// <summary>
    /// スキン変更
    /// </summary>
    public void ClickOnSkinChangeButton(int type)
    {
        SKIN_TYPE enumCast = (SKIN_TYPE)type;

        switch (enumCast)
        {
            case SKIN_TYPE.NORMAL: SetSkinName(Define.shaderPathStandard); break;
            case SKIN_TYPE.RIM: SetSkinName(Define.shaderPathRimLighting); break;
            case SKIN_TYPE.NOISE: SetSkinName(Define.shaderPathBlockNoise); break;
            case SKIN_TYPE.CLEAR: SetSkinName(Define.shaderPathClearIce); break;
            default:break;
        }
    }

    /// <summary>
    /// スキン名を設定
    /// </summary>
    /// <param name="path"></param>
    private void SetSkinName(string path)
    {
        Renderer renderer = skinTarget.GetComponent<Renderer>();

        renderer.material.shader = Shader.Find(path);
        gameData.playerSkinName = path;
    }

    /// <summary>
    /// 解放スキン確認
    /// </summary>
    private void CheckReleaseSkin()
    {
        for(int i = 0;i < skinChangeButtons.Count;++i)
        {
            skinChangeButtons[i].interactable = false;
        }

        List<Button> buttons = new List<Button>();
        int checkCount = skinChangeButtons.Count >= gameData.releaseSkinScoreBorder.Count ? gameData.releaseSkinScoreBorder.Count : skinChangeButtons.Count;
        for(int i = 0;i < checkCount;++i)
        {
            buttons.Add(skinChangeButtons[i]);
        }

        List<Text> texts = new List<Text>();
        checkCount = skinReleaseBorderText.Count >= gameData.releaseSkinScoreBorder.Count ? gameData.releaseSkinScoreBorder.Count : skinReleaseBorderText.Count;
        for(int i = 0;i < checkCount;++i)
        {
            texts.Add(skinReleaseBorderText[i]);
        }

        for(int i = 0;i < texts.Count;++i)
        {
            texts[i].text = gameData.releaseSkinScoreBorder[i].ToString() + "点";
        }

        for (int i = 0;i < buttons.Count;++i)
        {
            if(gameData.currentHighScore >= gameData.releaseSkinScoreBorder[i])
            {
                buttons[i].interactable = true;
            }
        }
    }
}
