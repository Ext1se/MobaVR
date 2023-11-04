using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MobaVR
{
    public class PlayerSpawner : BasePlayerSpawner<PlayerVR>
    {
        [SerializeField] private InputVR m_InputVR;
        [SerializeField] private PlayerVR m_PlayerPrefab;
        public GameObject localPlayer;
        public ManagerDevice managerDevice; // Ссылка на объект ManagerDevice
        public GameObject EventSystemVR; //евент систем из других сцен

        private void Awake()
        {
            managerDevice = FindObjectOfType<ManagerDevice>();
        }

        public override PlayerVR SpawnPlayer(Team team)
        {
            string prefabName = $"Players/{m_PlayerPrefab.name}";

            if (managerDevice.CanCreatePlayer) // Проверяем, нужно ли создавать игрока
            {
                EventSystemVR.SetActive(true);
                localPlayer = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);

                localPlayer.name += "_" + Random.Range(1, 1000);

                if (localPlayer.TryGetComponent(out PlayerVR playerVR))
                {
                    playerVR.SetLocalPlayer(m_InputVR);
                    playerVR.InitPlayer();
                    //TODO: need team??
                    //playerVR.SetTeamAndSync(team);

                    return playerVR;
                }
            }
            else
            {
                EventSystemVR.SetActive(false);
            }

            return null;
        }
    }
}