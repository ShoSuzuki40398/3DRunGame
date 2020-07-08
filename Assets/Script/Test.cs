using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    FadeController.Instance.FadeOut(1.0f);
        //    //AudioManager.Instance.PlayBGM(Define.BGM.BGM_1);
        //}
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            FadeController.Instance.FadeOut(1.0f);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            FadeController.Instance.FadeIn(1.0f);
        }
        //else if (Input.GetKeyUp(KeyCode.LeftControl))
        //{
        //    AudioManager.Instance.UnPause();
        //}
        //else if (Input.GetKeyUp(KeyCode.A))
        //{
        //    AudioManager.Instance.PlaySE(Define.SE.SE_1);
        //}
        //else if (Input.GetKeyUp(KeyCode.S))
        //{
        //    AudioManager.Instance.PlaySE(Define.SE.SE_2);
        //}
        //else if (Input.GetKeyUp(KeyCode.D))
        //{
        //    AudioManager.Instance.StopAllSE();
        //}
    }
}
