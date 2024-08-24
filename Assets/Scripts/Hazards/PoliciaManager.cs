using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliciaManager : MonoBehaviour
{
    [SerializeField] private float velMovimiento = 3.0f;
    Vector3 posicionOriginal;
    [SerializeField] Transform posicionAparicion;
    Vector3 posicionDestino;
    Animator animator;
    [SerializeField] GameObject policia;
    [SerializeField] AudioClip silbato;
    [SerializeField] AudioClip salsa;

    void Start()
    {
        posicionOriginal = policia.transform.localPosition;
        posicionDestino = posicionOriginal;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        policia.transform.localPosition = Vector3.Slerp(policia.transform.localPosition, posicionDestino, velMovimiento * Time.deltaTime);
    }
    public void aparecer() {
        //Debug.Log("Policia apareció");
        posicionDestino = posicionAparicion.localPosition;
        Camera.main.GetComponent<AudioSource>().PlayOneShot(silbato);
    }
    public void desaparecer() {
        //Debug.Log("Policia desapareció");
        posicionDestino = posicionOriginal;
    }
    public void atraparPlayer() {
        Camera.main.GetComponent<AudioSource>().clip = salsa;
        Camera.main.GetComponent<AudioSource>().Play();
        animator.SetTrigger("atraparPlayer");
        posicionDestino = new Vector3(posicionAparicion.localPosition.x, posicionAparicion.localPosition.y, posicionAparicion.localPosition.z-0.5f);
    }
}
