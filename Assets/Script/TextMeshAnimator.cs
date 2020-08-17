using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextMeshAnimator : MonoBehaviour
{
    private TMP_Text textMeshPro;

    [SerializeField,Range(0.0f,10.0f)]
    private float sinWaveValue = 1.0f;

    private void Awake()
    {
        textMeshPro = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        // メッシュを再生成する（リセット）
        textMeshPro.ForceMeshUpdate(true);
        //テキストメッシュプロの情報
        var textInfo = textMeshPro.textInfo;

        // 頂点データを編集した配列の作成
        var count = Mathf.Min(textInfo.characterCount, textInfo.characterInfo.Length);
        for (int i = 0; i < count; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // Wave
            Vector3[] verts = textInfo.meshInfo[materialIndex].vertices;

            float sinWaveOffset = 0.5f * i;
            float sinWave = Mathf.Sin(sinWaveOffset + Time.realtimeSinceStartup * Mathf.PI);
            verts[vertexIndex + 0].y += sinWave * sinWaveValue;
            verts[vertexIndex + 1].y += sinWave * sinWaveValue;
            verts[vertexIndex + 2].y += sinWave * sinWaveValue;
            verts[vertexIndex + 3].y += sinWave * sinWaveValue;
        }

        // メッシュを更新
        for (int i = 0; i < textInfo.materialCount; i++)
        {
            if (textInfo.meshInfo[i].mesh == null) { continue; }

            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;  // 変更
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
