using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{

    bool pressed = false;
    /// <summary>
    /// выполн€етс€ ли сейчас действие
    /// </summary>
    bool locked = false;
    /// <summary>
    /// сколько плиток двигаем
    /// </summary>
    public int moves = 0;
    /// <summary>
    /// сколько уже подвинули
    /// </summary>
    public int movesDone = 0;
    /// <summary>
    /// куда двигаем (отсчЄт по часовой)
    /// </summary>
    int direction = -1;
    /// <summary>
    /// ƒвигалось ли что-то вообще после последнего нажати€
    /// </summary>
    bool moved = false;
    /// <summary>
    /// очищаетс€ ли поле в данный момент
    /// </summary>
    bool destroying = false;
    public SpriteRenderer[] scoreTiles = new SpriteRenderer[0];  //ƒобавить вручную, слева направо.
    public SpriteRenderer[] bestScoreTiles = new SpriteRenderer[0];  //ƒобавить вручную, слева направо.

    int score = 0;
    int best = 0;
    System.Random random = new System.Random();


    void SetScore(SpriteRenderer[] tiles, int score)
    {
        var scoreStr = score.ToString();
        for (; scoreStr.Length < tiles.Length;)
            scoreStr = "0" + scoreStr;
        for (int i = 0; i < tiles.Length; i++)
        {
            var prefab = (FindObjectOfType(typeof(Config)) as Config).GetPrefabByTag("score " + scoreStr[i]);
            var sprite = prefab.GetComponent<SpriteRenderer>().sprite;
            tiles[i].sprite = sprite;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        SetScore(scoreTiles, score);
    }

    public void ReportMove()
    {
        movesDone++;
        moved = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        int n = GetObjectsFromLayer("плитки").Count;
        if (n == 16) n = 0;
        for (; n < 2; n++)
            GenerateNewTile();
        RefreshTiles();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckLose() & !destroying)
        {
            CleanField();
            if (score > best) SetScore(bestScoreTiles, (best = score));
            Debug.Log(best);
        }
        if (CheckEmpty()) Start();

        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !pressed && !locked)
        {
            if (Input.GetAxis("Horizontal") > 0) MoveAllRight();
            else if (Input.GetAxis("Horizontal") < 0) MoveAllLeft();
            if (Input.GetAxis("Vertical") > 0) MoveAllUp();
            else if (Input.GetAxis("Vertical") < 0) MoveAllDown();
            locked = true;
            
            return;
        }

        
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            pressed = false;
        else
            pressed = true;
        

        if (locked && moves == 0 && direction < 0)
        {
            locked = false;
            if (moved)
            {
                StopAllTiles();
                GenerateNewTile();
                RefreshTiles();
                moved = false;
            }
            
        }
        else if (movesDone >= moves && moves > 0) //ƒа, их может быть больше.  ак? ј € не знаю.
        {
            moves = 0;
            movesDone = 0;
            switch (direction)
            {
                case 0:
                    MoveAllLeft();
                    break;
                case 1:
                    MoveAllUp();
                    break;
                case 2:
                    MoveAllRight();
                    break;
                case 3:
                    MoveAllDown();
                    break;
            } 
        }
        CleanTrash();
    }

    bool CheckFull()
    {
        var posObj = GetTiles();
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                if (posObj[x, y].CompareTag("killmepls"))
                    return false;
        return true;
    }

    bool CheckEmpty()
    {
        var posObj = GetTiles();
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                if (!posObj[x, y].CompareTag("killmepls"))
                    return false;
        destroying = false;
        return true;
    }

    GameObject[,] GetTiles()
    {
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
            int x = (int)Math.Round(Math.Abs((-Config.startpos - g.transform.position.x) / Config.delta));
            int y = (int)Math.Round(Math.Abs((Config.startpos - g.transform.position.y) / Config.delta));
            posObj[x, y] = g;
        }
        return posObj;
    }

    bool CheckLose()
    {
        if (!CheckFull()) return false;
        var posObj = GetTiles();
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
            {
                var thisObj = posObj[x, y];
                var otherObj1 = posObj[x + 1, y];
                var otherObj2 = posObj[x, y + 1];
                if (thisObj.CompareTag(otherObj1.tag) || thisObj.CompareTag(otherObj2.tag)) return false;
            }
        for (int x = 3; x > 0; x--)
            for (int y = 3; y > 0; y--)
            {
                var thisObj = posObj[x, y];
                var otherObj1 = posObj[x - 1, y];
                var otherObj2 = posObj[x, y - 1];
                if (thisObj.CompareTag(otherObj1.tag) || thisObj.CompareTag(otherObj2.tag)) return false;
            }
        CleanTrash();
        return true;
    }

    void CleanField()
    {
        var toDestroy = GetObjectsFromLayer("плитки");
        for (int i = 0; i < toDestroy.Count; i++)
            toDestroy[i].GetComponent<Tile>().Destroy(Config.destroyingDelay);
        destroying = true;
    }

    void RefreshTiles()
    {
            var torefresh = GetObjectsFromLayer("плитки");
            for (int i = 0; i < torefresh.Count; i++)
                torefresh[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    void StopAllTiles()
    {
        var tiles = GetTiles();
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                if (!tiles[x, y].CompareTag("killmepls"))
                    tiles[x, y].GetComponent<Tile>().Stop(x, y);
    }

    void CleanTrash()
    {
        GameObject[] tokill = GameObject.FindGameObjectsWithTag("killmepls");
        for (int i = 0; i < tokill.Length; i++)
            Destroy(tokill[i]);
    }

    void GenerateNewTile()
    {
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
            int x = (int)Math.Round(Math.Abs((-Config.startpos - g.transform.position.x) / Config.delta));
            int y = (int)Math.Round(Math.Abs((Config.startpos - g.transform.position.y) / Config.delta));
            posObj[x, y] = g;
        }

        for(int emergency = 0; emergency < 10; emergency++)
        {
            int x = random.Next(4);
            int y = random.Next(4);
            if (posObj[x, y].CompareTag("killmepls"))
            {
                string tag = "2";
                if (random.Next(5) == 1) tag = "4";
                var prefab = (FindObjectOfType(typeof(Config)) as Config).GetPrefabByTag(tag);
                Instantiate(prefab,
                    new Vector3(-Config.startpos + Config.delta * x, Config.startpos - Config.delta * y, -1),
                    Quaternion.identity);
                return;
            }
        }
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                if (posObj[x, y].CompareTag("killmepls"))
                {
                    string tag = "2";
                    if (random.Next(5) == 1) tag = "4";
                    var prefab = (FindObjectOfType(typeof(Config)) as Config).GetPrefabByTag(tag);
                    Instantiate(
                        prefab,
                        new Vector3(-Config.startpos + Config.delta * x, Config.startpos - Config.delta * y, -1),
                        Quaternion.identity
                        );
                    return;
                }
        Debug.LogError("No place to generate a new tile.");
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
        direction = 2; //двигать будем вправо
        //обща€ дл€ всех часть
        var objs = GetObjectsFromLayer("плитки");
        GameObject[,] posObj = GetTiles();

        //отдельна€ часть дл€ конкретного движени€
        //ѕримечание: изменение цвета используетс€ дл€ предотвращени€ повторного сли€ни€.
        for (int y = 0; y < 4; y++)
            for (int x = 2; x >= 0; x--)
            {
                var otherObj = posObj[x + 1, y];
                var thisObj = posObj[x, y];
                if (thisObj.CompareTag("killmepls")) continue;

                if (
                    (
                    thisObj.CompareTag(otherObj.tag) &&
                    thisObj.GetComponent<SpriteRenderer>().color.a == 1 &&
                    otherObj.GetComponent<SpriteRenderer>().color.a == 1 &&
                    thisObj.transform.localScale.x > Config.scaleStart &&
                    otherObj.transform.localScale.x > Config.scaleStart
                    ) || 
                    otherObj.CompareTag("killmepls")
                    )
                {
                    thisObj.GetComponent<Tile>().Move(direction); //двигаем
                    //создаЄм пустоту там, где были
                    GameObject New = new GameObject();
                    New.tag = "killmepls";
                    posObj[x, y] = New;
                    posObj[x + 1, y] = thisObj;
                    moves++;
                }
            }
        if (moves == 0) direction = -1;
    }

    void MoveAllLeft()
    {
        direction = 0; //двигать будем влево
        //обща€ дл€ всех часть
        var objs = GetObjectsFromLayer("плитки");
        GameObject[,] posObj = GetTiles();

        //отдельна€ часть дл€ конкретного движени€
        //ѕримечание: изменение цвета используетс€ дл€ предотвращени€ повторного сли€ни€.
        for (int y = 0; y < 4; y++)
            for (int x = 1; x < 4; x++)
            {
                var otherObj = posObj[x - 1, y];
                var thisObj = posObj[x, y];
                if (thisObj.CompareTag("killmepls")) continue;
                if (
                    (
                    thisObj.CompareTag(otherObj.tag) &&
                    thisObj.GetComponent<SpriteRenderer>().color.a == 1 &&
                    otherObj.GetComponent<SpriteRenderer>().color.a == 1 &&
                    thisObj.transform.localScale.x > Config.scaleStart &&
                    otherObj.transform.localScale.x > Config.scaleStart
                    ) ||
                    otherObj.CompareTag("killmepls")
                    )
                {
                    thisObj.GetComponent<Tile>().Move(direction); //двигаем
                    //создаЄм пустоту там, где были
                    GameObject New = new GameObject();
                    New.tag = "killmepls";
                    posObj[x, y] = New;
                    posObj[x - 1, y] = thisObj;
                    moves++;
                }
            }
        if (moves == 0) direction = -1;
    }

    void MoveAllDown()
    {
        direction = 3; //двигать будем вниз
        //обща€ дл€ всех часть
        var objs = GetObjectsFromLayer("плитки");
        GameObject[,] posObj = GetTiles();

        //отдельна€ часть дл€ конкретного движени€
        //ѕримечание: изменение цвета используетс€ дл€ предотвращени€ повторного сли€ни€.
        for (int y = 2; y >= 0; y--)
            for (int x = 0; x < 4; x++)
            {
                var otherObj = posObj[x, y + 1];
                var thisObj = posObj[x, y];
                if (thisObj.CompareTag("killmepls")) continue;
                if (
                    (
                    thisObj.CompareTag(otherObj.tag) &&
                    thisObj.GetComponent<SpriteRenderer>().color.a == 1 &&
                    otherObj.GetComponent<SpriteRenderer>().color.a == 1 &&
                    thisObj.transform.localScale.x > Config.scaleStart &&
                    otherObj.transform.localScale.x > Config.scaleStart
                    ) ||
                    otherObj.CompareTag("killmepls")
                    )
                {
                    thisObj.GetComponent<Tile>().Move(direction); //двигаем
                    //создаЄм пустоту там, где были
                    GameObject New = new GameObject();
                    New.tag = "killmepls";
                    posObj[x, y] = New;
                    posObj[x, y + 1] = thisObj;
                    moves++;
                }
            }
        if (moves == 0) direction = -1;
    }

    void MoveAllUp()
    {
        direction = 1; //двигать будем вверх
        //обща€ дл€ всех часть
        var objs = GetObjectsFromLayer("плитки");
        GameObject[,] posObj = GetTiles();

        //отдельна€ часть дл€ конкретного движени€
        //ѕримечание: изменение цвета используетс€ дл€ предотвращени€ повторного сли€ни€.
        for (int y = 1; y < 4; y++)
            for (int x = 0; x < 4; x++)
            {
                var otherObj = posObj[x, y - 1];
                var thisObj = posObj[x, y];
                if (thisObj.CompareTag("killmepls")) continue;
                if (
                    (
                    thisObj.CompareTag(otherObj.tag) &&
                    thisObj.GetComponent<SpriteRenderer>().color.a == 1 &&
                    otherObj.GetComponent<SpriteRenderer>().color.a == 1 &&
                    thisObj.transform.localScale.x > Config.scaleStart &&
                    otherObj.transform.localScale.x > Config.scaleStart
                    ) ||
                    otherObj.CompareTag("killmepls")
                    )
                {
                    thisObj.GetComponent<Tile>().Move(direction); //двигаем
                    //создаЄм пустоту там, где были
                    GameObject New = new GameObject();
                    New.tag = "killmepls";
                    posObj[x, y] = New;
                    posObj[x, y - 1] = thisObj;
                    moves++;
                }
            }
        if (moves == 0) direction = -1;
    }
}
