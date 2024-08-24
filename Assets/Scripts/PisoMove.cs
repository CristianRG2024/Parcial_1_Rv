using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PisoMove : MonoBehaviour
{
    private GameManager gameManager;
    private float floorSpeed;

    void Start()
    {
        gameManager= FindObjectOfType<GameManager>();
        floorSpeed = gameManager.velocidadNivel;
    }
    void Update()
    {
        floorSpeed = gameManager.velocidadNivel;
        transform.position -= new Vector3(0, 0, floorSpeed * Time.deltaTime);
    }
}
