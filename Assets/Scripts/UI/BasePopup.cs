using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class BasePopup : MonoBehaviour
{
    public bool isShow;
    
    [SerializeField] protected CanvasGroup main;
    
    protected float timeShow = .5f;
    protected float alphaWhenShow = 0;

    public virtual void ShowImmediately(bool showImmediately, Action actionComplete = null)
    {
        isShow = true;
        DOTween.Kill(main);
        gameObject.SetActive(true);
        if (showImmediately)
        {
            main.alpha = 1;
            actionComplete?.Invoke();
        }
        else
        {
            main.DOFade(1, timeShow).From(alphaWhenShow).SetUpdate(true).OnComplete(() =>
            {
                actionComplete?.Invoke();
            });
        }
    }

    public virtual void HideImmediately(bool hideImmediately, Action actionComplete = null)
    {
        isShow = false;
        if (hideImmediately)
        {
            gameObject.SetActive(false);
            actionComplete?.Invoke();
        }
        else
        {
            main.DOFade(0, timeShow).SetUpdate(true).OnComplete(() =>
            {
                gameObject.SetActive(false);
                actionComplete?.Invoke();
            });
        }
    }
    
    protected virtual void OnDisable()
    {
        main.DOKill();
    }
}