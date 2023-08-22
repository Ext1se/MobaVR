using UnityEngine;
using UnityEngine.UI;  // ��� ������ � ����������� Text

public class TimerScript : MonoBehaviour
{
    public float timerDuration = 100f;  // ����������������� ������� � ��������
    private float timerLeft;

    public Text timerText;  // ������ �� ��������� ���������, ��� ����� ������������ ������

    public SliderMenu sliderMenuScript;  // ������ �� ������ SliderMenu

    private void Start()
    {
        timerDuration = 100f;
        timerLeft = timerDuration;
        UpdateTimerDisplay();
    }

    private void Update()
    {
        timerLeft -= Time.deltaTime;  // ��������� ���������� �����
        UpdateTimerDisplay();

        if (timerLeft <= 0)
        {
            TimerFinished();
        }
    }

    private void UpdateTimerDisplay()
    {
        timerText.text = Mathf.CeilToInt(timerLeft).ToString();  // ��������� ����� � ����������� � ������
    }

    private void TimerFinished()
    {
        sliderMenuScript.NextClickName();  // �������� ������� �� ������� �������
        
    }
}
