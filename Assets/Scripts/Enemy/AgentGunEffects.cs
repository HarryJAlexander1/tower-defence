using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGunEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem GunOneFlash;
    [SerializeField] private ParticleSystem GunTwoFlash;
    [SerializeField] private AudioSource GunSound;
    private bool GunOneFired = false;
    // Start is called before the first frame update

    public void PlayGunEffects()
    {
        if (GunOneFired)
        {            
            GunTwoFlash.Play();
            GunOneFired = false;
        }  
        else
        {
            GunOneFlash.Play();
            GunOneFired = true;
        }
        GunSound.Play();
    }
}

