using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePool : SingletonMonoBehaviour<ScorePool>
{
    public Score score = new Score();

    // 倒した敵の数
    private Dictionary<Enemy.ENEMY_TYPE, int> defeatedEnemy = new Dictionary<Enemy.ENEMY_TYPE, int>();
    
    /// <summary>
    /// 倒した敵の数を追加
    /// </summary>
    /// <param name="type"></param>
    public void AddDefeatedEnemy(Enemy.ENEMY_TYPE type)
    {
        if(!defeatedEnemy.ContainsKey(type))
        {
            defeatedEnemy[type] = 0;
        }
        defeatedEnemy[type] = defeatedEnemy[type] + 1;
    }

    /// <summary>
    /// スコアリセット
    /// </summary>
    public void ResetScore()
    {
        score.Reset();
        defeatedEnemy.Clear();
    }

    /// <summary>
    /// 倒した敵の数を取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetDefeatedEnemy(Enemy.ENEMY_TYPE type)
    {
        if(!defeatedEnemy.ContainsKey(type))
        {
            return 0;
        }

        return defeatedEnemy[type];
    }

    /// <summary>
    /// 最終スコア取得
    /// </summary>
    /// <returns></returns>
    public float GetResultScore()
    {
        var result = score.GetScore();
        if(defeatedEnemy.ContainsKey(Enemy.ENEMY_TYPE.PAWN))
        {
            result += defeatedEnemy[Enemy.ENEMY_TYPE.PAWN] * Define.pawnScore;
        }
        if (defeatedEnemy.ContainsKey(Enemy.ENEMY_TYPE.ROOK))
        {
            result += defeatedEnemy[Enemy.ENEMY_TYPE.ROOK] * Define.rookScore;
        }
        if (defeatedEnemy.ContainsKey(Enemy.ENEMY_TYPE.BISHOP))
        {
            result += defeatedEnemy[Enemy.ENEMY_TYPE.BISHOP] * Define.bishopScore;
        }

        return result;
    }
}
