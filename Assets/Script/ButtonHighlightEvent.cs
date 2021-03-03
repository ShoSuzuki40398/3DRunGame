using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonHighlightEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        transform.localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button == null)
        {
            return;
        }

        if (!button.interactable)
        {
            return;
        }

        AudioManager.Instance.PlaySE(Define.SE.BUTTON_HIGHLIGHT);

        if(Pauser.Instance.GetState() == Pauser.STATE.RESUME)
        {
            var tween = transform.DOScale(1.2f, 0.1f);
            tween.Play();
        }
        else if (Pauser.Instance.GetState() == Pauser.STATE.PAUSE)
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 0.0f);
        }
    }

    public void OnPointerEnter()
    {
        if(button == null)
        {
            return;
        }

        if (!button.interactable)
        {
            return;
        }

        AudioManager.Instance.PlaySE(Define.SE.BUTTON_HIGHLIGHT);

        if (Pauser.Instance.GetState() == Pauser.STATE.RESUME)
        {
            var tween = transform.DOScale(1.2f, 0.1f);
            tween.Play();
        }
        else if(Pauser.Instance.GetState() == Pauser.STATE.PAUSE)
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 0.0f);
        }
    }

    public void OnPointerEnter()
    {
        if (!button.interactable)
        {
            return;
        }

        AudioManager.Instance.PlaySE(Define.SE.BUTTON_HIGHLIGHT);

        if (Pauser.Instance.GetState() == Pauser.STATE.RESUME)
        {
            var tween = transform.DOScale(1.2f, 0.1f);
            tween.Play();
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button == null)
        {
            return;
        }

        if (!button.interactable)
        {
            return;
        }

        if (Pauser.Instance.GetState() == Pauser.STATE.RESUME)
        {
            var tween = transform.DOScale(1.0f, 0.1f);
            tween.Play();
        }
        else if (Pauser.Instance.GetState() == Pauser.STATE.PAUSE)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
        }
    }

    public void OnPointerExit()
    {
        if (button == null)
        {
            return;
        }

        if (!button.interactable)
        {
            return;
        }

        if (Pauser.Instance.GetState() == Pauser.STATE.RESUME)
        {
            var tween = transform.DOScale(1.0f, 0.1f);
            tween.Play();
        }
        else if (Pauser.Instance.GetState() == Pauser.STATE.PAUSE)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
        }
    }

    public void OnPointerExit()
    {
        if (!button.interactable)
        {
            return;
        }

        if (Pauser.Instance.GetState() == Pauser.STATE.RESUME)
        {
            var tween = transform.DOScale(1.0f, 0.1f);
            tween.Play();
        }
    }
}
