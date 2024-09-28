using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AssistantInteraction : MonoBehaviour
{
    public static AssistantInteraction assistantInstance;

    public Camera arCamera;              // C�mara AR
    public Transform pjParaPruebas;
    public float heightOffset = 1.0f;    // Altura del asistente respecto al dispositivo
    public Canvas worldSpaceCanvas;      // Canvas hijo del asistente

    //Movimiento
    public bool isFollowing = true;     // Estado para controlar si sigue a la c�mara o est� bloqueado
    private Vector3 targetPosition;      // Posici�n objetivo del asistente
    [Header("Movement")]
    public float smoothSpeed = 2.0f;     // Velocidad de interpolaci�n para suavizar el movimiento
    public float followRadius = 2.0f;    // Radio al que sigue el asistente
    public float maxDistanceBetween = 4.0f; // Distancia m�xima con el usuario

    //Aleatoriedad
    private float timeToChangePosition = 5; // Tiempo restante para cambiar la posici�n
    private float angle;
    private float xOffset = 1.0f;
    private float zOffset = 1.0f;
    [Header("Randomnes")]
    public float minWaitTime = 5.0f;   // Tiempo m�nimo para cambiar de posici�n
    public float maxWaitTime = 10.0f;   // Tiempo m�ximo para cambiar de posici�n

    // Utilidades
    [Header("Utility")]
    public float waitToStartWelcomeAudio=3;
    public Text textUI;
    [HideInInspector] public string actualZone;
    private AudioSource audioSource;

    void Awake() {
        assistantInstance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(IniciarAudio());

        // Asegurarse de que el Canvas est� desactivado al inicio
        if (worldSpaceCanvas != null)
        {
            worldSpaceCanvas.gameObject.SetActive(false);
        }

        // Inicializar el tiempo para cambiar de posici�n aleatoriamente
        SetRandomTimeToChangePosition();
    }

    void Update()
    {
        if (isFollowing)
        {
            // Si el tiempo ha pasado, asignar una nueva posici�n objetivo
            if (timeToChangePosition <= 0)
            {
                // Asignar una nueva posici�n aleatoria alrededor del usuario
                SetRandomAngleAroundUser();

                // Reiniciar el tiempo para el pr�ximo cambio de posici�n
                SetRandomTimeToChangePosition();
            }
            else
            {
                // Reducir el tiempo restante para cambiar la posici�n
                timeToChangePosition -= Time.deltaTime;
            }

            // Seguir al usuario
            ChaseUser();
        }
        else {
            if (Application.isEditor)
            {
                if (Vector3.Distance(transform.position, pjParaPruebas.transform.position)>maxDistanceBetween)
                { 
                    ToggleAssistantState();
                }
            }
            else {
                if (Vector3.Distance(transform.position, arCamera.transform.position) > maxDistanceBetween)
                {
                    ToggleAssistantState();
                }
            }
        }

        // Movimiento
        transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // Detectar toque en pantalla al asistente
        if (Application.isEditor)
        {
            // Orientaci�n
            transform.LookAt(pjParaPruebas.transform);

            // Detectar el clic en el asistente
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Golpe: " + hit.collider.gameObject.name);
                    if (hit.collider.CompareTag("Asistente")) {
                        ToggleAssistantState();
                    }
                }
            }
        }
        else
        {
            // Orientaci�n
            transform.LookAt(arCamera.transform);

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Solo detectar el toque cuando el estado es "Began", que es cuando el usuario toca la pantalla
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = arCamera.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("Golpe: " + hit.collider.gameObject.name);
                        if (hit.collider.CompareTag("Asistente"))
                        {
                            ToggleAssistantState();
                        }
                    }
                }
            }
        }
    }

    // Cambia entre seguir la c�mara y bloquearse en una posici�n
    public void ToggleAssistantState()
    {
        if (isFollowing)
        {
            // Mover el asistente a la posici�n de bloqueo
            LockAssistant();
        }
        else
        {
            // Volver a seguir a la c�mara
            FollowUser();
        }
    }

    // Bloquear al asistente en una posici�n fija
    void LockAssistant()
    {
        isFollowing = false;

        // Establecer posici�n del asistente a su �ltima posici�n
        targetPosition = transform.position;

        // Activar el Canvas si no est� activo
        if (worldSpaceCanvas != null && !worldSpaceCanvas.gameObject.activeSelf)
        {
            worldSpaceCanvas.gameObject.SetActive(true);
        }
    }

    // Hacer que el asistente siga al usuario
    void FollowUser()
    {
        isFollowing = true;

        // Desactivar el Canvas si est� activo
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

    // Asigna una nueva posici�n aleatoria alrededor del usuario dentro de un radio determinado
    void SetRandomAngleAroundUser()
    {
        // Generar un �ngulo aleatorio en radianes
        angle = Random.Range(0f, Mathf.PI * 2);

        // Calcular una posici�n aleatoria en un c�rculo alrededor de la c�mara
        float randomRadius = Random.Range(-1, 1);

        if (randomRadius <= 0)
        {
            xOffset = Mathf.Cos(angle) * (-followRadius);
            zOffset = Mathf.Sin(angle) * (-followRadius);
        }
        else {
            xOffset = Mathf.Cos(angle) * (followRadius);
            zOffset = Mathf.Sin(angle) * (followRadius);
        }
    }

    // Asigna un tiempo aleatorio para cambiar de posici�n nuevamente
    void SetRandomTimeToChangePosition()
    {
        timeToChangePosition = Random.Range(minWaitTime, maxWaitTime);
    }

    // Reproducci�n de audios

    // Audio inicial
    IEnumerator IniciarAudio()
    {
        AudioClip newClip = Resources.Load<AudioClip>("Audio_Bienvenida");
        audioSource.clip = newClip;
        yield return new WaitForSeconds(waitToStartWelcomeAudio);
        audioSource.Play();
    }

    // Audio por zonas (Interacci�n desde script TriggerFunction)
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

    // Audio m�s info
    public void playMoreInfoAudio()
    {
        // Cargar el AudioClip desde la carpeta Resources
        AudioClip newClip = Resources.Load<AudioClip>(actualZone + "_MoreInfo");

        // Si se encuentra el AudioClip, cambiarlo en el AudioSource y reproducir
        if (newClip != null)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioClip (" + actualZone + "_MoreInfo" + ") no encontrado.");
        }
    }





    // Interfaz de usuario
    public void actualiceCanvas(string zoneName)
    {
        string[] zona = zoneName.Split('_');
        textUI.text = $"En este momento estamos en: {zona[1]}";
    }


}
