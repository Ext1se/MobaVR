using System;
using BNG;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class BowSpell : Spell
    {
        [SerializeField] private GameObject m_BowModel;
        [SerializeField] private Collider[] m_Colliders;
        [SerializeField] private Grabbable m_Grabbable;

        public Grabbable Grabbable => m_Grabbable;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_Colliders != null && m_Colliders.Length == 0)
            {
                m_Colliders = GetComponents<Collider>();
            }

            if (m_Grabbable == null)
            {
                m_Grabbable = GetComponent<Grabbable>();
            }
        }

        private void Awake()
        {
            m_BowModel.SetActive(false);
            RpcShow(false);
            if (!photonView.IsMine)
            {
                //m_Grabbable.enabled = false;
            }

            foreach (Collider bowCollider in m_Colliders)
            {
                //bowCollider.enabled = false;
            }
        }

        public void Show(bool isShow)
        {
            //photonView.RPC(nameof(RpcShow), RpcTarget.AllBuffered, isShow);
            photonView.RPC(nameof(RpcShow), RpcTarget.All, isShow);
        }

        [PunRPC]
        private void RpcShow(bool isShow)
        {
            m_Grabbable.enabled = isShow;
            m_BowModel.SetActive(isShow);
            foreach (Collider bowCollider in m_Colliders)
            {
                bowCollider.enabled = isShow;
            }
        }
    }
}