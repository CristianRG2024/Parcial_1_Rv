using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager s_gameManager;

    [Header("Variables de Funcionamiento de velocidad")]
    public float velocidadNivel;
    public float velocidadLim;
    public float factorReduccionVelocidad;
    public bool isBoosting;
    [Header("Variables de Funcionamiento Cámara")]
    [SerializeField] private float distanciaRetroceso = 1.8f;
    [SerializeField] private float velMovCam = 5f;
    private Vector3 newPosCamera;
    [Header("Canvas")]
    [SerializeField] private GameObject canvasGameOver;
    [SerializeField] private GameObject canvasLeaderboard;
    [SerializeField] private TMP_Text textoPuntuaciones;

    //Variables de uso interno
    float tiempoPartida;
    int monedasRecogidas;
    private bool gameRunning;
    scoreManager leaderboard;

    private void Awake()
    {
        s_gameManager = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        leaderboard = scoreManager.s_ScoreManager;
    }
    void Start()
    {
        canvasGameOver.SetActive(false);
        canvasLeaderboard.SetActive(false);

        gameRunning = true;
        isBoosting = false;
        tiempoPartida = 0;
        monedasRecogidas = 0;

        newPosCamera = new Vector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, Camera.main.transform.localPosition.z - distanciaRetroceso);
    }
    private void FixedUpdate()
    {
        if (gameRunning)
        {
            tiempoPartida += Time.deltaTime;
            //Debug.Log("tiempo: " + Mathf.Floor(tiempoPartida));
            if (velocidadNivel <= velocidadLim)
            {
                velocidadNivel += Time.deltaTime / factorReduccionVelocidad;
            }
        }
        else
        {
            Camera.main.transform.localPosition = Vector3.Slerp(Camera.main.transform.localPosition, newPosCamera, velMovCam * Time.deltaTime);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void gameOver()
    {
        //Debug.Log("Perdiste");
        gameRunning = false;
        velocidadLim = 0;
        velocidadNivel = 0;
        canvasGameOver.SetActive(true);

        // Actualizar leaderboard
        leaderboard.setData(monedasRecogidas, tiempoPartida);
        leaderboard.saveGameData();
        cargarPuntajes();
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    // ------------------- CANVAS
    public void showHideLeaderboard()
    {
        canvasLeaderboard.SetActive(!canvasLeaderboard.activeSelf); 
    }
    public void showHideCanvasGameOver()
    {
        canvasGameOver.SetActive(!canvasGameOver.activeSelf);
    }

    // ------------------- METODOS DE FUNCIONAMIENTO DE SCORE
    public void recogerMoneda() {
        monedasRecogidas += 1;
    }

    public void cargarPuntajes() {
        textoPuntuaciones.text = leaderboard.checkLeaderboard();
    }
}