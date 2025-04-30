using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PopupGameplay : BasePopup
    {
        [SerializeField] private Button btnReset, btnHome;

        private void Awake()
        {
            btnReset.onClick.AddListener(() =>
            {
                CircleOutline.Instance.ScaleIn(() =>
                {
                    GameManager.Instance.LoadGame(GameManager.Instance.indexCurrentLevel);
                    CircleOutline.Instance.ScaleOut();
                });
            });

            btnHome.onClick.AddListener(() =>
            {
                CircleOutline.Instance.ScaleIn(() =>
                {
                    GameManager.Instance.ResetLevel();
                    HideImmediately(true);
                    PopupCtrl.Instance.GetPopupByType<PopupHome>().ShowImmediately(false);
                    CircleOutline.Instance.ScaleOut();
                });
            });
        }
    }
}
