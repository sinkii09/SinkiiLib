using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SinkiiLib.Systems
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadSceneGroup(int index)
        {
            
        }
        void EnableLoadingUI(bool enable = false)
        {

        }
    }
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> OnProgress;

        const float ratio = 1.0f;
        public void Report(float value)
        {
            OnProgress?.Invoke(value/ratio);
        }
    }
}
