using System.Collections.Generic;

namespace UnityARMODApp.Runtime
{
    [System.Serializable]
    public class RecommendShowcasesMapper
    {
        public int code;
        public string msg;
        public List<RecommendData> data;
    }

    [System.Serializable]
    public class RecommendData
    {
        public int app_uid;
        public int user_uid;
        public string project_id;
        public string project_name;
        public string project_brief;
        public string project_header;
    }
}