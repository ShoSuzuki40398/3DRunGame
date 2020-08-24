using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreText : MonoBehaviour
{
    [SerializeField]
    private Text highScoreText;

    [SerializeField]
    private GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        var highScore = gameData.currentHighScore;
        highScoreText.text = "HighScore : " + highScore;
    }
}
