using System.Collections.Generic;

namespace UnityARMODApp.Runtime
{
    [System.Serializable]
    public class ProjectsMapper
    {
        public int code;
        public string msg;
        public List<Project> data;
    }

    [System.Serializable]
    public class Project
    {
        public int app_uid;
        public int user_uid;
        public string project_id;
        public string project_name;
        public string project_brief;
        public string project_icon;
    }

    [System.Serializable]
    public class ProjectDetail
    {
        public int code;
        public string msg;
        public ProjectDetailData data;
    }

    [System.Serializable]
    public class ProjectDetailData
    {
        public int app_uid;
        public int user_uid;
        public string project_id;
        public string project_name;
        public string project_brief;
        public string project_icon;
        public string project_header;
        public string project_description;
    }
}