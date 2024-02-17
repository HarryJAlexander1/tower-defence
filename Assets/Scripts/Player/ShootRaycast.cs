using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class ShootRaycast : MonoBehaviour
{
    GameObject GameManagerObject;
    GameManager GameManager;
    private float RaycastDistance;
    GunEffects GunEffects;
    public ParticleSystem HitEffect;
    // Start is called before the first frame update
    void Awake()
    {
        RaycastDistance = 100f;
        GameManagerObject = GameObject.Find("Game Manager");
        GameManager = GameManagerObject.GetComponent<GameManager>();
        GunEffects = gameObject.GetComponentInChildren<GunEffects>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireRaycast(false);
        }
        else if (Input.GetMouseButtonDown(1)) 
        {
            FireRaycast(true);
        }
    }

    private void FireRaycast(bool rightClickPressed) 
    {
        Vector3 raycastOrigin = transform.position;
        Vector3 raycastDirection = transform.forward;

        // Create a Ray from the origin and direction
        Ray ray = new Ray(raycastOrigin, raycastDirection);
        RaycastHit hitInfo;
        if (!GameManager.IsAttackSequence && !GameManager.AgentExists)
        {      
            if (Physics.Raycast(ray, out hitInfo, RaycastDistance))
            {
                ManageBlocks(hitInfo, rightClickPressed);
            }      
        }
        else
        {
            ManageGunEffects();

            if (Physics.Raycast(ray, out hitInfo, RaycastDistance))
            {
                ManageHitOnEnemy(hitInfo);
                Instantiate(HitEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));   
            }          
        } 
    }

    private void ManageGunEffects() 
    {
        GunEffects.PlayGunEffects();
    }

    private void ManageHitOnEnemy(RaycastHit hitInfo) 
    {
        if (hitInfo.collider.gameObject.CompareTag("AgentBody")) 
        {
            AgentBehaviour agentBehaviour = hitInfo.collider.gameObject.GetComponentInParent<AgentBehaviour>();
            agentBehaviour.Hitpoints--;
            Debug.Log(agentBehaviour.Hitpoints);
        }
    }
    private void ManageBlocks(RaycastHit hitInfo, bool removeBlock) 
    {
        if (removeBlock)
        {
            if (hitInfo.collider.gameObject.CompareTag("Block"))
            {
                GameManager.RemoveBlock(hitInfo.point, hitInfo.collider.gameObject);
                GameManager.Fund += 5;
            }
        }
        else
        {
            if (hitInfo.collider.gameObject.CompareTag("Floor") && GameManager.Fund >= 10)
            {
                GameManager.PlaceBlockOnNearestEmptyVertex(hitInfo.point);
                GameManager.Fund -= 10;
            }
        }
    }
}
