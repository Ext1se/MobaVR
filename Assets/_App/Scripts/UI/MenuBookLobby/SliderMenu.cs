using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SliderMenu : MonoBehaviour
{
    public GameObject descriptionPanel;
    public Text descriptionText;

    public AudioClip hoverSound;
    public AudioClip clickSound;

    public Button[] characterButtons;

    public string[] descriptions;

    public Vector3[] originalScales;

    private void Start()
    {
        // ������� ��� ������ � ��������� �� ������������ �������
        //characterButtons = GetComponentsInChildren<Button>();

        originalScales = new Vector3[characterButtons.Length];
        for (int i = 0; i < characterButtons.Length; i++)
        {
            originalScales[i] = characterButtons[i].transform.localScale;
        }
    }

    public void OnCharacterButtonEnter(int buttonIndex)
    {
        // ����������� ������ ������ ��� ���������
        characterButtons[buttonIndex].transform.localScale = originalScales[buttonIndex] * 1.2f;

        // ������������� ���� ���������
        PlaySound(hoverSound);
    }




    public void OnCharacterButtonExit(int buttonIndex)
    {
        // ���������� ������ ������������ ������
        characterButtons[buttonIndex].transform.localScale = originalScales[buttonIndex];
    }

    public void OnCharacterButtonClick(int buttonIndex)
    {
        // ������������� ���� �������
        PlaySound(clickSound);

        // �������� ��� �������� ���������� � ���������� �������� ���������� ���������
        for (int i = 0; i < characterButtons.Length; i++)
        {
            if (i == buttonIndex)
            {
                characterButtons[i].transform.localScale = originalScales[i] * 0.8f;
                descriptionPanel.SetActive(true);
                descriptionText.text = GetCharacterDescription(buttonIndex);
            }
            else
            {
                characterButtons[i].transform.localScale = originalScales[i];
            }
        }
    }



    public void NextClick()
    {
        // ������������� ���� �������
        PlaySound(clickSound);

      
    }


    public void Red_team_Click()
    {
        // ������������� ���� �������
        PlaySound(clickSound);


    }

    public void Blue_team_Click()
    {
        // ������������� ���� �������
        PlaySound(clickSound);


    }



    private string GetCharacterDescription(int buttonIndex)
    {
        // ���������� �������� ��������� �� ������� ������ (����� ����� ���� ���� ������ ��������)
       //descriptions[] = {
       //     "������: ������� � ������� ����.",
       //     "�����: ������ � �������������� �����������.",
       //     "��� ����: �������� ���� � �������."
       // };

        if (buttonIndex >= 0 && buttonIndex < descriptions.Length)
        {
            return descriptions[buttonIndex];
        }

        return "�������� ��������� ����������.";
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
