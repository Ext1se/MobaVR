using Michsky.MUIP;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace MobaVR
{
    public class PlayerView : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI m_Nickname;
        [SerializeField] private bool m_IsShowHealthOnLocalPlayer = false;
        [SerializeField] private ProgressBar m_HealthProgressBar;
        [SerializeField] private bool m_IsShowManaOnLocalPlayer = false;
        [SerializeField] private ProgressBar m_ManaProgressBar;
        [SerializeField] private bool m_IsShowVoiceOnLocalPlayer = false;
        [SerializeField] private ProgressBar m_VoiceProgressBar;
        [SerializeField] private Transform m_PointPlayer;

        [SerializeField] private PlayerVR playerVR;
       
        private bool _Die;

        private void Awake()
        {
           
            SetHealth(100f);
            _Die=false;
        }

        private void Start()
        {
            if (photonView.IsMine)
            {
                if (m_Nickname != null)
                {
                    m_Nickname.gameObject.SetActive(false);
                }
                
                if (m_PointPlayer == null)
                {
                    GameObject pointPlayer = GameObject.Find("PlayerViewPoint");
                    if (pointPlayer != null)
                    {
                        m_PointPlayer = pointPlayer.transform;
                    }
                }
                
                if (m_HealthProgressBar != null)
                {
                    m_HealthProgressBar.gameObject.SetActive(m_IsShowHealthOnLocalPlayer);
                }
                
                if (m_ManaProgressBar != null)
                {
                    m_ManaProgressBar.gameObject.SetActive(m_IsShowManaOnLocalPlayer);
                }
                
                if (m_VoiceProgressBar != null)
                {
                    m_VoiceProgressBar.gameObject.SetActive(m_IsShowVoiceOnLocalPlayer);
                }

                if (m_PointPlayer == null)
                {
                    return;
                }
                
                Vector3 scale = transform.localScale;
                transform.parent = m_PointPlayer;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = scale;
            }
            else
            {
                if (Camera.main != null)
                {
                    transform.LookAt(Camera.main.transform);
                }

                /*
                if (m_HealthProgressBar != null)
                {
                    m_HealthProgressBar.gameObject.SetActive(false);
                }
                */
                
                if (m_ManaProgressBar != null)
                {
                    m_ManaProgressBar.gameObject.SetActive(false);
                }
                
                if (m_VoiceProgressBar != null)
                {
                    m_VoiceProgressBar.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (!photonView.IsMine && Camera.main != null)
            {
                transform.LookAt(Camera.main.transform);
            }
        }

        public void RpcSetHealth(float value)
        {


            if (value < 0)
            {
                _Die = true;
                value = 0f;
                if (_Die == true)
                {
                    /*
                    GameObject inputVRObject = GameObject.Find("InputVR");

                    if (inputVRObject != null)
                    {
                        LocalVR localVR = inputVRObject.GetComponent<LocalVR>();

                        if (localVR != null)
                        {
                            //������ ���������� ���� � ��������� ������
                            localVR.DieLocal();
                        }
                    }
                    */

                    //������ ���������� ���� � ������� ������
                    if (playerVR != null)
                    {
                        //photonView.RPC(nameof(SetDie), RpcTarget.AllBuffered);
                    }
                }
            }

            //������ ����� �����
            //photonView.RPC(nameof(SetHealth), RpcTarget.AllBuffered, value);
            photonView.RPC(nameof(SetHealth), RpcTarget.All, value);
        }

        [PunRPC]
        public void SetHealth(float value)
        {
            //float percent = value / m_ProgressBar.maxValue * 100;
            //m_ProgressBar.currentPercent = percent;
            m_HealthProgressBar.currentPercent = value;
            m_HealthProgressBar.UpdateUI();
        }
        
        [PunRPC]
        public void SetMaxHealth(float value)
        {
            m_HealthProgressBar.valueLimit = value;
            m_HealthProgressBar.maxValue = value;
            m_HealthProgressBar.currentPercent = value;
            m_HealthProgressBar.UpdateUI();
        }


        [PunRPC]
        public void SetDie()
        {

            playerVR.PlayerDie();
        }





    }
}