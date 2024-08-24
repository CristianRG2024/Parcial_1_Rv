using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pantallaInicio : MonoBehaviour
{
    public TMP_Text textoLeaderboard;
    private scoreManager scoreManager;

    private void Start()
    {
        scoreManager = scoreManager.s_ScoreManager;
        if (scoreManager.checkLeaderboard()!="") {
            textoLeaderboard.text = scoreManager.checkLeaderboard();
        }
    }

    public void cargarEscenaJuego() {
        if (scoreManager.setPersonalData())
        {
            SceneManager.LoadScene(1);
        }
        else {
            Debug.Log("No hay datos para el registro, imposible iniciar");
        }
    }
    }
