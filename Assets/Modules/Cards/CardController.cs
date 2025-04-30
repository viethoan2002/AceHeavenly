using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Cards
{
   public class CardController : MonoBehaviour
   {
      [SerializeField] private Button button;
      [SerializeField] private CanvasGroup canvasGroup;
      [SerializeField] private Sprite defaultSprite,highlightSprite;
      [SerializeField] private Text runText, jumpText;
      [SerializeField] private PathMove pathMove;

      public PathMove PathMove => pathMove;

      private void OnEnable()
      {
         canvasGroup.alpha = 1;
      }

      public void AddListener(Action action)
      {
         button.onClick.AddListener(() =>
         {
            action?.Invoke();
         });
      }

      public void ClearListener()
      {
         button.onClick.RemoveAllListeners();
      }

      public void HighlightCard(bool en)
      {
         button.image.sprite = en ? highlightSprite : defaultSprite;
      }

      public void HideCard()
      {
         canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
         {
            gameObject.SetActive(false);
         });
      }

      public void ShowCard()
      {
         canvasGroup.alpha = 1;
         gameObject.SetActive(true);
      }

      public void SetupPathMove(PathMove path)
      {
         this.pathMove = path;
         foreach (var move in pathMove.moves)
         {
            if (move.movementType == MovementType.Jump)
               jumpText.text = move.distanceMove.ToString();
            else
               runText.text = move.distanceMove.ToString();
         }
      }
   }
}
