using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultScoreView : MonoBehaviour
{
    // スコア表示の時間間隔
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
        pawnScoreText.text = " × " + ScorePool.Instance.GetDefeatedEnemy(Enemy.ENEMY_TYPE.PAWN).ToString();

        yield return new WaitForSeconds(animationInterval);
        rookScoreText.text = " × " + ScorePool.Instance.GetDefeatedEnemy(Enemy.ENEMY_TYPE.ROOK).ToString();

        yield return new WaitForSeconds(animationInterval);
        bishopScoreText.text = " × " + ScorePool.Instance.GetDefeatedEnemy(Enemy.ENEMY_TYPE.BISHOP).ToString();

        yield return new WaitForSeconds(animationInterval);
        string format = "{0:D}";
        resultScoreText.text = string.Format(format, (int)ScorePool.Instance.GetResultScore());

        // フッターボタンアクティブ
        yield return new WaitForSeconds(animationInterval);
        foreach (var button in footerButtons)
        {
            button.interactable = true;
        }
    }

    private void ResetView()
    {
        pawnScoreText.text = "×";
        rookScoreText.text = "×";
        bishopScoreText.text = "×";
        resultScoreText.text = "";
    }
}
