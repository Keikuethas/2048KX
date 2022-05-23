using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для хранения справочной инфы.
/// </summary>
public class Config : MonoBehaviour
{
    // ---- настройка движения ----

    /// <summary>
    /// Верхняя левая плитка - координаты
    /// </summary>
    public const float startpos = 3.391F; //Это для Y. Для X взять с противоположным знаком.

    /// <summary>
    /// Смещение (одинаково по осям)
    /// </summary>
    public const float delta = 2.259F;

    /// <summary>
    /// кол-во кадров, которые должна двигаться плитка (на 1 ячейку)
    /// </summary>
    public const int moveFrameCount = 15;

    /// <summary>
    /// Расчётное кол-во кадров в секунду
    /// </summary>
    public const int averageFPS = 60;


    // ---- настройка перехода ----

    /// <summary>
    /// размер плитки при появлении
    /// </summary>
    public const float scaleStart = 0.75F;

    /// <summary>
    /// кол-во кадров, которые должна увеличиваться плитка (переход)
    /// </summary>
    public const int scaleFrameCount = 12;


    // ---- префабы ----

    /// <summary>
    /// Префабы. Примечание: использовать FindObjectOfType, т. к. нестатическая фигня
    /// </summary>
    public GameObject[] prefabs = new GameObject[0]; //Важно! В Unity вручную добавить префабы
    /*
     * Список префабов:
     * 0 - 10 = плитки от 2 до 2048
     */

    /// <summary>
    /// Примечание: нестатический метод, FindObjectOfType в помощь
    /// </summary>
    /// <param name="tag">Тэг нужного префаба</param>
    /// <returns></returns>
    public GameObject GetPrefabByTag(string tag)
    {
        foreach (GameObject g in prefabs)
            if (g.CompareTag(tag))
                return g;
        return null;
    }

    /// <summary>
    /// Примечание: нестатический метод, FindObjectOfType в помощь
    /// </summary>
    /// <param name="name">Имя нужного префаба</param>
    /// <returns></returns>
    public GameObject GetPrefabByName(string name)
    {
        foreach (GameObject g in prefabs)
            if (g.name == name)
                return g;
        return null;
    }


    // ---- уничтожение ----

    /// <summary>
    /// задержка перед уничтожением (началом перехода)
    /// </summary>
    public const int destroyingDelay = 250;

    /// <summary>
    /// Предел уменьшения при уничтожении
    /// </summary>
    public const float endScale = 0.1F;

    /// <summary>
    /// Поворот при уничтожении
    /// </summary>
    public const float endRotation = 1;

    /// <summary>
    /// Длительность уничтожения (кадры)
    /// </summary>
    public const int destroyingFrameCount = 20;
}
