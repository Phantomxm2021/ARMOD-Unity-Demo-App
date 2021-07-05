using System.Collections.Generic;

namespace UnityARMODApp.Runtime
{
    [System.Serializable]
    public class ARExperienceShowcasesMapper
    {
        public int code;
        public string msg;
        public List<Showcase> data;
    }

    [System.Serializable]
    public class Showcase
    {
        public int app_uid;
        public int user_uid;
        public string arexperience_uid;
        public string showcase_uid;
        public string showcase_name;
        public string showcase_brief;
        public string showcase_icon;
        public List<string> showcase_not_index_tags;
    }
    
    [System.Serializable]
    public class ShowcaseDetail
    {
        public int code;
        public string msg;
        public ShowcaseDetailData data;
    }
    
    [System.Serializable]
    public class ShowcaseDetailData
    {
        public int app_uid;
        public int user_uid;
        public string arexperience_uid;
        public string showcase_uid;
        public string showcase_name;
        public string showcase_brief;
        public string showcase_icon;
        public string showcase_header;
        public string showcase_description;
        public string android_size;
        public string ios_size;
    }
}