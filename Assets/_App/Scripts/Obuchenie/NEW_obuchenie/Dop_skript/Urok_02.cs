using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MobaVR;
using BNG;

public class Urok_02 : MonoBehaviour
{
    //скрипт получения команды игрока и указание где книга.
    
    [SerializeField]
    private Transform targetRed; // Точка для команды "RED"
    
    [SerializeField]
    private Transform targetRedWay; // Точка куда будет смотреть указатель от игрока для команды "RED"

    [SerializeField]
    private Transform targetBlue; // Точка для команды "BLUE"
    
    [SerializeField]
    private Transform targetBlueWay; //  Точка куда будет смотреть указатель от игрока для команды "BLUE"

    private CharacterActions characterActions;

    public PlayerSpawner spawnerReference;  //Skript na kotorom visit nash personazh
    public string team; //tip komandy
    //команда выбрана
    public bool RunTeamTarget =false;
    
    private void OnEnable()
    {
        // Найти и назначить объекты
        FindAndAssignTargets();
        
        //находим компонент с уроками
        characterActions = GetComponent<CharacterActions>();
        RunTeamTarget =false;
        
        //Получаем команду
        //if (spawnerReference.localPlayer != null)
        PlayerVR playerScript = null;
        
        {
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

            if (playerScript != null)
            {
                //team = playerScript.m_Teammate;
                
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
                
    }
    
    private void Update()
    {
        // Проверка на второй шаг обучения
        if(characterActions.CurrentStepIndex == 1 && !RunTeamTarget)
        {
            SetTargetPointBasedOnTeam();
        }
    }
    
    
    private void FindAndAssignTargets()
    {
        // Найти объекты с заданными именами
        GameObject redWayObject = GameObject.Find("FX_Heal_02_Red");
        GameObject blueWayObject = GameObject.Find("FX_Heal_02_Blue");

        // Проверить, найдены ли объекты, и назначить их соответствующим переменным
        if (redWayObject != null)
            targetRedWay = redWayObject.transform;
        else
             Debug.LogError("FX_Heal_02_Red object not found!");

        if (blueWayObject != null)
            targetBlueWay = blueWayObject.transform;
        else
            Debug.LogError("FX_Heal_02_Blue object not found!");
    }
    
    

    //отправляем позиции в зависимости от команды персонаж топает к кинге
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
    
    //это вызывается из книги. потом она сбивается.
    public void FinishUrok02()
    {
        if (characterActions == null || characterActions.tutorialSteps == null)
        {
            return;
        }
        
        if(characterActions.tutorialSteps.Length > 1) // проверка, чтобы убедиться, что у нас есть хотя бы один шаг обучения
        {
            //обнуляем переменную, чтобы использовать скрипт повторно
            characterActions.tutorialSteps[characterActions.CurrentStepIndex].targetPoint = null;
            characterActions.tutorialSteps[characterActions.CurrentStepIndex].TransformTargetWay = null;
            characterActions.tutorialSteps[1].isTaskCompleted = true;
        }
    }
    
    
}
