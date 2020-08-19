using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = Define.scriptablePath + "GameData")]
public class GameData : ScriptableObject
{
    public string playerSkinName = Define.shaderPathStandard;

    public int currentHighScore = 0;
    
    public List<int> releaseSkinScoreBorder = new List<int>();
}
