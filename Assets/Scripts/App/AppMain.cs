using System.Threading.Tasks;
using com.phantoms.models.Runtime;
using NativePlugins.Plugin;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityARMODApp.Runtime
{
    public class AppMain : MonoBehaviour
    {
        public SDKConfiguration SDKConfiguration;
        public GameObject Home;
        public GameObject ARView;
        public CanvasGroup Background;
        public AlertWindow AlertWindow;
        public LoadingProgressView LoadingView;

        private RectTransform canvasRectTransform;
        private EventSystem appBuiltInEventSystem;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            canvasRectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            appBuiltInEventSystem = EventSystem.current;

            //AlertWindow.Close.onClick.AddListener(Utility.DisableAR);


            NativeAPI.NeedInstallARCoreServicesEventHandle += () =>
            {
                AlertWindow.gameObject.SetActive(true);
                AlertWindow.BodyText.text = "Your device is available ARMOD,But you need to install ARCore Service!";
                AlertWindow.ShowAlertWindow(true);
            };

            NativeAPI.ThrowExceptionEventHandle += (_message, _errorcode) =>
            {
                AlertWindow.gameObject.SetActive(true);
                AlertWindow.BodyText.text = $"ERROR:{_message},ERROR Code:{_errorcode}";
                AlertWindow.ShowAlertWindow(true);
                Debug.LogError(_message);
            };
            NativeAPI.AddLoadingOverlayEventHandle += () => { LoadingView.gameObject.SetActive(true); };
            NativeAPI.RemoveLoadingOverlayEventHandle += async () =>
            {
                LoadingView.gameObject.SetActive(false);

                await Task.Delay(1000);
                //Avoid more then one event system in the scene.
                var tmp_EventSystems = FindObjectsOfType<EventSystem>();
                if (tmp_EventSystems.Length > 1)
                    appBuiltInEventSystem.gameObject.SetActive(false);
            };
            NativeAPI.UpdateLoadingProgressEventHandle += (_progress) =>
            {
                LoadingView.progressText.text = $"{_progress * 100f}%";
            };
            NativeAPI.OnARMODLaunchEventHandle += () => { Debug.Log("ARMOD Launched"); };
            NativeAPI.OnARMODExitEventHandle += () =>
            {
                AlertWindow.ShowAlertWindow(false);
                Home.SetActive(true);
                ARView.SetActive(false);
                Background.alpha = 1;
                Background.gameObject.SetActive(true);
                
                if (!appBuiltInEventSystem.gameObject.activeSelf)
                    appBuiltInEventSystem.gameObject.SetActive(true);
            };
        }
    }
}