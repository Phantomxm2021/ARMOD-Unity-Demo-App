using System.Collections.Generic;

namespace UnityARMODApp.Runtime
{
    [System.Serializable]
    public class XRProjectMapper
    {
        public int code;
        public string msg;
        public AllXRProjects data;
    }

    [System.Serializable]
    public class AllXRProjects
    {
        public List<XRProject> all_project;
    }

    [System.Serializable]
    public class XRProject
    {
        public int app_uid;
        public int user_uid;
        public string project_uid;
        public string project_name;
        public string project_brief;
        public string project_icon;
    }
    
    [System.Serializable]
    public class XRProjectDetail
    {
        public int code;
        public string msg;
        public XRProjectDetailData data;
    }
    
    [System.Serializable]
    public class XRProjectDetailData
    {
        public int app_uid;
        public int user_uid;
        public string project_uid;
        public string project_name;
        public string project_icon;
        public string project_header;
        public string project_description;
        public string project_previews;
    }
}