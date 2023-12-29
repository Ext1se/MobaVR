using UnityEngine;

namespace MobaVR
{
    public class InfoView : BaseView
    {
        public override void Show()
        {
            if (gameObject != null)
            {
                gameObject.SetActive(true);
            }
        }

        public override void Hide()
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}