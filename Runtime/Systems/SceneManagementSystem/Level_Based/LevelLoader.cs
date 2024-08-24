using log4net.Core;
using SinkiiLib.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SinkiiLib.Systems
{
    public class LevelLoader : Singleton<LevelLoader>
    {
        private void OnEnable()
        {
            //EventManager.Addlistener("LoadTransitionEnd");
        }
        private void OnDisable()
        {
            
        }
        void OnTransitionEnd()
        {

        }
        public void ToLevel(int level)
        {
            StartCoroutine(LoadLevelAsync(level));
        }
        public void ToMainMenu()
        {
            StartCoroutine(LoadMainMenuAsync());
        }
        IEnumerator LoadLevelAsync(int level)
        {
            EventManager.TriggerEvent("ToggleLoaderScreen", true);
            yield return new WaitForSeconds(1f);

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Level " + level);
            while(!asyncOperation.isDone)
            {
                yield return null;
            }
            EventManager.TriggerEvent("ToggleLoaderScreen", false);
        }
        IEnumerator LoadMainMenuAsync()
        {
            EventManager.TriggerEvent("ToggleLoaderScreen", true);
            yield return new WaitForSeconds(1f);

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Main Menu");
            while(!asyncOperation.isDone)
            {
                yield return null;
            }
            EventManager.TriggerEvent("ToggleLoaderScreen", false);
        }
    }
}
