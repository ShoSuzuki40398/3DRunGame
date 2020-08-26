using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // 追従対象
    [SerializeField]
    private Transform target = null;

    // 離れる距離
    [SerializeField]
    private Vector3 offset;

    private bool isFollow = false;

    // Update is called once per frame
    void Update()
    {
        if (isFollow && target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target.transform);
        }
    }

    /// <summary>
    /// 追従対象設定
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    /// <summary>
    /// 追従ON
    /// </summary>
    public void EnableFollow()
    {
        if (target != null)
        {
            isFollow = true;
        }
    }

    /// <summary>
    /// 追従ON
    /// </summary>
    public void DisableFollow()
    {
        isFollow = false;
    }
}
