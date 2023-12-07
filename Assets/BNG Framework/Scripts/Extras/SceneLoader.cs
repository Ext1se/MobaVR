using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BNG {
    public class SceneLoader : MonoBehaviour {

        [Tooltip("Метод загрузки сцены 'LoadSceneMode' (в большинстве случаев должен быть одиночным). ")]
        public LoadSceneMode loadSceneMode = LoadSceneMode.Single;

        [Tooltip("Если значение true, компонент Screen Fader сделает экран черным перед загрузкой уровня.")]
        public bool UseSceenFader = true;

        [Tooltip("Подождите столько времени в секундах, прежде чем пытаться загрузить сцену. Полезно, если вам нужно погасить экран перед попыткой загрузить уровень.")]
        public float ScreenFadeTime = 0.5f;

        
        private LocalRepository localRepository;
        
        
        private void Awake()
        {
            localRepository = new LocalRepository();
            
        }
        
        ScreenFader sf;

        private string _loadSceneName = string.Empty;

        public void LoadLocalServerScene(string SceneName)
        {
            Debug.Log("Запускаем локальный, через IP ");
            localRepository.SetLocalServer(true);
            LoadScene(SceneName);
        }

        public void LoadRemoteServerScene(string SceneName)
        {
            Debug.Log("Запускаем онлайн");
            localRepository.SetLocalServer(false);
            LoadScene(SceneName);
        }
        
        [ContextMenu("Load Lobby")]
        public void LoadRemoteLobby()
        {
            Debug.Log("Запускаем онлайн");
            localRepository.SetLocalServer(false);
            LoadScene("Lobby");
        }
        
        private void Update(){
            /*
            if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.LeftAlt))
            {
                LoadRemoteServerScene("Lobby");
            }

            if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftAlt))
            {
                LoadLocalServerScene("Lobby");
            }
            */
        }

        public void LoadScene(string SceneName) {

            _loadSceneName = SceneName;

            if (UseSceenFader) {
                StartCoroutine("FadeThenLoadScene");
            }
            else {
                SceneManager.LoadScene(_loadSceneName, loadSceneMode);
            }
        }

        public IEnumerator FadeThenLoadScene() {

            if (UseSceenFader) {
                if (sf == null) {
                    sf = FindObjectOfType<ScreenFader>();
                    // May not have found anything
                    if (sf != null) {
                        sf.DoFadeIn();
                    }
                }
            }

            if(ScreenFadeTime > 0) {
                yield return new WaitForSeconds(ScreenFadeTime);
            }

            SceneManager.LoadScene(_loadSceneName, loadSceneMode);
        }
    }
    
}

