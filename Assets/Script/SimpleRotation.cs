using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField]
    private Vector3 deltaRotateAngle = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(deltaRotateAngle * Time.deltaTime, Space.World);
    }
}
