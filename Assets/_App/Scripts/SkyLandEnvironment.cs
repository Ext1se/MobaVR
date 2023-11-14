using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace MobaVR
{
    public class SkyLandEnvironment : MonoBehaviourPun
    {
        [SerializeField] private GameObject m_Environment;

        public UnityEvent OnShow;
        public UnityEvent OnHide;

        public void Show()
        {
            //photonView.RPC(nameof(RpcShow), RpcTarget.AllBuffered);
            photonView.RPC(nameof(RpcShow), RpcTarget.All);
        }
        
        public void Hide()
        {
            //photonView.RPC(nameof(RpcHide), RpcTarget.AllBuffered);
            photonView.RPC(nameof(RpcHide), RpcTarget.All);
        }
        
        [PunRPC]
        public void RpcShow()
        {
            OnShow?.Invoke();
            //m_Environment.SetActive(true);
        }

        [PunRPC]
        public void RpcHide()
        {
            OnHide?.Invoke();
            //m_Environment.SetActive(false);
        }
    }
}