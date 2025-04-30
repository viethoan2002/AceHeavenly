using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loading
{
    public static class Loader
    {
        private class LoadingMonoBehaviour : MonoBehaviour{}
        public enum Scene{
            MainMenu ,
            Loading ,
            Gameplay
        }

        private static Action _loaderCallback;
        private static AsyncOperation _loadingAsyncOperation;
        public static float Progress = 0;
    
        public static void LoadScene(Scene scene)
        {
            _loaderCallback = () =>
            {
                GameObject loadingGameObject = new GameObject("Loading GameObject");
                loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
            };
            
            SceneManager.LoadScene(Scene.Loading.ToString());
        }

        private static IEnumerator LoadSceneAsync(Scene scene)
        {
            yield return null;
            _loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
            if (_loadingAsyncOperation != null) _loadingAsyncOperation.allowSceneActivation = false;
            Progress = 0;
            while (_loadingAsyncOperation is { isDone: false })
            {
                Progress = Mathf.MoveTowards(Progress, _loadingAsyncOperation.progress, Time.deltaTime);
                if (Progress >= 0.9f)
                {
                    Progress = 1;
                    _loadingAsyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        public static void LoaderCallback()
        {
            if (_loaderCallback != null)
            {
                _loaderCallback();
                _loaderCallback = null;
            }
        }
    }
}
