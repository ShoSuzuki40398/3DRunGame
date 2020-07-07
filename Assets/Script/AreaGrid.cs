﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaGrid : MonoBehaviour
{
    public Vector3 gridPosition { get { return transform.position; } private set { } }

    public Vector3 gridSize { get { return GetComponent<Renderer>().bounds.size; } private set { } }

    // このマス目上に敵を生成するか
    [SerializeField]
    private bool enemySpawn = false;

    // 生成する敵の種類
    [SerializeField]
    private Enemy.ENEMY_TYPE enemyType = Enemy.ENEMY_TYPE.PAWN;

    /// <summary>
    ///初期化
    /// </summary>
    public void Initialize(Transform enemyRoot)
    {
        if(enemySpawn)
        {
            Enemy enemy = EnemyGenerator.Create(enemyType,enemyRoot);
            var enemyHeight = enemy.GetComponent<Renderer>().bounds.size.y;
            enemy.transform.position = new Vector3(gridPosition.x, gridPosition.y + enemyHeight*0.75f, gridPosition.z);
        }
    }
}