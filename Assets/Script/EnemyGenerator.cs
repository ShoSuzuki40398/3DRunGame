using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyGenerator
{
    private const string pawnPath = Define.enemyPath + "Enemy1";
    private const string rookPath = Define.enemyPath + "Enemy2";
    private const string bishopPath = Define.enemyPath + "Enemy3";

    public static Enemy Create(Enemy.ENEMY_TYPE type, Transform enemyRoot = null)
    {
        Enemy result;
        switch (type)
        {
            case Enemy.ENEMY_TYPE.PAWN: result = CreatePawn(); break;
            case Enemy.ENEMY_TYPE.BISHOP: result = CreateBishop(); break;
            case Enemy.ENEMY_TYPE.ROOK: result = CreateRook(); break;
            default: result = CreatePawn();break;
        }
        if(enemyRoot != null)
        {
            result.transform.parent = enemyRoot;
        }
        return result;
    }

    /// <summary>
    /// ポーン作成
    /// </summary>
    /// <returns></returns>
    private static Enemy CreatePawn()
    {
        GameObject obj = Resources.Load<GameObject>(pawnPath);
        var instance = GameObject.Instantiate(obj);
        var enemy = instance.ForceGetComponent<Enemy>();
        return enemy;
    }

    /// <summary>
    /// ビショップ作成
    /// </summary>
    /// <returns></returns>
    private static Enemy CreateBishop()
    {
        GameObject obj = Resources.Load<GameObject>(bishopPath);
        var instance = GameObject.Instantiate(obj);
        var enemy = instance.ForceGetComponent<Enemy>();
        return enemy;
    }

    /// <summary>
    /// ルーク作成
    /// </summary>
    /// <returns></returns>
    private static Enemy CreateRook()
    {
        GameObject obj = Resources.Load<GameObject>(rookPath);
        var instance = GameObject.Instantiate(obj);
        var enemy = instance.ForceGetComponent<Enemy>();
        return enemy;
    }
}
