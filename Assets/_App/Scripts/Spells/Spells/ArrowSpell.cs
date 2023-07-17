using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class ArrowSpell : MonoBehaviourPun
    {
        [SerializeField] private float m_Damage = 10f;
        private bool m_IsDamaged = false;

        public void Show(bool isShow)
        {
            photonView.RPC(nameof(RpcShow), RpcTarget.AllBuffered, isShow);
        }

        [PunRPC]
        private void RpcShow(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == transform || m_IsDamaged)
            {
                return;
            }

            if ((other.CompareTag("Player") || other.CompareTag("RemotePlayer"))
                && other.transform.TryGetComponent(out WizardPlayer wizardPlayer))
            {
                wizardPlayer.Hit(m_Damage);
                m_IsDamaged = true;
                Hide();
            }

            if (other.CompareTag("Enemy") && other.transform.TryGetComponent(out IHit iHit))
            {
                iHit.RpcHit(m_Damage);
                m_IsDamaged = true;
                Hide();
            }

            if (other.CompareTag("Item"))
            {
                Shield shield = other.GetComponentInParent<Shield>();
                if (shield != null)
                {
                    shield.Hit(1f);
                    m_IsDamaged = true;
                    Hide();
                }
            }
        }

        private void Hide()
        {
            if (PhotonNetwork.IsMasterClient && photonView != null)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}