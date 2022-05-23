using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� ��� �������� ���������� ����.
/// </summary>
public class Config : MonoBehaviour
{
    // ---- ��������� �������� ----

    /// <summary>
    /// ������� ����� ������ - ����������
    /// </summary>
    public const float startpos = 3.391F; //��� ��� Y. ��� X ����� � ��������������� ������.

    /// <summary>
    /// �������� (��������� �� ����)
    /// </summary>
    public const float delta = 2.259F;

    /// <summary>
    /// ���-�� ������, ������� ������ ��������� ������ (�� 1 ������)
    /// </summary>
    public const int moveFrameCount = 15;

    /// <summary>
    /// ��������� ���-�� ������ � �������
    /// </summary>
    public const int averageFPS = 60;


    // ---- ��������� �������� ----

    /// <summary>
    /// ������ ������ ��� ���������
    /// </summary>
    public const float scaleStart = 0.75F;

    /// <summary>
    /// ���-�� ������, ������� ������ ������������� ������ (�������)
    /// </summary>
    public const int scaleFrameCount = 12;


    // ---- ������� ----

    /// <summary>
    /// �������. ����������: ������������ FindObjectOfType, �. �. ������������� �����
    /// </summary>
    public GameObject[] prefabs = new GameObject[0]; //�����! � Unity ������� �������� �������
    /*
     * ������ ��������:
     * 0 - 10 = ������ �� 2 �� 2048
     */

    /// <summary>
    /// ����������: ������������� �����, FindObjectOfType � ������
    /// </summary>
    /// <param name="tag">��� ������� �������</param>
    /// <returns></returns>
    public GameObject GetPrefabByTag(string tag)
    {
        foreach (GameObject g in prefabs)
            if (g.CompareTag(tag))
                return g;
        return null;
    }

    /// <summary>
    /// ����������: ������������� �����, FindObjectOfType � ������
    /// </summary>
    /// <param name="name">��� ������� �������</param>
    /// <returns></returns>
    public GameObject GetPrefabByName(string name)
    {
        foreach (GameObject g in prefabs)
            if (g.name == name)
                return g;
        return null;
    }


    // ---- ����������� ----

    /// <summary>
    /// �������� ����� ������������ (������� ��������)
    /// </summary>
    public const int destroyingDelay = 250;

    /// <summary>
    /// ������ ���������� ��� �����������
    /// </summary>
    public const float endScale = 0.1F;

    /// <summary>
    /// ������� ��� �����������
    /// </summary>
    public const float endRotation = 1;

    /// <summary>
    /// ������������ ����������� (�����)
    /// </summary>
    public const int destroyingFrameCount = 20;
}
