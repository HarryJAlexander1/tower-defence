using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffects : MonoBehaviour
{
    private ParticleSystem MuzzleFlash;
    private ParticleSystem GunShells;
    private AudioSource GunSound;

    private void Awake()
    {
        MuzzleFlash = gameObject.transform.Find("FX_MuzzleFlash").GetComponent<ParticleSystem>();
        GunShells = gameObject.transform.Find("FX_Shells").GetComponent<ParticleSystem>();
        GunSound = GetComponentInParent<AudioSource>();
    }
    public void PlayGunEffects()
    {
        MuzzleFlash.Play();
        GunSound.Play();
        GunShells.Play();
    }
}
