using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AssistantInteraction : MonoBehaviour
{
    public static AssistantInteraction assistantInstance;

    public Camera arCamera;              // Cámara AR
    public Transform pjParaPruebas;
    public Transform lockPosition;       // Objeto vacío hijo de la cámara que define la posición de bloqueo
    public float heightOffset = 1.0f;    // Altura del asistente respecto al dispositivo
    public Canvas worldSpaceCanvas;      // Canvas hijo del asistente

    //Movimiento
    public bool isFollowing = true;     // Estado para controlar si sigue a la cámara o está bloqueado
    private Vector3 targetPosition;      // Posición objetivo del asistente
    [Header("Movement")]
    public float smoothSpeed = 2.0f;     // Velocidad de interpolación para suavizar el movimiento
    public float followRadius = 4.0f;    // Radio al que sigue el asistente

    //Aleatoriedad
    private float timeToChangePosition = 5; // Tiempo restante para cambiar la posición
    private float angle;
    private float xOffset;
    private float zOffset;
    [Header("Randomnes")]
    public float minWaitTime = 5.0f;   // Tiempo mínimo para cambiar de posición
    public float maxWaitTime = 10.0f;   // Tiempo máximo para cambiar de posición

    // Utilidades
    [Header("Utility")]
    public float waitToStartWelcomeAudio=3;
    private AudioSource audioSource;

    void Awake() {
        assistantInstance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(IniciarAudio());

        // Asegurarse de que el Canvas esté desactivado al inicio
        if (worldSpaceCanvas != null)
        {
            worldSpaceCanvas.gameObject.SetActive(false);
        }

        // Inicializar el tiempo para cambiar de posición aleatoriamente
        SetRandomTimeToChangePosition();
    }

    void Update()
    {
        if (isFollowing)
        {
            // Si el tiempo ha pasado, asignar una nueva posición objetivo
            if (timeToChangePosition <= 0)
            {
                // Asignar una nueva posición aleatoria alrededor del usuario
                SetRandomAngleAroundUser();

                // Reiniciar el tiempo para el próximo cambio de posición
                SetRandomTimeToChangePosition();
            }
            else
            {
                // Reducir el tiempo restante para cambiar la posición
                timeToChangePosition -= Time.deltaTime;
            }

            // Seguir al usuario
            ChaseUser();
        }

        // Movimiento
        transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // Detectar toque en pantalla al asistente
        if (Application.isEditor)
        {
            // Orientación
            transform.LookAt(pjParaPruebas.transform);

            // Detectar el clic en el asistente
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Golpe: " + hit.collider.gameObject.name);
                    if (hit.transform.parent != null) // Revisar si el objeto golpeado tiene un padre
                    {
                        GameObject parent = hit.transform.parent.gameObject;
                        if (parent.CompareTag("Asistente")) // Si el padre es el asistente
                        {
                            ToggleAssistantState();
                        }
                    }
                }
            }
        }
        else
        {
            // Orientación
            transform.LookAt(arCamera.transform);

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Solo detectar el toque cuando el estado es "Began", que es cuando el usuario toca la pantalla
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("Golpe: " + hit.collider.gameObject.name);
                        if (hit.transform.parent != null) // Revisar si el objeto golpeado tiene un padre
                        {
                            GameObject parent = hit.transform.parent.gameObject;
                            if (parent.CompareTag("Asistente")) // Si el padre es el asistente
                            {
                                ToggleAssistantState();
                            }
                        }
                    }
                }
            }
        }
    }

    // Cambia entre seguir la cámara y bloquearse en una posición
    public void ToggleAssistantState()
    {
        if (isFollowing)
        {
            // Mover el asistente a la posición de bloqueo
            LockAssistant();
        }
        else
        {
            // Volver a seguir a la cámara
            FollowUser();
        }
    }

    // Bloquear al asistente en una posición fija
    void LockAssistant()
    {
        isFollowing = false;

        // Establecer nueva posición del asistente a la posición de bloqueo (definida por el objeto lockPosition)
        targetPosition = lockPosition.position;

        // Activar el Canvas si no está activo
        if (worldSpaceCanvas != null && !worldSpaceCanvas.gameObject.activeSelf)
        {
            worldSpaceCanvas.gameObject.SetActive(true);
        }
    }

    // Hacer que el asistente siga al usuario
    void FollowUser()
    {
        isFollowing = true;

        // Desactivar el Canvas si está activo
        if (worldSpaceCanvas != null && worldSpaceCanvas.gameObject.activeSelf)
        {
            worldSpaceCanvas.gameObject.SetActive(false);
        }
    }

    void ChaseUser() {

        // Seguir al usuario
        if (Application.isEditor)
        {
            targetPosition = new Vector3(
                pjParaPruebas.transform.position.x + xOffset,
                pjParaPruebas.transform.position.y + heightOffset,
                pjParaPruebas.transform.position.z + zOffset
            );
        }
        else {
            targetPosition = new Vector3(
                arCamera.transform.position.x + xOffset,
                arCamera.transform.position.y + heightOffset,
                arCamera.transform.position.z + zOffset
            );
        }
    }

    // Asigna una nueva posición aleatoria alrededor del usuario dentro de un radio determinado
    void SetRandomAngleAroundUser()
    {
        // Generar un ángulo aleatorio en radianes
        angle = Random.Range(0f, Mathf.PI * 2);

        // Calcular una posición aleatoria en un círculo alrededor de la cámara
        float randomRadius = Random.Range(-followRadius, followRadius);
        xOffset = Mathf.Cos(angle) * randomRadius;
        zOffset = Mathf.Sin(angle) * randomRadius;
    }

    // Asigna un tiempo aleatorio para cambiar de posición nuevamente
    void SetRandomTimeToChangePosition()
    {
        timeToChangePosition = Random.Range(minWaitTime, maxWaitTime);
    }

    // Reproducción de audios

    // Audio por zonas (Interacción desde script TriggerFunction)
    public void playAudio(string audioName)
    {

        // Cargar el AudioClip desde la carpeta Resources
        AudioClip newClip = Resources.Load<AudioClip>(audioName);

        // Si se encuentra el AudioClip, cambiarlo en el AudioSource y reproducir
        if (newClip != null)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioClip " + audioName + " no encontrado.");
        }
    }

    // Audio inicial
    IEnumerator IniciarAudio() {
        AudioClip newClip = Resources.Load<AudioClip>("Audio_Bienvenida");
        audioSource.clip = newClip;
        yield return new WaitForSeconds(waitToStartWelcomeAudio);
        audioSource.Play();
    }

}
