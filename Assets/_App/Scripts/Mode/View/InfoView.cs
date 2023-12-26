using UnityEngine;

namespace MobaVR
{
    public class InfoView : BaseView
    {
        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}