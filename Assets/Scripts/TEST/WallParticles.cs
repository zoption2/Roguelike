using Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class WallParticles : MonoBehaviour, IMyPoolable
{
    [SerializeField]
    public ParticleSystem Particles;

    public void Play()
    {
        Particles.Play();
    }
    public void OnCreate()
    {
    }

    public void OnPull()
    {
    }

    public void OnRelease()
    {
        Particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
