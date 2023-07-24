using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBookLobby : MonoBehaviour
{
    public Text titleText;
    public Text outputText;
    public TextData[] textElements;
    public ZonaBook zonaBook;
    public AudioClip clickSound;

    public void OnButtonClick(string buttonId)
    {
        foreach (TextData textData in textElements)
        {
            if (textData.id == buttonId)
            {
                // ������������� ���� �������
                PlaySound(clickSound);


                titleText.text = textData.title; // ���������
                outputText.text = textData.text; // �����

                //���������� �������� �� ��������� � ������, ������� ��������� ������ ��� ���� �� �����
                zonaBook.targetID = textData.id;
                zonaBook.UpdateTargetID(textData.id);
                break;
            }
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


[System.Serializable]
public class TextData
{
    public string id;
    public string title;
    public string text;
}