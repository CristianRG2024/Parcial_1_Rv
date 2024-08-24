using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerMovement player;
    private PoliciaManager policia;
    private GameManager gameManager;
    private bool siendoPerseguido;
    [SerializeField] private float tiempoDePersecucion = 5.0f;
    [SerializeField] private string tagComparar = "Moneda";
    float tiempoTranscurridoPersecucion;

    [SerializeField] private AudioClip ClickMoneda;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        policia = FindObjectOfType<PoliciaManager>();
        gameManager = FindObjectOfType<GameManager>();
        siendoPerseguido = false;
        tiempoTranscurridoPersecucion = 0;
    }
    void Update()
    {
        if (siendoPerseguido)
        {
            tiempoTranscurridoPersecucion += Time.deltaTime;
        }
        else {
            tiempoTranscurridoPersecucion = 0;
        }

        if (tiempoTranscurridoPersecucion>=tiempoDePersecucion && player.isAlive) {
            policia.desaparecer();
            siendoPerseguido = false;
        }
    }
    public void golpeAturdimiento()
    {
        if (player.isAlive)
        {
            if (!siendoPerseguido)
            {
                //Debug.Log("Auch, eso dolió");

                player.animator.SetTrigger("Golpe");
                player.carrilPosicionActual = player.carrilesDisponibles[player.carrilAntesDelMov];
                policia.aparecer();
                siendoPerseguido = true;
            }
            else {
                golpeFatal();
            }
        }
    }
    public void golpeFatal()
    {
        gameManager.gameOver();
        player.animator.SetTrigger("Muerte");
        player.isAlive = false;
        policia.atraparPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tagComparar))
        {
            Destroy(other.gameObject);
            gameManager.recogerMoneda();
            Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickMoneda);
        }
    }
}
