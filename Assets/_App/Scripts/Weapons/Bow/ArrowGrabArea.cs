using System;
using System.Collections;
using System.Collections.Generic;
using BNG;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace MobaVR.Weapons.Bow
{
    public class ArrowGrabArea : MonoBehaviour
    {
        private Bow m_Bow;
        private Collider m_Collider;
        private PhotonView m_PhotonView;

        private void OnDestroy()
        {
            if (m_Bow != null)
            {
                m_Bow.OnReleaseArrow.RemoveListener(OnReleaseArrow);
            }
        }

        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Bow = transform.parent.GetComponent<Bow>();
            m_PhotonView = transform.GetComponentInParent<PhotonView>();
        }

        private void Start()
        {
            //m_Bow.OnReleaseArrow.AddListener(OnReleaseArrow);
        }

        private void OnReleaseArrow(Arrow arrow, Vector3 direction)
        {
            //if (m_Bow.ClosestGrabber != null && grabObject != null && m_Bow.ClosestGrabber == grabObject)
            {
                m_Bow.CanGrabArrow = true;
                m_Bow.ClosestGrabber = null;

                StartCoroutine(Reset());
            }
        }

        private IEnumerator Reset()
        {
            m_Collider.enabled = false;
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.2f);
            m_Collider.enabled = true;
        }

        private void GrabArrow(Collider other, bool isStay)
        {
            Grabber grabber = other.GetComponent<Grabber>();
            if (grabber != null)
            {
                m_Bow.ClosestGrabber = grabber;
                if (!grabber.HoldingItem)
                {
                    m_Bow.CanGrabArrow = true;
                }
                else if (grabber.HoldingItem && grabber.HeldGrabbable != null)
                {
                    Arrow arrow = grabber.HeldGrabbable.GetComponent<Arrow>();
                    if (arrow != null && m_Bow.GrabbedArrow == null)
                    {
                        arrow.AttachBow(m_Bow);
                        m_Bow.GrabArrow(arrow);
                        
                        if (isStay)
                        {
                            arrow.transform.LookAt(m_Bow.getArrowRest());
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!m_PhotonView.IsMine)
            {
                return;
            }
            
            GrabArrow(other, false);
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!m_PhotonView.IsMine)
            {
                return;
            }
            
            GrabArrow(other, true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!m_PhotonView.IsMine)
            {
                return;
            }

            Grabber grabObject = other.GetComponent<Grabber>();
            if (m_Bow.ClosestGrabber != null && grabObject != null && m_Bow.ClosestGrabber == grabObject)
            {
                m_Bow.CanGrabArrow = false;
                m_Bow.ClosestGrabber = null;
            }
        }
    }
}