using System;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MobaVR
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkDamageable : Damageable
    {
        [SerializeField] private bool m_IsValidateSourceDamage = false;
        [SerializeField] private int m_DeltaMs = 100;
        [SerializeField] [ReadOnly] private List<HitDataTime> m_Hits = new();

        private PhotonView m_PhotonView;

        public UnityEvent<HitData> OnHit;
        public UnityEvent OnDie;
        public UnityEvent OnReborn;

        private void Awake()
        {
            TryGetComponent(out m_PhotonView);
        }

        private void Clear()
        {
            DateTime dateTimeNow = DateTime.Now;
            m_Hits.RemoveAll(matchHitData =>
            {
                if (matchHitData.HitData == null)
                {
                    return true;
                }

                TimeSpan delta = dateTimeNow - matchHitData.DateTime;
                return delta.Milliseconds > m_DeltaMs;
            });
        }

        private bool IsValidateHitData(HitData hitData)
        {
            Clear();

            if (hitData == null)
            {
                return false;
            }

            HitDataTime hitDataTime = null;

            if (m_Hits.Count > 0)
            {
                hitDataTime = m_Hits.Find(matchHitData =>
                {
                    return matchHitData.HitData != null &&
                           matchHitData.HitData.PhotonView != null &&
                           hitData.PhotonView != null &&
                           matchHitData.HitData.PhotonView == hitData.PhotonView;
                });
            }

            if (hitDataTime == null)
            {
                HitDataTime newHitDataTime = new HitDataTime();
                newHitDataTime.HitData = hitData;
                newHitDataTime.DateTime = DateTime.Now;
                m_Hits.Add(newHitDataTime);

                return true;
            }

            DateTime dateTimeNow = DateTime.Now;
            TimeSpan delta = dateTimeNow - hitDataTime.DateTime;
            if (delta.Milliseconds > m_DeltaMs)
            {
                hitDataTime.HitData = hitData;
                hitDataTime.DateTime = dateTimeNow;
                return true;
            }

            return false;
        }

        public override void Hit(HitData hitData)
        {
            if (m_IsValidateSourceDamage && !IsValidateHitData(hitData))
            {
                return;
            }

            if (m_PhotonView != null)
            {
                //TODO: AllBuffered hit damage
                //m_PhotonView.RPC(nameof(RpcHit), RpcTarget.AllBuffered, hitData);
                m_PhotonView.RPC(nameof(RpcHit), RpcTarget.All, hitData);
            }
        }

        [PunRPC]
        protected virtual void RpcHit(HitData hitData)
        {
            OnHit?.Invoke(hitData);
        }

        public override void Die()
        {
            if (m_PhotonView != null)
            {
                //TODO: AllBuffered hit damage
                //m_PhotonView.RPC(nameof(RpcDie), RpcTarget.AllBuffered);
                m_PhotonView.RPC(nameof(RpcDie), RpcTarget.All);
            }
        }

        [PunRPC]
        protected virtual void RpcDie()
        {
            OnDie?.Invoke();
        }

        public override void Reborn()
        {
            if (m_PhotonView != null)
            {
                //TODO: AllBuffered hit damage
                //m_PhotonView.RPC(nameof(RpcReborn), RpcTarget.AllBuffered);
                m_PhotonView.RPC(nameof(RpcReborn), RpcTarget.All);
            }
        }

        [PunRPC]
        protected void RpcReborn()
        {
            OnReborn?.Invoke();
        }

        #region Debug

        [ContextMenu("Hit")]
        private void Hit_Debug()
        {
            HitData hitData = new HitData()
            {
                Action = HitActionType.Damage,
                //Player = PhotonNetwork.LocalPlayer,
                PhotonOwner = m_PhotonView,
                Amount = 50f,
            };

            Hit(hitData);
        }
        
        [ContextMenu("HitRandom")]
        private void HitRandom_Debug()
        {
            HitData hitData = new HitData()
            {
                Action = HitActionType.Damage,
                //Player = PhotonNetwork.LocalPlayer,
                PhotonOwner = m_PhotonView,
                Amount = Random.Range(1, 60),
            };

            Hit(hitData);
        }

        #endregion
    }
}