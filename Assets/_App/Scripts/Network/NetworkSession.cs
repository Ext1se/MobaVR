using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MetaConference
{
    public class NetworkSession : MonoBehaviourPunCallbacks
    {
        [SerializeField] public string m_MenuScene = "Menu";
        
        private Player m_Player;

        public Player Player => m_Player;

        [Space]
        public UnityEvent OnDisconnectPun;
        public UnityEvent OnConnectPun;
        public UnityEvent OnJoinRoomPun;
        public UnityEvent OnLeftRoomPun;
        public UnityEvent OnConnectToMasterPun;

        #region MonoBehaviour

        private void Awake()
        {
            /*
            if (!PhotonNetwork.IsConnected)
            {
                LoadLauncherScene();
            }
            */
        }

        #endregion

        private void WaitAndBackToMenu()
        {
            SceneManager.LoadScene(m_MenuScene);
        }

        private void LoadLauncherScene()
        {
            //SceneManager.LoadScene(0);
            SceneManager.LoadScene(m_MenuScene);
            //Invoke(nameof(WaitAndBackToMenu), 4f);
        }

        public void BackToLaunch()
        {
            PhotonNetwork.Disconnect();
        }

        public void JoinRoom(string nameRoom)
        {
            //PhotonNetwork.LeaveRoom();
        }

        #region PUN

        public override void OnConnected()
        {
            base.OnConnected();
            OnConnectPun?.Invoke();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log($"{nameof(NetworkSession)}: OnConnectedToMaster");

            OnConnectToMasterPun?.Invoke();
            //PhotonNetwork.JoinRandomRoom();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            Debug.Log($"{nameof(NetworkSession)}: OnMasterClientSwitched: {newMasterClient.NickName}");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log($"{nameof(NetworkSession)}: OnPlayerLeftRoom: {otherPlayer.NickName}");
            OnLeftRoomPun?.Invoke();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.Log($"{nameof(NetworkSession)}: OnDisconnected: {cause}");

            OnDisconnectPun?.Invoke();
            LoadLauncherScene();
            //m_AvatarManager.DisconnectPlayer(m_MineAvatar);
            //PhotonNetwork.RejoinRoom("NewRoom");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log($"{nameof(NetworkSession)}: OnCreateRoomFailed: {message}");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.Log($"{nameof(NetworkSession)}: OnJoinRoomFailed: {message}");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            Debug.Log($"{nameof(NetworkSession)}: OnJoinRandomFailed: {message}");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log($"{nameof(NetworkSession)}: OnJoinedRoom");

            OnJoinRoomPun?.Invoke();
        }

        #endregion
    }
}