using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaLine : MonoBehaviour
{
    // マス目情報
    [HideInInspector]
    public List<AreaGrid> grids;

    // 先頭マス
    public AreaGrid leadGrid { get { return grids[0]; } }
    // 末尾マス
    public AreaGrid endGrid { get { return grids[grids.Count-1]; } }

    // マス目の数
    public int gridCount { get { return grids.Count; } }

    private void Start()
    {
        // マス目取得
        grids = new List<AreaGrid>(GetComponentsInChildren<AreaGrid>());
    }
}
