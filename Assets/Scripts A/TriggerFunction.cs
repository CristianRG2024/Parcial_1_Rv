using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerFunction : MonoBehaviour
{
    AssistantInteraction assistant;

    private void Start()
    {
        assistant = AssistantInteraction.assistantInstance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            assistant.actualZone = this.name;
            assistant.playAudio(this.name);
            assistant.actualiceCanvas(this.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            assistant.actualiceCanvas("Zona_ZonaCom�n");
        }
    }

}
