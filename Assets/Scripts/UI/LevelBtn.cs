using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBtn : MonoBehaviour
{
    [SerializeField] private Button btn;

    public void AddListener(Action action)
    {
        btn.onClick.AddListener(() =>
        {
            action?.Invoke();
        });
    }

    public void SetLock(bool en)
    {

    }
}
