using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityARMODApp.Runtime
{
    public class ShowcaseElement:MonoBehaviour,IPointerClickHandler
    {
        public Text ShowcaseTitleText;
        public Text ShowcaseBriefText;
        public Image ShowcaseIconImage;
        public string ShowcaseId;
        public Action<string> ClickedEvent;
        
        /// <summary>
        /// Clicked event
        /// </summary>
        /// <param name="_eventData"></param>
        public void OnPointerClick(PointerEventData _eventData)
        {
            ClickedEvent?.Invoke(ShowcaseId);
        }
    }
}