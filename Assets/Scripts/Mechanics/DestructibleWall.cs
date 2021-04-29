﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DestructibleWall : MonoBehaviour, IDamagable
{
    [SerializeField] int healthPoints = 3;

    public int MaxHealthPoints => 0;

    public int HealthPoints => healthPoints;

    public event WallEvent OnWallDestroyed;
    public delegate void WallEvent();

    PoolVfxs particleDestruction;

    public void Awake()
    {
        particleDestruction = GameObject.Find("VFXsBuildExplo(Pool)").GetComponent<PoolVfxs>();
    }

    public void TakeDamage()
    {
        healthPoints--;

        // Activar las partículas

        if (healthPoints <= 0)
            Die();
    }

    void Die()
    {

        ParticleSystem destruction = particleDestruction.GetItem(transform.position, tag);

        OnWallDestroyed?.Invoke();

        // Desactivar los renderer de los hijos
        foreach (Renderer render in GetComponentsInChildren<Renderer>())
            render.enabled = false;


        // Desactivar los renderer y el collider
        GetComponent<Renderer>().enabled = false;

        foreach (Collider collider in GetComponents<Collider>())
            collider.enabled = false;

        DynamicGridObstacle dynamic = GetComponentInChildren<DynamicGridObstacle>();

        // Updatear el grid
        dynamic.DoUpdateGraphs();
        // Desactivar el collider
        foreach (Collider collider in dynamic.gameObject.GetComponents<Collider>())
            collider.enabled = false;

        // Encender el collider de los escombros
        transform.GetChild(0).gameObject.SetActive(true);
        Collider trigger;

        if (transform.GetChild(0).TryGetComponent(out trigger))
            trigger.enabled = true;
        else
            for (int i = 0; i < transform.GetChild(0).childCount; i++)
                transform.GetChild(0).GetChild(i).GetComponent<Collider>().enabled = true;
    }
}

