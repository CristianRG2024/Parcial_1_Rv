using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variables de Funcionamiento")]
    public Transform carrilesParent; //Grupo de carriles para moverse
    public float velHorizontal; //Velocidad de cambio de Carril
    public float jumpForce; // Fuerza de salto
    public float landForce; // Fuerza de aterrizaje
    [SerializeField] private Transform jumpRaycast;
    [SerializeField] private float gravity = -9.81f;

    [HideInInspector] public bool isAlive;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public int carrilAntesDelMov;
    [HideInInspector] public Transform[] carrilesDisponibles;
    [HideInInspector] public Transform carrilPosicionActual; //Actualiza la posicion del personaje

    private int carrilActualIndex; //carriles 0, 1 y 2
    private bool isJumping; // Estado de saltar
    private bool isFalling; // Estado de caer en el aire

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        carrilesDisponibles = new Transform[carrilesParent.childCount];
        for (int i = 0; i < carrilesParent.childCount; i++)
        {
            carrilesDisponibles[i] = carrilesParent.GetChild(i);
        }
    }
    private void Start()
    {
        carrilActualIndex = 1;
        carrilAntesDelMov = carrilActualIndex;
        carrilPosicionActual = carrilesDisponibles[carrilActualIndex];
        transform.position = carrilPosicionActual.position;
        isAlive = true;
        isJumping = false;
        isFalling = false;
    }

    void Update()
    {
        if (isAlive)
        {
            //Cambio de Carril Izquierda
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (carrilActualIndex > 0)
                {
                    carrilAntesDelMov = carrilActualIndex;
                    carrilActualIndex--;
                }
                carrilPosicionActual = carrilesDisponibles[carrilActualIndex];
            }
            //Cambio de Carril Derecha
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (carrilActualIndex < carrilesDisponibles.Length - 1)
                {
                    carrilAntesDelMov = carrilActualIndex;
                    carrilActualIndex++;
                }
                carrilPosicionActual = carrilesDisponibles[carrilActualIndex];
            }
            //Establecer nueva posicion X pero mantener posición Y y Z del personaje (Evitar interferencia con salto)
            Vector3 newPos = new Vector3(carrilPosicionActual.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, velHorizontal * Time.deltaTime);

            //Debug.DrawRay(jumpRaycast.position, Vector3.down * 0.35f, Color.red);
            //Debug.Log(Physics.Raycast(jumpRaycast.position, Vector3.down, 0.35f));
            if (Physics.Raycast(jumpRaycast.position, Vector3.down, 0.35f))
            {
                //Debug.Log("En Suelo");
                isJumping = false;
                isFalling = false;
                animator.SetBool("Jump", false);
                animator.SetBool("Fall", false);
            }
            else
            {
                isFalling = true;
                if (!isJumping)
                {
                    animator.SetBool("Fall", true);
                }
            }
            if (Input.GetKeyDown(KeyCode.Space) && !isFalling)
            {
                //Debug.Log("Oprimido salto");
                isJumping = true;
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                animator.SetBool("Jump", true);
            }
            if (Input.GetKeyDown(KeyCode.S) && isFalling)
            {
                rb.AddForce(Vector3.down * landForce, ForceMode.Impulse);
            }
        }
        else {
            Vector3 groundPos = new Vector3(transform.position.x, 0.1f, transform.position.z);
            transform.position = Vector3.Slerp(transform.position, groundPos, 2.3f*Time.deltaTime);
        }
    }

    //FixedUpdate para físicas de caida del salto más realistas
    void FixedUpdate()
    {
        rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }
}