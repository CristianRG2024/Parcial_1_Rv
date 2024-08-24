using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float boostAmount = 2f; // Aumento de velocidad (x2 en este caso)
    public float duration = 5f; // Duración del aumento de velocidad en segundos

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.s_gameManager.velocidadNivel *= boostAmount;
            StartCoroutine(ResetSpeedAfterTime(duration));
        }
    }

    IEnumerator ResetSpeedAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        GameManager.s_gameManager.velocidadNivel = GameManager.s_gameManager.velocidadLim;
    }
}
