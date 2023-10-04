using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MobaVR
{
    public class Skin : TeamItem, ISkin
    {
        public string ID;

        [Header("Armature")]
        [SerializeField] private Transform m_Armature;
        [SerializeField] [ReadOnly] private float m_ArmatureScale = 0.54f;

        [Header("Hand")]
        [SerializeField] private bool m_UseSkinHands = true;
        [SerializeField] private GameObject m_LeftHandModel;
        [SerializeField] private GameObject m_RightHandModel;
        
        [Header("Team")]
        [SerializeField] private List<SkinItem> m_TeamRenderers = new();

        [Header("Renderers")]
        [SerializeField] private bool m_SetInactiveOnDie = true;
        [SerializeField] private List<Renderer> m_Renderers = new();
        [SerializeField] private List<Renderer> m_LegRenderers = new();
        [SerializeField] private List<Renderer> m_FaceRenderers = new();
        [SerializeField] private List<Renderer> m_BodyRenderers = new();
        [SerializeField] private List<Renderer> m_HiddenVrRenderers = new();
        [SerializeField] private List<Renderer> m_DieRenderers = new();

        [Space]
        [Header("Events")]
        public UnityEvent OnActivated;
        public UnityEvent OnDeactivated;
        public UnityEvent OnDie;

        private string[] m_LegNames = new[]
        {
            "leg",
            "shoe",
            "boot"
        };

        private string[] m_FaceNames = new[]
        {
            "face",
            "eye",
            "head",
            "hair",
            "neck",
            "teeth",
        };

        public Transform Armature => m_Armature;

        private void OnValidate()
        {
            //FindArmature();
            //FindTeamRenderers();
            //FindAllRenderers();
            //FindLegs();
            //FindFace();
        }

        public bool SetInactiveOnDie
        {
            get => m_SetInactiveOnDie;
            set => m_SetInactiveOnDie = value;
        }

        #region Find Renderers

        [ContextMenu("FindArmature")]
        private void FindArmature()
        {
            if (m_Armature == null)
            {
                m_Armature = transform.Find("Armature");
            }
        }

        [ContextMenu("FindAllRenderers")]
        private void FindAllRenderers()
        {
            if (m_Renderers.Count == 0)
            {
                m_Renderers.AddRange(GetComponentsInChildren<Renderer>(true));
            }
        }

        [ContextMenu("FindTeamRenderers")]
        private void FindTeamRenderers()
        {
            if (m_TeamRenderers.Count == 0)
            {
                m_TeamRenderers.AddRange(GetComponentsInChildren<SkinItem>(true));
            }
        }


        [ContextMenu("FindFace")]
        private void FindFace()
        {
            if (m_TeamRenderers.Count != 0 && m_FaceRenderers.Count == 0)
            {
                m_FaceRenderers.AddRange(m_Renderers.Where(meshRenderer =>
                {
                    foreach (string legName in m_FaceNames)
                    {
                        if (meshRenderer.name.Contains(legName, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    return false;
                }));
            }

            Transform face = transform.Find("Body/Base/Head");
            if (face != null)
            {
                m_FaceRenderers.AddRange(face.GetComponentsInChildren<Renderer>(true));
            }

            Transform customization = transform.Find("Customization");
            if (customization != null)
            {
                m_FaceRenderers.AddRange(customization.GetComponentsInChildren<Renderer>(true));
            }
        }

        [ContextMenu("FindLegs")]
        private void FindLegs()
        {
            if (m_TeamRenderers.Count != 0 && m_LegRenderers.Count == 0)
            {
                m_LegRenderers.AddRange(m_Renderers.Where(meshRenderer =>
                {
                    foreach (string legName in m_LegNames)
                    {
                        if (meshRenderer.name.Contains(legName, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    return false;
                }));
            }
        }

        #endregion

        #region Visibility

        [ContextMenu("ScaleArmature")]
        public void ScaleArmature()
        {
            if (m_Armature != null)
            {
                m_Armature.localScale = new Vector3(m_ArmatureScale, m_ArmatureScale, m_ArmatureScale);
            }
        }

        [ContextMenu("SetVisibilityLegs")]
        public void SetVisibilityLegs(bool isVisible = false)
        {
            foreach (Renderer meshRenderer in m_LegRenderers)
            {
                meshRenderer.gameObject.SetActive(isVisible);
            }
        }

        [ContextMenu("SetVisibilityFace")]
        public void SetVisibilityFace(bool isVisible = false)
        {
            foreach (Renderer meshRenderer in m_FaceRenderers)
            {
                meshRenderer.gameObject.SetActive(isVisible);
            }
        }

        [ContextMenu("SetVisibility")]
        public void SetVisibilityVR(bool isVisible = false)
        {
            foreach (Renderer meshRenderer in m_HiddenVrRenderers)
            {
                meshRenderer.gameObject.SetActive(isVisible);
            }
        }

        [ContextMenu("SetVisibilityBody")]
        public void SetVisibilityBody(bool isVisible = false)
        {
            foreach (Renderer meshRenderer in m_BodyRenderers)
            {
                meshRenderer.gameObject.SetActive(isVisible);
            }
        }
        
        [ContextMenu("SetVisibilityDie")]
        public void SetVisibilityDieRenderers(bool isVisible = false)
        {
            foreach (Renderer meshRenderer in m_DieRenderers)
            {
                meshRenderer.gameObject.SetActive(isVisible);
            }
        }

        #endregion

        #region Skin

        private void SetEnableHands(bool isEnable)
        {
            if (m_UseSkinHands)
            {
                if (m_LeftHandModel)
                {
                    m_LeftHandModel.gameObject.SetActive(isEnable);
                }

                if (m_RightHandModel)
                {
                    m_RightHandModel.gameObject.SetActive(isEnable);
                }
            }
        }

        public override void SetTeam(TeamType teamType)
        {
            base.SetTeam(teamType);

            foreach (SkinItem skinItem in m_TeamRenderers)
            {
                skinItem.SetTeam(teamType);
            }
        }

        public void ActivateSkin(TeamType teamType)
        {
            gameObject.SetActive(true);
            SetEnableHands(true);

            SetTeam(teamType);

            OnActivated?.Invoke();
        }

        public void DeactivateSkin()
        {
            gameObject.SetActive(false);
            SetEnableHands(false);

            OnDeactivated?.Invoke();
        }
        
        public void SetDieSkin()
        {
            if (m_SetInactiveOnDie)
            {
                gameObject.SetActive(false);
            }
            
            // TODO: check hands on Die
            SetEnableHands(false);

            OnDie?.Invoke();
        }
        
        #endregion
    }
}