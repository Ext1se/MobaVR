using Photon.Pun;
using TMPro;
using UnityEngine;

namespace MobaVR
{
    public class TeamScoreView : BaseTeamScoreView
    {
        [SerializeField] private TextMeshPro m_ScoreText;
        [SerializeField] private PhotonView m_PhotonView;

        public override void SetScore(int score)
        {
            m_ScoreText.text = score.ToString();
            
            if (PhotonNetwork.IsMasterClient)
            {
                //m_PhotonView.RPC(nameof(RpcSetScore), RpcTarget.AllBuffered, score);
                m_PhotonView.RPC(nameof(RpcSetScore), RpcTarget.All, score);
            }
        }

        [PunRPC]
        private void RpcSetScore(int score)
        {
            m_ScoreText.text = score.ToString();
        }

        public override void Show()
        {
            m_ScoreText.gameObject.SetActive(true);
            //m_ScoreText.enabled = true;
        }

        public override void Hide()
        {
            m_ScoreText.gameObject.SetActive(false);
            //m_ScoreText.enabled = false;
        }
    }
}