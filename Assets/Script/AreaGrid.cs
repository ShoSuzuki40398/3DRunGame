using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaGrid : MonoBehaviour
{
    public Vector3 gridPosition { get { return transform.position; } private set { } }
    
    public Vector3 gridSize { get { return GetComponent<Renderer>().bounds.size; } private set { } }
}
