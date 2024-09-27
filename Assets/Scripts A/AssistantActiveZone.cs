using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistantActiveZone : MonoBehaviour
{
    AssistantInteraction assistant;

    private void Start()
    {
        assistant = AssistantInteraction.assistantInstance;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            if (!assistant.isFollowing)
            {
                assistant.ToggleAssistantState();
            }
        }
    }
}
