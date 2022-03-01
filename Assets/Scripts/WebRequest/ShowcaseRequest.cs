using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using com.Phantoms.WebRequestExtension.Runtime.WebRequest;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnityARMODApp.Runtime
{
    public class ShowcaseRequest : MonoBehaviour
    {
        public GameObject RecommendUIPrefab;
        public GameObject ARExperienceListItemPrefab;


        public Transform RecommendListHolder;
        public Transform ARExperienceListHolder;
        public DetailPopWindow DetailPopWindow;
        private ProjectsMapper projectsMapper;
        private RecommendShowcasesMapper recommendShowcasesMapper;
        private List<ProjectElement> projects = new List<ProjectElement>();


        private bool initialized = false;
        private int page_num = 0;
        private int page_size = 10;

        /// <summary>
        /// Accessing the ARMOD Web Services API
        /// </summary>
        /// <param name="_url">ARMOD web services url</param>
        /// <param name="_showcaseId">current showcase id,It will be use to query the current showcase detail information(Option)</param>
        /// <returns></returns>
        private async Task<string> NetworkQuery(string _url, string _showcaseId = "")
        {
            string tmp_Response = string.Empty;
            IDictionary<string, string> tmp_Headers = new Dictionary<string, string>();
            tmp_Headers.Add("Authorization",
                $"Token {FindObjectOfType<AppMain>().SDKConfiguration.dashboardConfig.token}");
            tmp_Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            var tmp_Form = new WWWForm();
            tmp_Form.AddField("package_id", Application.identifier);
            tmp_Form.AddField("project_id", _showcaseId);
            var tmp_WebRequestSender = new WebRequestWithProgress(_timeout: 60);
            tmp_Response = await tmp_WebRequestSender.SendRequest(new Uri(_url),
                "POST", tmp_Headers, tmp_Form);

            if (!string.IsNullOrEmpty(tmp_Response) && !tmp_Response.Contains("Error")) return tmp_Response;
            Debug.LogError(tmp_Response);
            return null;
        }


        private void GetProjectList(string _url, int _page_num, int _page_size, Action<string> _callback)
        {
            var tmp_WebRequest = UnityWebRequest.Get($"{_url}?page_num={_page_num}&page_size={_page_size}");
            tmp_WebRequest.SetRequestHeader("Authorization",
                $"Token {FindObjectOfType<AppMain>().SDKConfiguration.dashboardConfig.token}");
            tmp_WebRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            tmp_WebRequest.SendWebRequest().completed += operation =>
            {
                switch (tmp_WebRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        _callback.Invoke(tmp_WebRequest.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.ProtocolError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(tmp_WebRequest.error);
                        break;
                }
            };
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

            CreateRecommendList();
            CreateShowcaseList();

            initialized = true;
        }

        /// <summary>
        /// Generate the recommend showcase list
        /// </summary>
        private async void CreateRecommendList()
        {
            var tmp_Response = await NetworkQuery(Path.Combine(ConstKey.CONST_BASE_GATEWAY_KEY,
                ConstKey.CONST_GET_RECOMMEND_SHOWCASE_KEY));
            if (string.IsNullOrEmpty(tmp_Response))
            {
                Debug.LogError("Query empty!");
                return;
            }

            recommendShowcasesMapper = JsonUtility.FromJson<RecommendShowcasesMapper>(tmp_Response);
            foreach (RecommendData tmp_RecommandData in recommendShowcasesMapper.data)
            {
                //Create recommend element
                var tmp_RecommendCloneGO = Instantiate(RecommendUIPrefab, RecommendListHolder);
                var tmp_RecommendTrans = tmp_RecommendCloneGO.transform;
                tmp_RecommendTrans.localScale = Vector3.one;
                tmp_RecommendTrans.localPosition = Vector3.zero;
                tmp_RecommendTrans.localRotation = Quaternion.identity;

                //Full in data for each recommend elements
                var tmp_RecommendElement = tmp_RecommendCloneGO.GetComponent<RecommendElement>();
                tmp_RecommendElement.ShowcaseId = tmp_RecommandData.project_id;
                tmp_RecommendElement.RecommendBriefText.text = tmp_RecommandData.project_brief;
                tmp_RecommendElement.RecommendTitleText.text = tmp_RecommandData.project_name;
                //Accessing and download to image from web url,It maybe return a `Sprite`
                StartCoroutine(Utility.TryAcquireSpriteFromUri(new Uri(tmp_RecommandData.project_header),
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
        private void CreateShowcaseList()
        {
            var tmp_URL = Path.Combine(ConstKey.CONST_BASE_GATEWAY_KEY,
                ConstKey.CONST_GET_SHOWCASE_KEY);
            int tmp_NextPage = page_num + 1;
            GetProjectList(tmp_URL, tmp_NextPage, page_size, _result =>
            {
                var tmp_Data = JsonUtility.FromJson<ProjectsMapper>(_result);
                if (projectsMapper == null)
                    projectsMapper = tmp_Data;
                else
                    projectsMapper.data.AddRange(tmp_Data.data);

                foreach (Project tmp_ShowcaseData in projectsMapper.data)
                {
                    if (projects.Find(_e => _e.ProjectId == tmp_ShowcaseData.project_id)) continue;

                    //Create all showcase elements
                    var tmp_ARExperienceSingleItem = Instantiate(ARExperienceListItemPrefab, ARExperienceListHolder);
                    var tmp_ARExperienceSingleItemTrans = tmp_ARExperienceSingleItem.transform;
                    tmp_ARExperienceSingleItemTrans.localScale = Vector3.one;
                    tmp_ARExperienceSingleItemTrans.localPosition = Vector3.zero;
                    tmp_ARExperienceSingleItemTrans.localRotation = Quaternion.identity;

                    //Full in data for each showcase elements
                    var tmp_ARExperienceSingleItemElement = tmp_ARExperienceSingleItem.GetComponent<ProjectElement>();
                    tmp_ARExperienceSingleItemElement.ProjectId = tmp_ShowcaseData.project_id;
                    tmp_ARExperienceSingleItemElement.ProjectBriefText.text = tmp_ShowcaseData.project_brief;
                    tmp_ARExperienceSingleItemElement.ProjectTitleText.text = tmp_ShowcaseData.project_name;
                    //Accessing and download to image from web url,It maybe return a `Sprite`
                    StartCoroutine(Utility.TryAcquireSpriteFromUri(new Uri(tmp_ShowcaseData.project_icon),
                        tmp_ARExperienceSingleItemElement.ProjectIconImage));

                    //Register click event for every ar experience element 
                    tmp_ARExperienceSingleItemElement.ClickedEvent = _showcaseId =>
                    {
                        FixFreezeParameter(tmp_ARExperienceSingleItemElement.ProjectId);
                    };
                }

                page_num = tmp_NextPage;
            });
        }


        /// <summary>
        /// Find the corresponding showcase data through showcase id.
        /// </summary>
        /// <param name="_showcaseId">Unique id of showcase</param>
        /// <returns>Showcase Data</returns>
        private Project FindShowcaseDataByShowcaseId(string _showcaseId)
        {
            return projectsMapper.data.Find((_showcase =>
                String.Compare(_showcase.project_id, _showcaseId, StringComparison.Ordinal) == 0));
        }

        /// <summary>
        /// Get the AR experience by showcase id and launch
        /// </summary>
        /// <param name="_projecetId">showcase id</param>
        private async void QueryARProjectByShowcaseId(string _projecetId)
        {
            //1. Get ar showcase data
            var tmp_ShowcaseId = _projecetId;
            var tmp_Showcase = FindShowcaseDataByShowcaseId(tmp_ShowcaseId);

            //2. Show pop window
            DetailPopWindow.gameObject.SetActive(true);
            DetailPopWindow.PopWindow(true);
            Debug.Log(Path.Combine(ConstKey.CONST_BASE_GATEWAY_KEY,
                ConstKey.CONST_GET_SHOWCASE_DETAIL_KEY));
            var tmp_QueryData = await NetworkQuery(Path.Combine(ConstKey.CONST_BASE_GATEWAY_KEY,
                ConstKey.CONST_GET_SHOWCASE_DETAIL_KEY), tmp_Showcase.project_id);
            var tmp_ProjectDetailData = JsonUtility.FromJson<ProjectDetail>(tmp_QueryData).data;

            //3. Full in data to pop window
            DetailPopWindow.ARExperienceDescriptionText.text = tmp_ProjectDetailData.project_description;
            DetailPopWindow.ARExperienceTitleText.text = tmp_ProjectDetailData.project_name;
            StartCoroutine(Utility.TryAcquireSpriteFromUri(new Uri(tmp_ProjectDetailData.project_header),
                DetailPopWindow.ARExperienceHeaderImage));
            StartCoroutine(Utility.TryAcquireSpriteFromUri(new Uri(tmp_ProjectDetailData.project_icon),
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
                    tmp_ProjectDetailData.project_id);

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