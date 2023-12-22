using MobaVR;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using TMPro;
using UnityEngine;

public class DieView : MonoBehaviour
{
    [SerializeField] private WizardPlayer wizardPlayer;

    public GameObject DiePanel;
    public GameObject DieInfoPanel;
    public GameObject DieInfoPanelPvE;
    public TextMeshProUGUI KillerName;


    public AudioClip[] sounds;

    private AudioSource audioSource;
    private ClassicGameSession gameSession;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameSession = FindObjectOfType<ClassicGameSession>();
        
        DieInfoPanel.SetActive(false);
        DieInfoPanelPvE.SetActive(false);
        DiePanel.SetActive(false);
    }

    private void OnEnable()
    {
        wizardPlayer.OnDie += OnDie;
        wizardPlayer.OnReborn += OnReborn;
    }

    private void OnDisable()
    {
        wizardPlayer.OnDie -= OnDie;
        wizardPlayer.OnReborn -= OnReborn;
    }

    private void OnDie()
    {
        PlayRandomSound();
        DiePanel.SetActive(true);
    }

    private void OnReborn()
    {
        DiePanel.SetActive(false);
        DieInfoPanel.SetActive(false);
        DieInfoPanelPvE.SetActive(false);
    }

    public void SetDieInfo(string nickname)
    {
        if (gameSession.Mode.GameModeType is GameModeType.PVP or GameModeType.MOBA)
        {
            DieInfoPanelPvE.SetActive(false);
            DieInfoPanel.SetActive(true);
            KillerName.text = nickname;
        }
        else
        {
            DieInfoPanel.SetActive(false);
            DieInfoPanelPvE.SetActive(true);
        }
    }

    public void PlayRandomSound()
    {
        if (sounds.Length > 0)
        {
            // ���������� ��������� ������ ��� ������ ���������� ����� �� �������
            int randomIndex = Random.Range(0, sounds.Length);

            // ������������� ����, ��������������� ���������� �������
            AudioClip soundToPlay = sounds[randomIndex];
            audioSource.PlayOneShot(soundToPlay);
        }

    }

}
