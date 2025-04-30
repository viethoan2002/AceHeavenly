using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Ball;
using Modules.Cards;
using Modules.Command;
using Modules.Data;
using Modules.Map;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private List<LevelData> levelDatas = new List<LevelData>();
    [SerializeField] private BallController ball;
    
    public MapController mapController;
    public int indexCurrentLevel;
    
    private void Awake()
    {
        Instance = this;
        PopupCtrl.Instance.GetPopupByType<PopupHome>().HideImmediately(true);
        PopupCtrl.Instance.GetPopupByType<PopupGameplay>().ShowImmediately(false);
        LoadGame(0);
    }
    
    public void LoadGame(int indexLevel)
    {
        StartCoroutine(LoadGameCoroutine(indexLevel));
    }

    IEnumerator LoadGameCoroutine(int indexLevel)
    {
        yield return new WaitForSeconds(0.01f);
        if(mapController != null)
            Destroy(mapController.gameObject);

        CommandInvoke.ClearCommands();
        indexCurrentLevel = indexLevel;
        
        CardManager.Instance.SetupCards(levelDatas[indexCurrentLevel].pathMoves);
        var newMap = Instantiate(levelDatas[indexLevel].levelPrefab).GetComponent<MapController>();
        mapController = newMap;
        ball.SetupBall(mapController.posSpawnPlayer.position);
        
        yield return new WaitForSeconds(0.01f);
        CircleOutline.Instance.ScaleOut();
    }

    public void ResetLevel()
    {

    }

    public void WinGame()
    {
        StartCoroutine(WinGameCoroutine());
    }

    IEnumerator WinGameCoroutine()
    {
        yield return new WaitForSeconds(2f);
        CircleOutline.Instance.ScaleIn(() =>
        {
            LoadGame(indexCurrentLevel + 1);
        });
    }
}
