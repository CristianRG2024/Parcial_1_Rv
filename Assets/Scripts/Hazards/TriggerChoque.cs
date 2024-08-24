using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerChoque : MonoBehaviour
{
    public UnityEvent onCollisionEnter;
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Colisionador activado: " + this.gameObject.transform.parent.gameObject.name + " - " + this.gameObject.name);
            //Debug.Log("Cuerpo golpeado: " + collision.gameObject.name);
            onCollisionEnter.Invoke();
        }
    }
}
