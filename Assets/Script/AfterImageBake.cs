using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageBake : MonoBehaviour
{
    // 複製した残像
    private List<GameObject> clones = new List<GameObject>();

    // 残像用プレハブ
    [SerializeField]
    private GameObject clonePrefab;

    // 残像対象のオブジェクト
    [SerializeField]
    private MeshRenderer targetRenderer;

    // 残像マテリアル
    [SerializeField]
    private Material afterImageMaterial;

    // 残像数
    [SerializeField, Range(1, 10)]
    private int cloneCount = 3;

    // 残像の更新頻度(frame)
    // 値が高いほど本体から残像が離れる
    [SerializeField, Range(1, 10)]
    private int updateCount = 4;

    // 現在の残像の更新カウント(frame)
    private int currentUpdateCount = 0;

    // 追従の重み
    // 値が低いほどくっつくように残像を残す
    // 値が高いほど残像同士が離れる
    [SerializeField, Range(0.1f, 0.9f)]
    private float ghostingWeight = 0.1f;

    // Update is called once per frame
    void FixedUpdate()
    {
        currentUpdateCount++;
        if (currentUpdateCount % updateCount != 0)
        {
            return;
        }

        CloneUpdate();
    }

    /// <summary>
    /// 残像の座標更新
    /// </summary>
    private void CloneUpdate()
    {
        Vector3 firstPosition = targetRenderer.transform.position;
        Quaternion firstRotaion = targetRenderer.transform.rotation;

        for (int i = 0; i < clones.Count; ++i)
        {
            clones[i].transform.position = Vector3.Lerp(firstPosition, clones[i].transform.position, ghostingWeight);
            clones[i].transform.rotation = Quaternion.Lerp(firstRotaion, clones[i].transform.rotation, ghostingWeight);

            firstPosition = clones[i].transform.position;
            firstRotaion = clones[i].transform.rotation;
        }
    }

    /// <summary>
    /// 残像作成
    /// </summary>
    public void Bake(bool enable = true)
    {
        if (clones.Count != 0)
        {
            return;
        }

        for (int i = 0; i < cloneCount; ++i)
        {
            var obj = Instantiate(clonePrefab);
            obj.transform.localScale = targetRenderer.transform.localScale;
            obj.GetComponent<MeshFilter>().mesh = targetRenderer.GetComponent<MeshFilter>().mesh;
            clones.Add(obj);

            obj.GetComponent<MeshRenderer>().material = afterImageMaterial;
            obj.GetComponent<MeshRenderer>().enabled = enable;
        }
    }

    /// <summary>
    /// 残像表示の設定
    /// </summary>
    public void SetRenderEnable(bool enable)
    {
        foreach (var obj in clones)
        {
            obj.GetComponent<MeshRenderer>().enabled = enable;
        }
    }

    /// <summary>
    /// 残像をすべて削除
    /// </summary>
    public void DestoryAfterImage()
    {
        for (int i = 0; i < clones.Count; ++i)
        {
            Destroy(clones[i]);
        }

        clones.Clear();
    }

    /// <summary>
    /// 残像色変更
    /// </summary>
    /// <param name="color"></param>
    public void ChangeColor(Color color)
    {
        afterImageMaterial.SetColor("_BaseColor", color);
    }
}
