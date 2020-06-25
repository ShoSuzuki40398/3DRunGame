using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    protected void Awake()
    {
        AudioManager.Instance.Awake();
    }
}
