using UnityEngine;

//скрипт который анимирует взлёт контроллеров к лицу и обратно в руки

public class ControllerDisplayScript : MonoBehaviour
{
    
    // Установить вращение объекта
    public float rotationX = 0.0f; // Угол вращения вокруг оси X
    public float rotationY = 0.0f; // Угол вращения вокруг оси Y
    public float rotationZ = 0.0f; // Угол вращения вокруг оси Z
    
    public Transform targetPosition; // Переменная 2: точка, куда переместится объект с контроллером
    public AudioClip soundClip; // Переменная 3: звук
    public Transform originalParent; // Переменная 4: стандартное положение контроллера

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isMovingToTarget = false;
    private bool isRotating = false;
    private bool isReturning = false;

    
    private float totalRotation = 0f; // переменную для отслеживания общего угла поворота
    


    public float rotationTime = 1f; // Время в секундах, за которое объект должен совершить полный оборот на 360 градусов
    private float rotationTimer = 0f; // Таймер для отслеживания времени вращения
    
    
    private void Start()
    {
        // Сохранить исходное положение и ориентацию
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    public void Activate()
    {
        // Установить родительский объект
        transform.SetParent(targetPosition);

        // Воспроизвести звук
        if (soundClip != null)
        {
            AudioSource.PlayClipAtPoint(soundClip, transform.position);
        }

        // Начать перемещение к цели
        isMovingToTarget = true;
    }

    private void Update()
    {
        if (isMovingToTarget)
        {
            MoveToTarget();
        }
        else if (isRotating)
        {
            //RotateAround();
            Ogidanie();
        }
        else if (isReturning)
        {
            ReturnToOriginalPosition();
        }
    }

    private void MoveToTarget()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, Time.deltaTime);

        if (transform.localPosition == Vector3.zero)
        {
            isMovingToTarget = false;
            isRotating = true;
        }
    }

    private void RotateAround()
    {
        // Сброс вращения объекта перед началом вращения
        if (rotationTimer == 0f)
        {
            transform.localRotation = Quaternion.identity;
        }

        // Увеличиваем таймер на время, прошедшее с последнего кадра
        rotationTimer += Time.deltaTime;

        // Вычисляем долю времени, прошедшую от начала вращения
        float fraction = rotationTimer / rotationTime;

        // Вычисляем текущий угол поворота на основе доли прошедшего времени
        float currentRotation = fraction * 360f;
        float rotationThisFrame = currentRotation - totalRotation; // Угол поворота за текущий кадр

        transform.Rotate(Vector3.right, rotationThisFrame); // Поворачиваем объект вокруг оси X
        totalRotation = currentRotation; // Обновляем общий угол поворота

        // Проверяем, достиг ли объект полного вращения на 360 градусов
        if (fraction >= 1f)
        {
            isRotating = false;
            isReturning = true;
            totalRotation = 0f; // Сбрасываем общий угол поворота для будущих вращений
            rotationTimer = 0f; // Сбрасываем таймер вращения
        }
    }
    
    private void Ogidanie()
    {
        // Сброс вращения объекта перед началом вращения
        if (rotationTimer == 0f)
        {
            transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        }

        // Увеличиваем таймер на время, прошедшее с последнего кадра
        rotationTimer += Time.deltaTime;

        // Вычисляем долю времени, прошедшую от начала вращения
        float fraction = rotationTimer / rotationTime;
        
        // Проверяем, достиг ли объект полного вращения на 360 градусов
        if (fraction >= 1f)
        {
            isRotating = false;
            isReturning = true;
            rotationTimer = 0f; // Сбрасываем таймер вращения
        }
    }
    



    private void ReturnToOriginalPosition()
    {
       
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPosition, Time.deltaTime);
        // transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalRotation, Time.deltaTime);
        transform.localRotation = Quaternion.identity;

        if (transform.localPosition == originalPosition && transform.localRotation == originalRotation)
        {
            isReturning = false;
        }
    }

    public void Deactivate()
    {
        // Мгновенно вернуть в исходное положение
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;

        isMovingToTarget = false;
        isRotating = false;
        isReturning = false;
    }
}
