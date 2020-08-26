using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultScoreView : MonoBehaviour
{
    // スコア表示の時間間隔
    [SerializeField]
    private float animationInterval = 1.0f;

    [SerializeField]
    private Text pawnScoreText;

    [SerializeField]
    private Text rookScoreText;

    [SerializeField]
    private Text bishopScoreText;

    [SerializeField]
    private Text resultScoreText;

    [SerializeField]
    private List<Button> footerButtons;

    [SerializeField]
    private GameData gameData;

    /// <summary>
    /// スコア表示開始
    /// </summary>
    public void DisplayScore()
    {
        ResetView();

        StartCoroutine(AnimationScore());
    }

    /// <summary>
    /// スコア表示演出
    /// </summary>
    /// <returns></returns>
    private IEnumerator AnimationScore()
    {
        // フッターボタン非アクティブ
        foreach (var button in footerButtons)
        {
            button.interactable = false;
        }

        // 順番にスコア表示
        yield return new WaitForSeconds(animationInterval);
        StartCoroutine(ScoreAnimation(0, ScorePool.Instance.GetDefeatedEnemy(Enemy.ENEMY_TYPE.PAWN), animationInterval, pawnScoreText));

        yield return new WaitForSeconds(animationInterval);
        StartCoroutine(ScoreAnimation(0, ScorePool.Instance.GetDefeatedEnemy(Enemy.ENEMY_TYPE.ROOK), animationInterval, rookScoreText));

        yield return new WaitForSeconds(animationInterval);
        StartCoroutine(ScoreAnimation(0, ScorePool.Instance.GetDefeatedEnemy(Enemy.ENEMY_TYPE.BISHOP), animationInterval, bishopScoreText));

        yield return new WaitForSeconds(animationInterval);
        string format = "{0:D}";
        int resultScore = (int)ScorePool.Instance.GetResultScore();

        // ハイスコアの更新
        if (resultScore > gameData.currentHighScore)
        {
            gameData.currentHighScore = resultScore;
        }
        resultScoreText.text = string.Format(format, resultScore);

        // フッターボタンアクティブ
        yield return new WaitForSeconds(animationInterval);
        foreach (var button in footerButtons)
        {
            button.interactable = true;
        }
    }

    private void ResetView()
    {
        pawnScoreText.text = "× ";
        rookScoreText.text = "× ";
        bishopScoreText.text = "× ";
        resultScoreText.text = "";
    }

    /// <summary>
    /// スコアをアニメーションさせる
    /// </summary>
    /// <param name="startScore"></param>
    /// <param name="endScore"></param>
    /// <param name="duration"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator ScoreAnimation(float startScore, float endScore, float duration, Text text)
    {
        // 開始時間
        float startTime = Time.time;

        // 終了時間
        float endTime = startTime + duration;

        do
        {
            // 現在の時間の割合
            float timeRate = (Time.time - startTime) / duration;

            // 数値を更新
            float updateValue = (float)((endScore - startScore) * timeRate + startScore);

            // テキストの更新
            text.text = "× " + updateValue.ToString("f0"); // （"f0" の "0" は、小数点以下の桁数指定）

            // 1フレーム待つ
            yield return null;

        } while (Time.time < endTime);

        // 最終的な着地のスコア
        text.text = "× " + endScore.ToString();
        AudioManager.Instance.PlaySE(Define.SE.SCORE_DISPLAY);
    }
}
