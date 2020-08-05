using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHighlightEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySE(Define.SE.BUTTON_HIGHLIGHT);
        var tween = transform.DOScale(1.2f, 0.1f);
        tween.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var tween = transform.DOScale(1.0f, 0.1f);
        tween.Play();
    }
}
