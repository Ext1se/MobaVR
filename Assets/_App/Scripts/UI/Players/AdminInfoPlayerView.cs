using System;
using Michsky.MUIP;
using MobaVR.Utils;
using TMPro;
using UnityEngine;

namespace MobaVR
{
    public class AdminInfoPlayerView : BaseAdminPlayerView
    {
        [SerializeField] private TMP_InputField m_NickNameInput;
        [SerializeField] private CustomDropdown m_TeamDropdown;
        [SerializeField] private CustomDropdown m_RoleDropdown;

        public void ResetCalibration()
        {
            if (m_PlayerVR == null && m_PlayerVR.Calibration == null)
            {
                return;
            }

            m_PlayerVR.Calibration.ResetCalibration();
        }

        public void SetHeight()
        {
            if (m_PlayerVR == null && m_PlayerVR.Calibration == null)
            {
                return;
            }

            m_PlayerVR.Calibration.SetHeight();
        }

        public void UpdateNickName(string nickName)
        {
            if (m_PlayerVR == null)
            {
                return;
            }

            m_PlayerVR.SetNickName(nickName);
        }

        public void UpdateRole(string idRole)
        {
            if (m_PlayerVR == null)
            {
                return;
            }

            m_PlayerVR.ClassSwitcher.SetRole(idRole);
        }

        public void UpdateTeam(string teamName)
        {
            if (m_PlayerVR == null)
            {
                return;
            }

            TeamType teamType = teamName.Equals("red", StringComparison.OrdinalIgnoreCase)
                ? TeamType.RED
                : TeamType.BLUE;

            //m_PlayerVR.SetTeam(teamType);

            if (m_GameSession != null)
            {
                m_GameSession.SetTeam(teamType, m_PlayerVR);
            }
        }

        #region OnUpdate

        protected override void OnUpdateRole(string idRole)
        {
            int selectedItemIndex = 0;
            switch (idRole)
            {
                case (AppStrings.ID_ROLE_MALE_WIZARD):
                {
                    selectedItemIndex = 0;
                    break;
                }

                case (AppStrings.ID_ROLE_MALE_DEFENDER):
                {
                    selectedItemIndex = 1;
                    break;
                }

                case (AppStrings.ID_ROLE_MALE_ARCHER):
                {
                    selectedItemIndex = 2;
                    break;
                }
                
                case (AppStrings.ID_ROLE_FEMALE_WIZARD):
                {
                    selectedItemIndex = 3;
                    break;
                }

                case (AppStrings.ID_ROLE_FEMALE_WIZARD_1):
                {
                    selectedItemIndex = 4;
                    break;
                }

                case (AppStrings.ID_ROLE_FEMALE_DEFENDER):
                {
                    selectedItemIndex = 5;
                    break;
                }
                
                case (AppStrings.ID_ROLE_FEMALE_DEFENDER_1):
                {
                    selectedItemIndex = 6;
                    break;
                }

                case (AppStrings.ID_ROLE_FEMALE_ARCHER):
                {
                    selectedItemIndex = 7;
                    break;
                }

                case (AppStrings.ID_ROLE_FEMALE_ARCHER_1):
                {
                    selectedItemIndex = 8;
                    break;
                }
            }

            m_RoleDropdown.SetDropdownIndex(selectedItemIndex);
            m_RoleDropdown.UpdateItemLayout();
        }

        protected override void OnUpdateNickName(string nickName)
        {
            m_NickNameInput.text = m_PlayerVR.PlayerData.NickName;
        }

        protected override void OnUpdateTeam(TeamType teamType)
        {
            int selectedItemIndex = teamType == TeamType.BLUE ? 0 : 1;
            m_TeamDropdown.SetDropdownIndex(selectedItemIndex);
            m_TeamDropdown.UpdateItemLayout();
        }

        #endregion
    }
}