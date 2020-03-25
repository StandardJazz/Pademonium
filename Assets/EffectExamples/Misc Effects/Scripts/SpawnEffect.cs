using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{

    public float spawnEffectTime = 3;
    public float pause = 1;
    public AnimationCurve fadeIn;

    ParticleSystem ps;
    float timer = 0;
    Renderer _renderer;

    int shaderProperty;
    bool doOnce = false;

    void Start()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponent<Renderer>();
        ps = GetComponentInChildren<ParticleSystem>();

        var main = ps.main;
        main.duration = spawnEffectTime;
        ps.Stop();
    }

    void Update()
    {
        if (!doOnce && GameManager.player.GetComponent<AHeroes>().IsSpawning())
        {
            _renderer.enabled = true;
            doOnce = true;
            timer = 0.0f;
        }

        if (doOnce)
        {
            if (timer == 0.0f)
                ps.Play();

            if(ps.isPlaying)
                timer += Time.deltaTime;

            if (timer > 3.0f)
            {
                ps.Stop();
                _renderer.enabled = false;
                doOnce = false;
            }

            _renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
        }

    }
}
