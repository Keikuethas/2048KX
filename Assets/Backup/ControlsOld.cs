using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsOld : MonoBehaviour
{

    bool pressed = false;

    // Start is called before the first frame update
    void Start()
    {
        var torefresh = GetObjectsFromLayer("плитки");
        for (int i = 0; i < torefresh.Count; i++)
            torefresh[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        Debug.Log("objects refreshed");
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis("Horizontal") != 0 && pressed == false)
        {
            if (Input.GetAxis("Horizontal") > 0) MoveAllRight();
            else if (Input.GetAxis("Horizontal") < 0) MoveAllLeft();
            pressed = true;
        }

        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            pressed = false;
        else
            pressed = true;
    }

    List<GameObject> GetObjectsFromLayer(string layerName)
    {
        List<GameObject> objects = new List<GameObject>();
        int layer = LayerMask.NameToLayer(layerName);
        foreach (GameObject g in FindObjectsOfType<GameObject>())
            if (g.layer == layer)
                objects.Add(g);
        return objects;
    }

    void MoveAllRight()
    {
        //общая для всех часть
        var objs = GetObjectsFromLayer("плитки");
        GameObject[,] posObj = new GameObject[4, 4];
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
            {
                GameObject New = new GameObject();
                New.tag = "killmepls";
                posObj[x, y] = New;
            }
        for (int i = 0; i < objs.Count; i++) //раскидываем по сетке
        {
            GameObject g = objs[i];
            int x = (int)Math.Abs((-Config.startpos - g.transform.position.x) / Config.delta);
            int y = (int)Math.Abs((Config.startpos - g.transform.position.y) / Config.delta);
            posObj[x, y] = g;
        }

        //отдельная часть для конкретного движения
        //Примечание: изменение цвета используется для предотвращения повторного слияния.
        int movements = 1;
        int emergency = 0;
        while (movements > 0)
        {
            movements = 0;
            for (int y = 0; y < 4; y++)
                for (int x = 2; x >= 0; x--)
                {
                    var otherObj = posObj[x + 1, y];
                    var thisObj = posObj[x, y];
                    if (thisObj.CompareTag("killmepls")) continue;
                    if (!otherObj.CompareTag("killmepls"))
                        if (thisObj.CompareTag(otherObj.tag) && otherObj.GetComponent<SpriteRenderer>().color.a == 1)
                        {
                            string tag = thisObj.tag;
                            Debug.Log(thisObj.name + " gonna eat " + otherObj.name + " with tag " + otherObj.tag);
                            Destroy(thisObj);
                            Destroy(otherObj);
                            var prefab = (FindObjectOfType(typeof(Config)) as Config).GetPrefabByTag((int.Parse(tag)*2).ToString());
                            thisObj = Instantiate(prefab, new Vector3(-Config.startpos + Config.delta * (x + 1), Config.startpos - Config.delta * y, -1), Quaternion.identity);
                            //создаём пустоту там, где были
                            GameObject New = new GameObject();
                            New.tag = "killmepls";
                            posObj[x, y] = New;
                            posObj[x + 1, y] = thisObj;
                            movements++;
                        }
                        else
                        {

                        }
                    else
                    {
                        Destroy(otherObj); //удаляем пустоту справа
                        thisObj.transform.position = new Vector3(-Config.startpos + Config.delta * (x + 1), Config.startpos - Config.delta * y, -1); //двигаем
                        //создаём пустоту там, где были
                        GameObject New = new GameObject();
                        New.tag = "killmepls";
                        posObj[x, y] = New;
                        posObj[x + 1, y] = thisObj;
                        movements++;
                    }
                }
            GameObject[] tokill = GameObject.FindGameObjectsWithTag("killmepls");
            for (int i = 0; i < tokill.Length; i++)
                Destroy(tokill[i]);
            emergency++;
            if (emergency > 5)
            {
                Debug.LogError("Had to break your loop. Make your code less shit.");
                break;
            }
            
        }
        var torefresh = GetObjectsFromLayer("плитки");
        for (int i = 0; i < torefresh.Count; i++)
            torefresh[i].GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        Debug.Log("objects refreshed");
    }

    void MoveAllLeft()
    {

    }

    void MoveAllDown()
    {

    }

    void MoveAllUp()
    {

    }
}
