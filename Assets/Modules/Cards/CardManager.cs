using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Modules.Cards
{
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance;
        [SerializeField] private List<CardController> cardControllers = new List<CardController>();
        [SerializeField] private RectTransform start, end;
        [SerializeField] private bool canSelectCard;
        [SerializeField] private int activeCards;

        public static Action<CardController> OnSelectCard;

        private void Awake()
        {
            Instance = this;
        }

        public void SetupCards(List<PathMove> pathMoves)
        {
            foreach (var car in cardControllers)
            {
                ObjectPool.Instance.Return(car.gameObject,true);
            }
            
            cardControllers.Clear();
            
            foreach (var pathMove in pathMoves)
            {
                if (pathMove.moves.Count > 1)
                {
                    var newCard = ObjectPool.Instance.Get(ObjectPool.Instance.cardMove).GetComponent<CardController>();
                    newCard.SetupPathMove(pathMove);
                    newCard.transform.SetParent(transform);
                    cardControllers.Add(newCard);
                }
                else
                {
                    if (pathMove.moves[0].movementType == MovementType.Jump)
                    {
                        var newCard = ObjectPool.Instance.Get(ObjectPool.Instance.cardJump).GetComponent<CardController>();
                        newCard.SetupPathMove(pathMove);
                        newCard.transform.SetParent(transform);
                        cardControllers.Add(newCard);
                    }
                    else
                    {
                        var newCard = ObjectPool.Instance.Get(ObjectPool.Instance.cardRun).GetComponent<CardController>();
                        newCard.SetupPathMove(pathMove);
                        newCard.transform.SetParent(transform);
                        cardControllers.Add(newCard);
                    }
                        
                }
            }

            if (cardControllers.Count == 1)
            {
                cardControllers[0].transform.position =
                    Vector3.Lerp(start.transform.position, end.transform.position, 0.5f);
            }
            else for (int i = 0; i < cardControllers.Count; i++)
            {
                int index = i;
                cardControllers[i].transform.position = Vector3.Lerp(start.transform.position, end.transform.position,
                    (float)i / ((float)cardControllers.Count - 1));
                cardControllers[index].ClearListener();
                cardControllers[index].AddListener(() =>
                {
                    HighlightCard(cardControllers[index]);
                    OnSelectCard?.Invoke(cardControllers[index]);
                });
            }
            
            activeCards = cardControllers.Count;
            
            HighlightCard(cardControllers[0]);
            OnSelectCard?.Invoke(cardControllers[0]);
        }

        private void HighlightCard(CardController cardController)
        {
            foreach (var car in cardControllers)
            {
                car.HighlightCard(car == cardController);
                car.GetComponent<RectTransform>().anchoredPosition = car == cardController ? new Vector2(car.GetComponent<RectTransform>().anchoredPosition.x, 88) 
                    : new Vector2(car.GetComponent<RectTransform>().anchoredPosition.x, 60);
            }
        }

        public void RemoveCard(CardController cardController)
        {
            StartCoroutine(RemoveCardCoroutine(cardController));
        }

        IEnumerator RemoveCardCoroutine(CardController cardController)
        {
            cardController.HideCard();
            yield return new WaitForSeconds(0.5f);
            activeCards -= 1;
            
            if(activeCards == 0)
                yield break;
            
            SortCards();
            foreach (var card in cardControllers)
            {
                if (card.gameObject.activeInHierarchy)
                {
                    HighlightCard(card);
                    OnSelectCard?.Invoke(card);
                    break;
                }
            }
        }

        public void AddCard(CardController cardController)
        {
            cardController.ShowCard();
            
            HighlightCard(cardController);
            OnSelectCard?.Invoke(cardController);
        }

        public void SortCards()
        {
            int indexActive = 0;
            if (activeCards == 1)
            {
                foreach (var t in cardControllers)
                {
                    if (t.gameObject.activeInHierarchy)
                    {
                        t.transform.position =
                            Vector3.Lerp(start.transform.position, end.transform.position, 0.5f);
                        break;
                    }
                }
            }
            else foreach (var t in cardControllers)
            {
                if(!t.gameObject.activeInHierarchy)
                    continue;
                t.transform.position = Vector3.Lerp(start.transform.position, end.transform.position,
                    (float)indexActive / ((float)activeCards - 1));
                indexActive += 1;
            }
        }
    }
}
