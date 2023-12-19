using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//скрипт, рисующий луч для обучения, чтобы игроки знали куда ходить. Этот луч прерывается, если одна из точек пропала, и этот луч прерывается, если точки близко друг к другу


public class ConstantBezierLine : MonoBehaviour {

    public GameObject startPoint; // Начальная точка линии
    public GameObject endPoint;   // Конечная точка линии
    public GameObject Strelka;   // Стрелка
    public LineRenderer lineRenderer; // Компонент LineRenderer для рисования линии

    public int segmentCount = 100; // Количество сегментов в линии
    public float lerpAmount = 0.5f; // Коэффициент сглаживания
    public float heightAdjustment = 0.5f; // Регулировка высоты дуги
    public float minDistance = 1.0f; // Минимальное расстояние между объектами для рисования линии

    void Start() {
        if(lineRenderer == null) {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    void Update() {
        if(startPoint != null && endPoint != null && startPoint.activeInHierarchy && endPoint.activeInHierarchy) {
            if(Vector3.Distance(startPoint.transform.position, endPoint.transform.position) >= minDistance) {
                DrawBezierCurve();
                lineRenderer.enabled = true;
                Strelka.SetActive(true);
            } else {
                // Выключаем линию, если объекты слишком близко друг к другу
                lineRenderer.enabled = false;
                Strelka.SetActive(false);
            }
        } else {
            // Выключаем линию, если один из объектов отключен или исчез
            lineRenderer.enabled = false;
            Strelka.SetActive(false);
        }
    }

    void DrawBezierCurve() {
        Vector3 point0 = startPoint.transform.position;
        Vector3 point2 = endPoint.transform.position;

        // Вычисляем среднюю точку для дуги
        Vector3 midPoint = Vector3.Lerp(point0, point2, lerpAmount);
        midPoint.y += heightAdjustment;

        // Настройка параметров линии
        lineRenderer.positionCount = segmentCount;
        lineRenderer.useWorldSpace = true;

        // Рисуем дугу
        float t = 0f;
        Vector3 b = new Vector3(0, 0, 0);
        for (int i = 0; i < segmentCount; i++) {
            b = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * midPoint + t * t * point2;
            lineRenderer.SetPosition(i, b);
            t += (1 / (float)segmentCount);
        }
    }
}
