using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour
{
    float leftToMove = 0;
    float moveStep = 0;
    float scaleStep = 0;
    float moveStepTime = 0;
    int millis = 0;
    float scaleStepDestr = 0;
    float rotateStep = 0;
    int destroyingDelay = 0;

    DateTime prev = new DateTime();
    DateTime countDown = new DateTime();
    public int direction = -1;


    public void Move(int Direction)
    {
        direction = Direction;
        leftToMove = Config.delta;
        moveStep = leftToMove / Config.moveFrameCount;
        moveStepTime = 1000 / (Config.averageFPS * (Config.averageFPS / Config.moveFrameCount));
        prev = DateTime.Now;
        //Debug.Log("tile " + name + " moving...");
    }

    public void Stop(int x, int y)
    {
        leftToMove = 0;
        direction = -1;
        transform.position = new Vector3(
            -Config.startpos + Config.delta * x,
            Config.startpos - Config.delta * y,
            -1
            );
    }

    public void Destroy(int delayTime)
    {
        destroyingDelay = delayTime;
        direction = -3;
        scaleStepDestr = (transform.localScale.x - Config.endScale) / Config.destroyingFrameCount;
        rotateStep = (Config.endRotation - transform.rotation.z) / Config.destroyingFrameCount;
        countDown = DateTime.Now;
    }

    void Start()
    {
        (FindObjectOfType(typeof(Controls)) as Controls).AddScore(int.Parse(tag));
        var scale = Config.scaleStart;
        transform.localScale = new Vector3(scale, scale, scale);
        scaleStep = (1 - scale) / Config.scaleFrameCount;
        if (gameObject.CompareTag("2048"))
        {
            (FindObjectOfType(typeof(ParticleController)) as ParticleController).ActivateParticles();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //движение
        //if (millis >= moveStepTime)
        if (direction >= 0)
        {
            for (; millis >= moveStepTime;)
            {
                millis -= (int)moveStepTime;
                if (leftToMove >= 0)
                {
                    switch (direction)
                    {
                        case 0:
                            if (leftToMove > moveStep)
                                transform.position = new Vector3(transform.position.x - moveStep, transform.position.y, transform.position.z);
                            else
                                transform.position = new Vector3(transform.position.x - leftToMove, transform.position.y, transform.position.z);
                            break;
                        case 1:
                            if (leftToMove > moveStep)
                                transform.position = new Vector3(transform.position.x, transform.position.y + moveStep, transform.position.z);
                            else
                                transform.position = new Vector3(transform.position.x, transform.position.y + leftToMove, transform.position.z);
                            break;
                        case 2:
                            if (leftToMove > moveStep)
                                transform.position = new Vector3(transform.position.x + moveStep, transform.position.y, transform.position.z);
                            else
                                transform.position = new Vector3(transform.position.x + leftToMove, transform.position.y, transform.position.z);
                            break;
                        case 3:
                            if (leftToMove > moveStep)
                                transform.position = new Vector3(transform.position.x, transform.position.y - moveStep, transform.position.z);
                            else
                                transform.position = new Vector3(transform.position.x, transform.position.y - leftToMove, transform.position.z);
                            break;
                    }
                    leftToMove -= moveStep;
                }
                else if (direction >= 0)
                {
                    (FindObjectOfType(typeof(Controls)) as Controls).ReportMove();
                    direction = -1;
                }
            }
        millis += (int)Math.Round((DateTime.Now - prev).TotalMilliseconds);
        prev = DateTime.Now;
    }

        //слияние (проверка)
        var collisions = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.5F, 0.5F), 0);
        for(int i = 0; i < collisions.Length; i++)
        {
            var col = collisions[i];
            if (col.gameObject == gameObject) continue;
            
            if (col.gameObject.GetComponent<Tile>().direction != -2)
            {
                if (col.gameObject.CompareTag(gameObject.tag))
                {
                    var prefab = (FindObjectOfType(typeof(Config)) as Config).GetPrefabByTag((int.Parse(gameObject.tag) * 2).ToString());
                    Vector3 offset = new Vector3(0, 0, 0);
                    switch(direction)
                    {
                        case 0:
                            offset = new Vector3(-leftToMove, 0, 0);
                            break;
                        case 1:
                            offset = new Vector3(0, leftToMove, 0);
                            break;
                        case 2:
                            offset = new Vector3(leftToMove, 0, 0);
                            break;
                        case 3:
                            offset = new Vector3(0, -leftToMove,  0);
                            break;
                    }
                    Instantiate(prefab, transform.position + offset, Quaternion.identity);
                    (FindObjectOfType(typeof(Controls)) as Controls).ReportMove();
                }
                
            }
            direction = -2;
            Destroy(gameObject);
        }

        //увеличение (переход)
        var scale = transform.localScale.x;
        if (scale < 1)
        {
            if (scale + scaleStep >= 1)
                scale = 1;
            else
                scale += scaleStep;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        //уничтожение
        destroyingDelay -= (int)((DateTime.Now - countDown).TotalMilliseconds) * (direction == -3 ? 1 : 0);
        countDown = DateTime.Now;
        if (direction == -3 && destroyingDelay <= 0)
        {
            transform.localScale -= new Vector3(scaleStepDestr, scaleStepDestr, 0);

            transform.rotation = new Quaternion(0, 0, transform.rotation.z + rotateStep, 1);

            if (transform.localScale.x <= Config.endScale)
                Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        
    }
}
