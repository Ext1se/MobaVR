using UnityEngine;

namespace MobaVR
{
    public class SimpleView : BaseView
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