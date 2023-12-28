using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class PvpVictoryView : MonoBehaviourPun, IViewVisibility
    {
        public enum PvpVictoryType
        {
            WIN_RED,
            WIN_BLUE,
            DRAW
        }
        
        [SerializeField] private GameObject m_VictoryVfx;
        [SerializeField] private GameObject m_RedVictoryPanel;
        [SerializeField] private GameObject m_BlueVictoryPanel;
        [SerializeField] private GameObject m_DrawPanel;

        private void Awake()
        {
            HideAll();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void HideAll()
        {
            m_VictoryVfx.SetActive(false);
            m_RedVictoryPanel.SetActive(false);
            m_BlueVictoryPanel.SetActive(false);
            m_DrawPanel.SetActive(false);
        }

        public void ShowRedVictory()
        {
            HideAll();
            m_VictoryVfx.SetActive(true);
            m_RedVictoryPanel.SetActive(true);
        }
        
        public void ShowBlueVictory()
        {
            HideAll();
            m_VictoryVfx.SetActive(true);
            m_BlueVictoryPanel.SetActive(true);
        }
        
        public void ShowDraw()
        {
            HideAll();
            m_VictoryVfx.SetActive(true);
            m_DrawPanel.SetActive(true);
        }

        public void SetVictory(PvpVictoryType victoryType)
        {
            photonView.RPC(nameof(RpcSetVictory), RpcTarget.All, victoryType);
        }
        
        [PunRPC]
        private void RpcSetVictory(PvpVictoryType victoryType)
        {
            switch (victoryType)
            {
                case PvpVictoryType.WIN_RED:
                    ShowRedVictory();
                    break;
                case PvpVictoryType.WIN_BLUE:
                    ShowBlueVictory();
                    break;
                case PvpVictoryType.DRAW:
                    ShowDraw();
                    break;
            }
        }
    }
}