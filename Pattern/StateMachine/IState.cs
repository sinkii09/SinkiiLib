using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinkiiLib.Pattern
{
    public interface IState 
    {
        void OnEnter();
        void OnUpdate();
        void OnFixedUpdate();
        void OnExit();
    }
}
