using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectController : MonoBehaviour
{
    [SerializeField]
    private GameObject destroyTarget = null;

    private void OnParticleSystemStopped()
    {
        if(destroyTarget == null)
        {
            destroyTarget = gameObject;
        }
        Destroy(destroyTarget);
    }
}
