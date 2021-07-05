using System.Collections.Generic;

namespace UnityARMODApp.Runtime
{
    [System.Serializable]
    public class RecommendShowcasesMapper
    {
        public int code;
        public string msg;
        public List<RecommendShowcaseData> data;
    }

    [System.Serializable]
    public class RecommendShowcaseData
    {
        public int app_uid;
        public int user_uid;
        public string arexperience_uid;
        public string showcase_uid;
        public string showcase_name;
        public string showcase_brief;
        public string showcase_icon;
        public string showcase_header;
        public List<string> showcase_not_index_tags;
    }
}