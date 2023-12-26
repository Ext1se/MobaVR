using Photon.Pun;
using TMPro;
using UnityEngine;

namespace MobaVR
{
    public abstract class BaseView : MonoBehaviour, IViewVisibility
    {
        public abstract void Show();
        public abstract void Hide();
    }
}