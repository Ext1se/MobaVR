using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.SceneManagement;

//Этот скрипт, при смене цены выключает вибрацию контроллеров, ставит все HUD на место, ставит все руки на метсо и включает нужные.
public class RessetObuchHansHUDVibr : MonoBehaviour
{
    
    public Grabber grabLeft;//рука, которая будет вибрировать (вставлять в неё граббер)
    public Grabber grabRight;//рука, которая будет вибрировать (вставлять в неё граббер)

    public HandAnimationController HandAnimationControllerLeft;//через этот скрипт могу управлять анимациями и вибрациями рук
    public HandAnimationController HandAnimationControlRight;//через этот скрипт могу управлять анимациями и вибрациями рук
    public MassivHandsPlayer leftTargetScript;//включаем стандартные контроллеры
    public MassivHandsPlayer rightTargetScript;//включаем стандартные контроллеры
    
    private void Start()
    { 

        // Подписываемся на событие загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Отписываемся от события загрузки сцены
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OffVibro()
    {

        HandAnimationControllerLeft.StopVibration();
        HandAnimationControlRight.StopVibration();
        InputBridge.Instance.VibrateController(0f, 0f, 0f, grabLeft.HandSide); //выключаем вибрацию
        InputBridge.Instance.VibrateController(0f, 0f, 0f, grabRight.HandSide); //выключаем вибрацию
    }
    

    
    
    
    //если вдруг выходим из сцены, т овсё резко возвращаем на место
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        OffVibro();// отключаем вибрацию
        
        //если нажали на кнопку, то сбрасываем контроллеры на стандартные
        leftTargetScript?.ActivateObject(0);
        rightTargetScript?.ActivateObject(0);



    }
    
    
    
}
