using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityARMODApp.Runtime
{
    public class RecommendElement : MonoBehaviour
    {
        public Text RecommendTitleText;
        public Text RecommendBriefText;
        public Image RecommendHeaderImage;
        public string ShowcaseId;
        public Button ClickButton;

        public LayoutElement LayoutElement;

        private void Awake()
        {
            AdaptUI();
        }

        private void AdaptUI()
        {
            var tmp_CanvasTrans = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
            var tmp_Rect = tmp_CanvasTrans.rect;
            GetComponent<RectTransform>().sizeDelta =
                new Vector2(tmp_Rect.width, tmp_Rect.width / 1.618f + 46 + 28.7294f);

            LayoutElement.preferredWidth = tmp_Rect.width;
            LayoutElement.preferredHeight = tmp_Rect.height;
        }
    }
}