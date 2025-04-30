using Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PopupHome : BasePopup
    {
        [SerializeField] private Button btnPlay, btnGuid;

        private void Awake()
        {
            btnPlay.onClick.AddListener(() =>
            {
                CircleOutline.Instance.ScaleIn(() =>
                {
                    PopupCtrl.Instance.GetPopupByType<PopupGameplay>().ShowImmediately(false);
                    SceneManager.LoadSceneAsync(Loader.Scene.Gameplay.ToString());
                });
            });

            btnGuid.onClick.AddListener(() =>
            {
                HideImmediately(true);
                PopupCtrl.Instance.GetPopupByType<PopupGuid>().ShowImmediately(false);
            });
        }
    }
}
