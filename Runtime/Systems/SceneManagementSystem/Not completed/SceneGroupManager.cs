using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SinkiiLib.Systems
{
    public class SceneGroupManager 
    {
        SceneGroup ActiveSceneGroup;
        public void UnLoadScenes()
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            int sceneCount = SceneManager.sceneCount;

            for (int i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if(!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName == "Bootstrapper") continue;

            }
        }
    }
    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0:Operations.Average(o=>o.progress);
        public bool IsDone => Operations.All(o=>o.isDone);

        public AsyncOperationGroup(int capacity)
        {
            Operations = new List<AsyncOperation>(capacity);
        }
    }
}
