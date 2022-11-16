using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnityARMODApp.Runtime
{
    public class XRProjectRequest : MonoBehaviour
    {
        public GameObject RecommendUIPrefab;
        public GameObject ARExperienceListItemPrefab;


        public Transform RecommendListHolder;
        public Transform ARExperienceListHolder;
        public DetailPopWindow DetailPopWindow;
        private XRProjectMapper xrProjectMapper;
        private RecommendXRProjectMapper recommendXRProjectMapper;

        private bool initialized = false;

        /// <summary>
        /// Accessing the ARMOD Web Services API
        /// </summary>
        /// <param name="_url">ARMOD web services url</param>
        /// <param name="_projectId">current showcase id,It will be use to query the current showcase detail information(Option)</param>
        /// <returns></returns>
        private async Task<string> PostNetworkQuery(string _url, string _projectId = "")
        {
            string tmp_Response = string.Empty;
            IDictionary<string, string> tmp_Headers = new Dictionary<string, string>();
            tmp_Headers.Add("Authorization",
                $"Token {FindObjectOfType<AppMain>().SDKConfiguration.dashboardConfig.token}");
            tmp_Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            var tmp_Form = new WWWForm();
            tmp_Form.AddField("packageid", Application.identifier);
            tmp_Form.AddField("project_id", _projectId);
            var progress = Progress.Create<float>(NativePlugins.Plugin.NativeAPI.UpdateLoadingProgress);
            var tmp_WebRequestSender = UnityWebRequest.Post(_url, tmp_Form);
            foreach (KeyValuePair<string, string> tmp_Header in tmp_Headers)
            {
                tmp_WebRequestSender.SetRequestHeader(tmp_Header.Key, tmp_Header.Value);
            }

            tmp_WebRequestSender.timeout = 60;
            await tmp_WebRequestSender.SendWebRequest().ToUniTask(progress);
            tmp_Response = tmp_WebRequestSender.downloadHandler.text;
            if (!string.IsNullOrEmpty(tmp_Response) && !tmp_Response.Contains("Error")) return tmp_Response;
            Debug.LogError(tmp_WebRequestSender.error);
            return null;
        }

        private async Task<string> GetNetworkQuery(string _url, string _projectId = "")
        {
            string tmp_Response = string.Empty;
            IDictionary<string, string> tmp_Headers = new Dictionary<string, string>();
            tmp_Headers.Add("Authorization",
                $"Token {FindObjectOfType<AppMain>().SDKConfiguration.dashboardConfig.token}");
            tmp_Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var tmp_WebRequestSender = UnityWebRequest.Get(new Uri($"{_url}&platform={GetCurPlatform()}"));
            foreach (KeyValuePair<string, string> tmp_Header in tmp_Headers)
            {
                tmp_WebRequestSender.SetRequestHeader(tmp_Header.Key, tmp_Header.Value);
            }

            await tmp_WebRequestSender.SendWebRequest();
            tmp_Response = tmp_WebRequestSender.downloadHandler.text;
            if (!string.IsNullOrEmpty(tmp_Response) && !tmp_Response.Contains("Error")) return tmp_Response;
            Debug.LogError(tmp_Response);
            return null;
        }

        private string GetCurPlatform()
        {
#if UNITY_IOS
            return "iOS";
#elif UNITY_ANDROID
            return "Android";
#endif
        }

        /// <summary>
        /// Unity Event function
        /// </summary>
        private void Start()
        {
            if (initialized) return;
            DetailPopWindow.MaskButton.onClick.AddListener(() =>
            {
                //Hide Popwindow
                DetailPopWindow.PopWindow(false);

                //Remove try play ar experience button event
                DetailPopWindow.ARExperienceButton.onClick.RemoveAllListeners();
            });

            //CreateRecommendList();
            CreateShowcaseList();

            initialized = true;
        }

        /// <summary>
        /// Generate the recommend showcase list
        /// </summary>
        private async void CreateRecommendList()
        {
            var tmp_Url =
                $"{ConstKey.CONST_BASE_GATEWAY_KEY}{ConstKey.CONST_GET_RECOMMEND_SHOWCASE_KEY}?app_package_id={Application.identifier}&page_num=1&page_size=20";
            var tmp_Response = await GetNetworkQuery(tmp_Url);
            if (string.IsNullOrEmpty(tmp_Response))
            {
                Debug.LogError("Query empty!");
                return;
            }

            recommendXRProjectMapper = JsonUtility.FromJson<RecommendXRProjectMapper>(tmp_Response);
            foreach (RecommendXRProjectData tmp_ShowcaseData in recommendXRProjectMapper.data.all_project)
            {
                //Create recommend element
                var tmp_RecommendCloneGO = Instantiate(RecommendUIPrefab, RecommendListHolder);
                var tmp_RecommendTrans = tmp_RecommendCloneGO.transform;
                tmp_RecommendTrans.localScale = Vector3.one;
                tmp_RecommendTrans.localPosition = Vector3.zero;
                tmp_RecommendTrans.localRotation = Quaternion.identity;

                //Full in data for each recommend elements
                var tmp_RecommendElement = tmp_RecommendCloneGO.GetComponent<RecommendElement>();
                tmp_RecommendElement.ShowcaseId = tmp_ShowcaseData.project_uid.ToString();
                tmp_RecommendElement.RecommendBriefText.text = tmp_ShowcaseData.project_brief;
                tmp_RecommendElement.RecommendTitleText.text = tmp_ShowcaseData.project_name;
                //Accessing and download to image from web url,It maybe return a `Sprite`
                StartCoroutine(Utility.TryAcquireSpriteFromUri(new Uri(tmp_ShowcaseData.project_header),
                    tmp_RecommendElement.RecommendHeaderImage));
                tmp_RecommendElement.ClickButton.onClick.AddListener(() =>
                {
                    FixFreezeParameter(tmp_RecommendElement.ShowcaseId);
                });
            }
        }


        /// <summary>
        /// Generate the showcase list 
        /// </summary>
        private async void CreateShowcaseList()
        {
            var tmp_GetXRExperienceListUrl =
                Path.Combine(ConstKey.CONST_BASE_GATEWAY_KEY, ConstKey.CONST_GET_SHOWCASE_KEY);
            var tmp_Response =
                await GetNetworkQuery(
                    $"{tmp_GetXRExperienceListUrl}?app_package_id={Application.identifier}&page_num=1&page_size=20");
            if (string.IsNullOrEmpty(tmp_Response))
            {
                Debug.LogError("Query empty!");
                return;
            }

            xrProjectMapper = JsonUtility.FromJson<XRProjectMapper>(tmp_Response);
            foreach (XRProject tmp_ShowcaseData in xrProjectMapper.data.all_project)
            {
                //Create all showcase elements
                var tmp_ARExperienceSingleItem = Instantiate(ARExperienceListItemPrefab, ARExperienceListHolder);
                var tmp_ARExperienceSingleItemTrans = tmp_ARExperienceSingleItem.transform;
                tmp_ARExperienceSingleItemTrans.localScale = Vector3.one;
                tmp_ARExperienceSingleItemTrans.localPosition = Vector3.zero;
                tmp_ARExperienceSingleItemTrans.localRotation = Quaternion.identity;

                //Full in data for each showcase elements
                var tmp_ARExperienceSingleItemElement = tmp_ARExperienceSingleItem.GetComponent<ShowcaseElement>();
                tmp_ARExperienceSingleItemElement.ProjectId = tmp_ShowcaseData.project_uid;
                tmp_ARExperienceSingleItemElement.ShowcaseBriefText.text = tmp_ShowcaseData.project_brief;
                tmp_ARExperienceSingleItemElement.ShowcaseTitleText.text = tmp_ShowcaseData.project_name;
                //Accessing and download to image from web url,It maybe return a `Sprite`
                StartCoroutine(Utility.TryAcquireSpriteFromUri(new Uri(tmp_ShowcaseData.project_icon),
                    tmp_ARExperienceSingleItemElement.ShowcaseIconImage));

                //Register click event for every ar experience element 
                tmp_ARExperienceSingleItemElement.ClickedEvent = _showcaseId =>
                {
                    FixFreezeParameter(tmp_ARExperienceSingleItemElement.ProjectId);
                };
            }
        }


        /// <summary>
        /// Find the corresponding showcase data through showcase id.
        /// </summary>
        /// <param name="_projectId">Unique id of showcase</param>
        /// <returns>Showcase Data</returns>
        private XRProject FindShowcaseDataByShowcaseId(string _projectId)
        {
            return xrProjectMapper.data.all_project.Find((_showcase =>
                String.Compare(_showcase.project_uid, _projectId, StringComparison.Ordinal) == 0));
        }

        /// <summary>
        /// Get the AR experience by showcase id and launch
        /// </summary>
        /// <param name="_showcaseId">showcase id</param>
        private async void QueryARProjectByShowcaseId(string _showcaseId)
        {
            //1. Get ar showcase data
            var tmp_ShowcaseId = _showcaseId;
            var tmp_Showcase = FindShowcaseDataByShowcaseId(tmp_ShowcaseId);

            //2. Show pop window
            DetailPopWindow.gameObject.SetActive(true);
            DetailPopWindow.PopWindow(true);
            var tmp_Url =
                $"{ConstKey.CONST_BASE_GATEWAY_KEY}{ConstKey.CONST_GET_SHOWCASE_DETAIL_KEY}?project_uid={tmp_Showcase.project_uid}";
            var tmp_QueryData = await GetNetworkQuery(tmp_Url);

            var tmp_ShowcaseDetailData = JsonUtility.FromJson<XRProjectDetail>(tmp_QueryData).data;

            //3. Full in data to pop window
            DetailPopWindow.ARExperienceDescriptionText.text = tmp_ShowcaseDetailData.project_description;
            DetailPopWindow.ARExperienceTitleText.text = tmp_ShowcaseDetailData.project_name;
            StartCoroutine(Utility.TryAcquireSpriteFromUri(new Uri(tmp_ShowcaseDetailData.project_header),
                DetailPopWindow.ARExperienceHeaderImage));
            StartCoroutine(Utility.TryAcquireSpriteFromUri(new Uri(tmp_ShowcaseDetailData.project_icon),
                DetailPopWindow.ARExperienceIconImage));

            //4. Add play ar event for button
            DetailPopWindow.ARExperienceButton.onClick.AddListener(() =>
            {
                //Clean up all events
                DetailPopWindow.ARExperienceButton.onClick.RemoveAllListeners();

                //Find and accessing the `AppMain` script 
                var tmp_ARPPMainScript = FindObjectOfType<AppMain>();

                //Switch the user interface to AR mode
                tmp_ARPPMainScript.Home.SetActive(false);
                tmp_ARPPMainScript.ARView.SetActive(true);
                tmp_ARPPMainScript.Background.alpha = 0;
                tmp_ARPPMainScript.Background.gameObject.SetActive(false);

                //Hide the Detail pop window
                DetailPopWindow.PopWindow(false);

                //Start AR
                Utility.LaunchAR(FindObjectOfType<AppMain>().SDKConfiguration,
                    tmp_ShowcaseDetailData.project_uid);

                //Register event for AR user interface, Disable the AR and exit.
                var tmp_CloseBtnGO = tmp_ARPPMainScript.ARView.transform.Find("Close");
                tmp_CloseBtnGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    //Disabled the AR
                    Utility.DisableAR();

                    //We need reset user interface to Home from AR-View when disabled the AR
                    tmp_ARPPMainScript.Home.SetActive(true);
                    tmp_ARPPMainScript.ARView.SetActive(false);
                    tmp_ARPPMainScript.Background.alpha = 1;
                    tmp_ARPPMainScript.Background.gameObject.SetActive(true);
                });
            });
        }

        private void FixFreezeParameter(string _showcaseId)
        {
            QueryARProjectByShowcaseId(_showcaseId);
        }
    }
}