using System.Collections.Generic;

namespace UnityARMODApp.Runtime
{
    [System.Serializable]
    public class RecommendXRProjectMapper
    {
        public int code;
        public string msg;
        public List<RecommendXRProjectData> data;
    }

    [System.Serializable]
    public class RecommendXRProjectData
    {
        public int app_uid;
        public int user_uid;
        public string project_id;
        //public string project_uid;
        public string project_name;
        public string project_brief;
        public string project_icon;
        public string project_header;
        public List<string> showcase_not_index_tags;
    }
}