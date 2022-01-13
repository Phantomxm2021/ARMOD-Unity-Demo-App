using UnityEngine;
using UnityEngine.UI;

namespace UnityARMODApp.Runtime
{
    public class AlertWindow : MonoBehaviour
    {
        public Text BodyText;
        public Button Close;
        private Animator animator;
        private static readonly int SHOW = Animator.StringToHash("Show");

        private void Start()
        {
            animator = GetComponent<Animator>();
            Close.onClick.AddListener(Utility.DisableAR);
        }

        public void ShowAlertWindow(bool _show)
        {
            animator.SetBool(SHOW, _show);
        }
    }
}