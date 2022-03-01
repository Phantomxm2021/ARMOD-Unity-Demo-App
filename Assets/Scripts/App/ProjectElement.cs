using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityARMODApp.Runtime
{
    public class ProjectElement:MonoBehaviour,IPointerClickHandler
    {
        public Text ProjectTitleText;
        public Text ProjectBriefText;
        public Image ProjectIconImage;
        public string ProjectId;
        public Action<string> ClickedEvent;
        
        /// <summary>
        /// Clicked event
        /// </summary>
        /// <param name="_eventData"></param>
        public void OnPointerClick(PointerEventData _eventData)
        {
            ClickedEvent?.Invoke(ProjectId);
        }
    }
}