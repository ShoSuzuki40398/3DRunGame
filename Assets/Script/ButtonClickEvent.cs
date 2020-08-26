using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClickEvent : MonoBehaviour, IPointerClickHandler
{
    // 初期スケール値
    private Vector3 initScale = Vector3.one;

    private void Start()
    {
        initScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySE(Define.SE.BUTTON_CLICK);
        transform.localScale = initScale;
    }
}
