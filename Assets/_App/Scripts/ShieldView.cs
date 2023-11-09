using Michsky.MUIP;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class ShieldView : MonoBehaviourPunCallbacks
    {
        [SerializeField] private ProgressBar m_LocalProgressBar;
        [SerializeField] private ProgressBar m_RemoteProgressBar;

        private ProgressBar m_ActiveProgressBar;

        private void SetActiveProgressBar()
        {
            if (photonView.IsMine)
            {
                m_LocalProgressBar.gameObject.SetActive(true);
                m_RemoteProgressBar.gameObject.SetActive(false);

                m_ActiveProgressBar = m_LocalProgressBar;
            }
            else
            {
                m_LocalProgressBar.gameObject.SetActive(false);
                m_RemoteProgressBar.gameObject.SetActive(true);
                
                m_ActiveProgressBar = m_RemoteProgressBar;
            }
        }
        
        private void Awake()
        {
            if (m_ActiveProgressBar == null)
            {
                SetActiveProgressBar();
            }
        }

        private void Update()
        {
            /*
            if (!photonView.IsMine && Camera.main != null)
            {
                transform.LookAt(Camera.main.transform);
            }
            */
        }

        public void SetDefenceValue(float value)
        {
            if (value == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
            
            //photonView.RPC(nameof(RpcSetDefenceValue), RpcTarget.AllBuffered, value);
            photonView.RPC(nameof(RpcSetDefenceValue), RpcTarget.All, value);
        }

        [PunRPC]
        public void RpcSetDefenceValue(float value)
        {
            if (m_ActiveProgressBar == null)
            {
                SetActiveProgressBar();
            }

            if (m_ActiveProgressBar == null)
            {
                return;
            }
            
            m_ActiveProgressBar.gameObject.SetActive(value >= 0);

            m_ActiveProgressBar.currentPercent = value;
            m_ActiveProgressBar.UpdateUI();
            
        }
    }
}