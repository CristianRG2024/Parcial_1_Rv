using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float boostAmount = 2f; // Aumento de velocidad (x2 en este caso)
    public float duration = 5f; // Duración del aumento de velocidad en segundos
    private float velActual;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!GameManager.s_gameManager.isBoosting) {
                velActual = GameManager.s_gameManager.velocidadNivel;
                GameManager.s_gameManager.velocidadNivel *= boostAmount;
                StartCoroutine(ResetSpeedAfterTime(duration));
            }
        }
    }

    IEnumerator ResetSpeedAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        GameManager.s_gameManager.velocidadNivel = velActual;
        GameManager.s_gameManager.isBoosting = false;
    }
}
