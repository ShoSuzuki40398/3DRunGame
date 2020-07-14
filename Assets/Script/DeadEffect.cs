using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEffect : MonoBehaviour
{
    private ParticleSystem particle;

    // Update is called once per frame

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.Play();
    }

    void Update()
    {
        if(particle.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
