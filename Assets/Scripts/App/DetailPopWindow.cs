using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityARMODApp.Runtime
{
    public class DetailPopWindow : MonoBehaviour
    {
        public Text ARExperienceTitleText;
        public Image ARExperienceIconImage;
        public Image ARExperienceHeaderImage;
        public Text ARExperienceDescriptionText;
        public Button ARExperienceButton;
        public Button MaskButton;
        public Animator Animator;
        private static readonly int SHOW = Animator.StringToHash("Show");

        public void PopWindow(bool _show)
        {
            Animator.SetBool(SHOW,_show);
        }
    }
}
