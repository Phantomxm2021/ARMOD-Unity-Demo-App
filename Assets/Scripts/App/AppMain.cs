using com.Phantoms.NativePlugins;
using com.Phantoms.SDKConfigures.Runtime;
using UnityEngine;

namespace UnityARMODApp.Runtime
{
    public class AppMain : MonoBehaviour
    {
        public SDKConfiguration SDKConfiguration;
        public GameObject Home;
        public GameObject ARView;
        public CanvasGroup Background;
        public RectTransform RecommendScrollView;
        public RectTransform AllScrollView;
        public AlertWindow AlertWindow;

        private RectTransform canvasRectTransform;

        private void Awake()
        {
            Utility.CleanCache();
            DontDestroyOnLoad(this.gameObject);
            canvasRectTransform = GetComponent<RectTransform>();
            AdaptRecommendView();
            AdaptAllScrollView();
        }

        private void Start()
        {
            AlertWindow.Close.onClick.AddListener(() =>
            {
                AlertWindow.ShowAlertWindow(false);
                Home.SetActive(true);
                ARView.SetActive(false);
                Background.alpha = 1;
                Background.gameObject.SetActive(true);
                
                Utility.DisableAR();
            });

            NativeAPI.ThrowExceptionEventHandle += (_s, _i) =>
            {
                AlertWindow.gameObject.SetActive(true);
                AlertWindow.BodyText.text = $"ERROR:{_s},ERROR Code:{_i}";
                AlertWindow.ShowAlertWindow(true);
                Debug.LogError(_s);
            };

            NativeAPI.OnARMODLaunchEventHandle += () => { Debug.Log("ARMOD Launched"); };
        }

        private void AdaptRecommendView()
        {
            var tmp_Rect = canvasRectTransform.rect;

            //Resize our recommend scroll view. Make it looks beautiful.
            //1.618f is Golden ratio value.
            //46 and 28.7294f are the height of text label
            RecommendScrollView.sizeDelta =
                new Vector2(RecommendScrollView.sizeDelta.x, tmp_Rect.width / 1.618f + 46 + 28.7294f);
        }


        private void AdaptAllScrollView()
        {
            //Connect to the bottom of the Recommend UI
            AllScrollView.offsetMax =
                new Vector2(AllScrollView.offsetMax.x, RecommendScrollView.offsetMin.y);
        }
    }
}