using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class BannerDropScript : MonoBehaviourPunCallbacks
{
    public bool isSkipMovie = true;
    public Transform topPoint; // Верхняя точка, откуда начинается движение щита
    public Transform bottomPoint; // Нижняя точка, где заканчивается движение щита
    public GameObject shield; // Объект щита, который будет перемещаться
    public AudioClip chainSound; // Звук цепи

    public bool runZvuk; // Переменная для проверки, проигрывался ли звук

    private AudioSource audioSource; // Источник звука для проигрывания звуковых эффектов
    private string sceneToLoadNext; // Имя следующей сцены для загрузки


    // Массив объектов баннера
    [System.Serializable]
    public class SceneBannerObject
    {
        public string sceneName; // имя сцены
        public GameObject bannerObject; // объект баннера
    }

    public List<SceneBannerObject> sceneBanners = new List<SceneBannerObject>();


    private void Start()
    {
        CityManager.Instance.RegisterBannerDropScript(this); //регестрируем код в менеджере сцен, чтобы он находил его

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Если источник звука отсутствует, добавляем его
        }

        runZvuk = false; // По умолчанию звук не проигрывался
    }

    /*[PunRPC]
    public void LowerShield() // Опускание щита
    {
        if(runZvuk)
        {
            StartCoroutine(RaiseBanner()); // Если звук проигрывался, поднимаем баннер
        }
        else
        {
            StartCoroutine(LowerAndActivateBanner()); // Иначе опускаем щит и активируем баннер
        }
    }*/

    [PunRPC]
    public void LowerShieldBySceneName(string sceneName) // Опускание щита
    {
        sceneToLoadNext = sceneName;

        if (!isSkipMovie)
        {
            if (runZvuk)
            {
                StartCoroutine(RaiseBanner()); // Если звук проигрывался, поднимаем баннер
            }
            else
            {
                StartCoroutine(LowerAndActivateBanner()); // Иначе опускаем щит и активируем баннер
            }
        }
        else
        {
            StartCoroutine(LoadNewSceneWithDelay());
        }
    }

    private IEnumerator LowerAndActivateBanner() // Опускание щита и активация баннера
    {
        // Расчет дистанции и времени для опускания щита
        float journeyLength = Vector3.Distance(topPoint.position, bottomPoint.position);
        float startTime = Time.time;
        float journeyTime = 6.0f;

        audioSource.PlayOneShot(chainSound); // Проигрываем звук цепи

        while (true)
        {
            float distCovered = (Time.time - startTime) * (journeyLength / journeyTime);
            float fracJourney = distCovered / journeyLength;
            shield.transform.position = Vector3.Lerp(topPoint.position, bottomPoint.position, fracJourney);

            if (fracJourney >= 1) break; // Если щит достиг нижней точки, прерываем цикл

            yield return null;
        }

        ActivateBanner(); // Активируем баннер
    }

    private IEnumerator RaiseBanner() // Поднятие баннера
    {
        // Расчет дистанции и времени для поднятия баннера
        float journeyLength = Vector3.Distance(bottomPoint.position, topPoint.position);
        float startTime = Time.time;
        float journeyTime = 6.0f;
        audioSource.PlayOneShot(chainSound); // Проигрываем звук цепи
        while (true)
        {
            float distCovered = (Time.time - startTime) * (journeyLength / journeyTime);
            float fracJourney = distCovered / journeyLength;
            shield.transform.position = Vector3.Lerp(bottomPoint.position, topPoint.position, fracJourney);

            if (fracJourney >= 1) break; // Если баннер достиг верхней точки, прерываем цикл

            yield return null;
        }


        StartCoroutine(LoadNewSceneWithDelay());
    }

    //активируем баннер
    private void ActivateBanner()
    {
        // извлекаем имя сцены без города
        string baseName = ExtractBaseSceneName(sceneToLoadNext);
        GameObject targetBanner = null;

        foreach (SceneBannerObject sbo in sceneBanners)
        {
            if (sbo.sceneName == baseName)
            {
                targetBanner = sbo.bannerObject;
                break;
            }
        }

        if (targetBanner != null)
        {
            targetBanner.SetActive(true);
            AudioSource bannerAudio = targetBanner.GetComponent<AudioSource>();
            if (bannerAudio != null)
            {
                bannerAudio.Play();

                //float delay = isSkipMovie ? 2f : bannerAudio.clip.length;
                float delay = bannerAudio.clip.length;

                StartCoroutine(DeactivateBannerAfterSound(targetBanner, delay));
                StartCoroutine(LoadNewSceneAfterSound(delay));
            }
        }
    }

    private IEnumerator DeactivateBannerAfterSound(GameObject banner, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        banner.SetActive(false);
    }


    //извленкаем имя сцены без города
    private string ExtractBaseSceneName(string fullSceneName)
    {
        int underscoreIndex = fullSceneName.LastIndexOf('_');
        if (underscoreIndex >= 0)
        {
            return fullSceneName.Substring(0, underscoreIndex);
        }

        return fullSceneName;
    }


    private IEnumerator LoadNewSceneAfterSound(float waitTime) // Загрузка новой сцены после проигрывания звука
    {
        yield return new WaitForSeconds(waitTime);
        runZvuk = true; // Устанавливаем, что звук проигрался
        StartCoroutine(RaiseBanner()); // Если звук проигрывался, поднимаем баннер
        //TriggerLowerShield(); // Запускаем процесс опускания щита
    }


    private IEnumerator LoadNewSceneWithDelay()
    {
        yield return new WaitForSeconds(3f); // Задержка в 3 секунды

        if (!string.IsNullOrEmpty(sceneToLoadNext))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoadNext); // Загружаем сцену
        }
    }


    public void SetSceneToLoadNext(string sceneName) // Устанавливаем имя следующей сцены для загрузки
    {
        sceneToLoadNext = sceneName;
    }

    /*public void TriggerLowerShield() // Запуск опускания щита через Photon
    {
        photonView.RPC("LowerShield", RpcTarget.All);
    }*/

    public void TriggerLowerShield(string sceneName) // Запуск опускания щита через Photon
    {
        sceneToLoadNext = sceneName;
        photonView.RPC("LowerShieldBySceneName", RpcTarget.All, sceneName);
    }
}