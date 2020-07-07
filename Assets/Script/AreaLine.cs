using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaLine : MonoBehaviour
{
    // マス目情報
    [HideInInspector]
    public List<AreaGrid> grids;

    // 先頭マス
    public AreaGrid leadGrid;
    // 末尾マス
    public AreaGrid endGrid;

    // マス目の数
    public int gridCount { get { return grids.Count; } }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(Transform enemyRoot)
    {
        // マス目取得
        grids = new List<AreaGrid>(GetComponentsInChildren<AreaGrid>());

        leadGrid = grids[0];
        endGrid = grids[grids.Count - 1];

        for(int i = 0; i < gridCount; ++i)
        {
            grids[i].Initialize(enemyRoot);
        }
    }
}
