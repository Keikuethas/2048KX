using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem[] particles = new ParticleSystem[0];
    public Sprite[] sprites = new Sprite[0];
    public string particlesTag = "particles";
    // Start is called before the first frame update
    void Start()
    {
        if (particles.Length == 0)
        {
            var objs = GameObject.FindGameObjectsWithTag(particlesTag);
            particles = new ParticleSystem[objs.Length];
            for(int i = 0; i < objs.Length; i++)
                particles[i] = objs[i].GetComponent<ParticleSystem>();
        }
        if (sprites.Length == 0) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSprite(int spriteID)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            var sp = particles[i].shape;
            sp.spriteRenderer.sprite = sprites[spriteID];
        }
    }

    public void ChangeSprite(int particleID, int spriteID)
    {
        var sp = particles[particleID].shape;
        sp.spriteRenderer.sprite = sprites[spriteID];
    }

    public void ActivateParticles()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].gameObject.SetActive(false);
            particles[i].gameObject.SetActive(true);
        }
    }

    public void ActivateParticles(int particleID)
    {
        particles[particleID].gameObject.SetActive(false);
        particles[particleID].gameObject.SetActive(true);
    }
}
