using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SinkiiLib.UI
{
    public class UIManager : MonoBehaviour
    {
        protected UIDocument m_Document;
        protected VisualElement root;
        protected virtual void OnEnable()
        {
            m_Document = GetComponent<UIDocument>();
            SetupViews();
            SubscribeEvents();
        }
        protected virtual void OnDisable()
        {
            UnSubscribeEvents();
        }


        protected virtual void SetupViews()
        {
            root = m_Document.rootVisualElement;
        }


        protected virtual void SubscribeEvents()
        {

        }
        protected virtual void UnSubscribeEvents()
        {
        }
    }
}