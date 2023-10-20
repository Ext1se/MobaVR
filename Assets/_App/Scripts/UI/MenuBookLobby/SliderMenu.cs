using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SliderMenu : MonoBehaviour
{

    public AudioClip hoverSound;
    public AudioClip clickSound;

    public Button[] characterButtons;

    public Vector3[] originalScales;
    
    // Ссылка на Urok_02, чтобы завершить обучение
    public Urok_02 urok02Script;

    public GameObject MenuClass;
    public GameObject MenuName;
    public GameObject MenuHands;
    public GameObject MenuGoTir;
    
    public GameObject RightHead;
    public GameObject LeftHead;

    public Text HandsLeftRight;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
       
        //characterButtons = GetComponentsInChildren<Button>();

        originalScales = new Vector3[characterButtons.Length];
        for (int i = 0; i < characterButtons.Length; i++)
        {
            originalScales[i] = characterButtons[i].transform.localScale;
        }
    }

    public void OnCharacterButtonEnter(int buttonIndex)
    {
        if (buttonIndex >= characterButtons.Length)
        {
            return;
        }
        
        if (buttonIndex >= originalScales.Length)
        {
            return;
        }
        
        characterButtons[buttonIndex].transform.localScale = originalScales[buttonIndex] * 1.2f;
        PlaySound(hoverSound);
    }




    public void OnCharacterButtonExit(int buttonIndex)
    {
        if (buttonIndex >= characterButtons.Length)
        {
            return;
        }
        
        if (buttonIndex >= originalScales.Length)
        {
            return;
        }
        
        characterButtons[buttonIndex].transform.localScale = originalScales[buttonIndex];
    }

    public void OnCharacterButtonClick(int buttonIndex)
    {
        
        PlaySound(clickSound);
    

    }



    public void NextClick()
    {
        
        PlaySound(clickSound);
        MenuClass.SetActive(false);
        MenuName.SetActive(true);
        MenuHands.SetActive(false);
        MenuGoTir.SetActive(false);
       
    }


    public void NextClickName()
    {
        
        PlaySound(clickSound);
        MenuClass.SetActive(false);
        MenuName.SetActive(false);
        MenuHands.SetActive(true);
        MenuGoTir.SetActive(false);
    }

    public void NextClickHands()
    {
        
        PlaySound(clickSound);
        MenuClass.SetActive(false);
        MenuName.SetActive(false);
        MenuHands.SetActive(false);
        MenuGoTir.SetActive(true);
    }  
    
    
    public void RessetMenu()
    {
        
        PlaySound(clickSound);
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
        MenuClass.SetActive(true);
        MenuName.SetActive(false);
        MenuHands.SetActive(false);
        MenuGoTir.SetActive(false);
    }

    public void Red_team_Click()
    {
      
        PlaySound(clickSound);


    }

    public void Blue_team_Click()
    {
        
        PlaySound(clickSound);

    }


    public void LeftHands()
    {
        //сюда нужно добавить переменную ,чтобы потом использовать её
        PlaySound(clickSound);
       // HandsLeftRight.text = "Левая рука";
       RightHead.SetActive(false);
       LeftHead.SetActive(true);

    }   
    
    public void RightHands()
    {
    //сюда нужно добавить переменную ,чтобы потом использовать её
        PlaySound(clickSound);
       // HandsLeftRight.text = "Правая рука";
       LeftHead.SetActive(false);
       RightHead.SetActive(true);
   
    }

    
    //функция для запуска FinishUrok02() если он включен т.е. если обучение идёт
    public void TriggerFinishUrok02()
    {
        if (urok02Script != null)
        {
            // Проверка, является ли объект, на котором находится скрипт Urok_02, активным
            if (urok02Script.gameObject.activeInHierarchy)
            {
                urok02Script.FinishUrok02();
            }
            else
            {
              //  Debug.LogWarning("Объект с Urok_02 не активен в иерархии!");
            }
        }
        else
        {
          //  Debug.LogWarning("Ссылка на Urok_02 не установлена!");
        }
    }
    



    private void PlaySound(AudioClip sound)
    {
        // ��������������� �����
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
        }
    }
}
