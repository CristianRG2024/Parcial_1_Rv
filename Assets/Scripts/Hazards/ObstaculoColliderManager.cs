using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoColliderManager : MonoBehaviour
{
    private PlayerManager player;
    private GameManager gameManager;
    public TriggerChoque colliderFrontal, colliderDerecho, colliderIzquierdo;

    private void Awake()
    {
        colliderFrontal.onCollisionEnter.AddListener(onColliderFrontalHit);
        colliderDerecho.onCollisionEnter.AddListener(onColliderDerechoHit);
        colliderIzquierdo.onCollisionEnter.AddListener(onColliderIzquierdoHit);

        player = FindObjectOfType<PlayerManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    public void onColliderFrontalHit() {
        player.golpeFatal();
    }
    public void onColliderDerechoHit()
    {
        player.golpeAturdimiento();
    }
    public void onColliderIzquierdoHit()
    {
        player.golpeAturdimiento();
    }
}
