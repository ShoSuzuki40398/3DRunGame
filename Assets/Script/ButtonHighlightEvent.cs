using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHighlightEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!button.interactable)
        {
            return;
        }

        AudioManager.Instance.PlaySE(Define.SE.BUTTON_HIGHLIGHT);
        var tween = transform.DOScale(1.2f, 0.1f);
        tween.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }

        var tween = transform.DOScale(1.0f, 0.1f);
        tween.Play();
    }
}
