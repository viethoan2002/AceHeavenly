using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PopupGuid : BasePopup
    {
        [SerializeField] private Button btnExit;

        private void Awake()
        {
            btnExit.onClick.AddListener(() =>
            {
                HideImmediately(true);
                PopupCtrl.Instance.GetPopupByType<PopupHome>().ShowImmediately(false);
            });
        }
    }
}
