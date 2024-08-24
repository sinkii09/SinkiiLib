using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SinkiiLib.Systems
{
    public class SceneGroup
    {
        public string GroupName = "New Scene Group";
        public List<SceneData> Scenes;

        public string FindSceneNameByType(SceneType type)
        {
            return Scenes.FirstOrDefault(scene => scene.SceneType == type)?.Name;
        }
    }
    public class SceneData
    {
        public Scene Reference;
        public string Name => Reference.name;

        public SceneType SceneType;
    }
    public enum SceneType { ActiveScene, MainMenu, HUD}
}
