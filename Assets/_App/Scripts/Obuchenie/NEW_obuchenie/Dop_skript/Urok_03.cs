using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MobaVR;
using BNG;

public class Urok_03 : MonoBehaviour
{
   [System.Serializable]
    public class SoundEntry
    {
        public string soundName;
        public AudioClip soundClip;
    }

    public TargetRes targetsManage;
    
    public SaveInfoClass _SaveInfoClass;//тут получаем класс игрока
    public SoundEntry[] soundEntries; // Массив для записей звука
    public string ID; // Переменная ID для идентификации звука


    [SerializeField]
    private Transform targetRed;
    
    [SerializeField]
    private Transform targetRedWay; // Точка куда будет смотреть указатель от игрока для команды "RED"

    [SerializeField]
    private Transform targetBlue;
    
    [SerializeField]
    private Transform targetBlueWay; //  Точка куда будет смотреть указатель от игрока для команды "BLUE"

    private CharacterActions characterActions;

    public PlayerSpawner spawnerReference;  // Скрипт, на котором находится наш персонаж
    public string team;
    
    //команда выбрана
    public bool RunTeamTarget =false;//команда выбрана
    public bool RunTargets =false;//включаем мишени
    public bool FinishTargets =false;//если мишени отключены

    private void OnEnable()
    {
        //находим скрипт с уроками
        characterActions = GetComponent<CharacterActions>();
            //говорим, что команда не выбрана
        RunTeamTarget =false;
        RunTargets =false;
        
        //говорим, что плеер скрипт сейчас ноль
        PlayerVR playerScript = null;

        //ищем плеер скрипт, если он наш, то берём его в работу
        PlayerVR[] players = FindObjectsOfType<PlayerVR>();
        foreach (PlayerVR playerVR in players)
        {
            if (playerVR.photonView.IsMine)
            {
                playerScript = playerVR;
                break;
            }
        }

        
        if (playerScript == null)
        {
            playerScript = spawnerReference.localPlayer.GetComponent<PlayerVR>();
        }
            
        //получаем команду
        if (playerScript != null)
        {
            if (playerScript.TeamType == TeamType.RED)
            {
                team = "RED";
            }

            if (playerScript.TeamType == TeamType.BLUE)
            {
                team = "BLUE";
            }
        }
    }
    
    private void Update()
    {
        //если у нас сейчас идёт 3-й уроки
        if (characterActions.CurrentStepIndex == 2 && !RunTeamTarget)
        {
            //отправляем игрока к тиру, в зависимости от команды
            SetTargetPointBasedOnTeam();
                //присваиваем класс игрока к переменной ID
            ID = _SaveInfoClass.targetID;
        }  
        
        //если у нас сейчас идёт 3-й уроки у него сыгран основной звук
        if (characterActions.CurrentStepIndex == 2 && characterActions.tutorialSteps[2].mainTaskSoundRun)
        {
            //включим мишени, даже если они ещё не выключены все
            if (!RunTargets)
            {
                //говорим восстановить все мишени
                targetsManage.EnableTargetsImmediately();
                //мишени включены
                RunTargets =true;
            }
            
            //проверяем мишени
            CheckObjects();

        }
        AssignSoundByID();
    }

    //отправляем точки в переменные скрипта обучения
    private void SetTargetPointBasedOnTeam()
    {
        if (team == "RED")
        {
            characterActions.tutorialSteps[characterActions.CurrentStepIndex].targetPoint = targetRed;
            characterActions.tutorialSteps[characterActions.CurrentStepIndex].TransformTargetWay = targetRedWay;
        }
        else if (team == "BLUE")
        {
            characterActions.tutorialSteps[characterActions.CurrentStepIndex].targetPoint = targetBlue;
            characterActions.tutorialSteps[characterActions.CurrentStepIndex].TransformTargetWay = targetBlueWay;
        }
        
        //путь указан и команда выбрана
        RunTeamTarget = true;
    }

    //проверяем есть ли мишени
    void CheckObjects()
    {
        //если все объекты подбиты, завершаем урок
        FinishTargets = targetsManage.allTargetsDisabled;

        if (FinishTargets)
        {
            FinishTargets = false;
            FinishUrok03();
        }
           

    }
    

    //урок закончен
    public void FinishUrok03()
    {
        if (characterActions.tutorialSteps.Length > 2)
        {
            //обнуляем точки, для следующего запуска
            characterActions.tutorialSteps[characterActions.CurrentStepIndex].targetPoint = null; 
            
            //обнуляем точки пути, для следующего запуска
            characterActions.tutorialSteps[characterActions.CurrentStepIndex].TransformTargetWay = null;
            
            //обнуляем звук
            characterActions.tutorialSteps[characterActions.CurrentStepIndex].mainTaskSound = null;
            
            //говорим, что урок пройдён.
            characterActions.tutorialSteps[2].isTaskCompleted = true;
        }
    }

    // Функция для установки звука на основе ID
    void AssignSoundByID()
    {
        
        // Если сейчас идёт не второй урок, выйти из функции
        if(characterActions.CurrentStepIndex != 2)
        {
            return;
        }
        
        foreach (SoundEntry entry in soundEntries)
        {
            if (entry.soundName == ID)
            {
                if (characterActions.CurrentStepIndex < characterActions.tutorialSteps.Length)
                {
                    characterActions.tutorialSteps[characterActions.CurrentStepIndex].mainTaskSound = entry.soundClip;
                }

                break;
            }
        }
    }
}
