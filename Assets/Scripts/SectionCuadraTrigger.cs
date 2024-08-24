using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SectionCuadraTrigger : MonoBehaviour
{
    public Transform pos;
    public GameObject[] edificiosToInstantiate;
    private Vector3 posNewEdificio;
    //private float offset = 10f; // Desplazamiento para evitar colisión

    void Start()
    {
        posNewEdificio = new Vector3(pos.position.x, pos.position.y, pos.position.z + 187.5f);
        //Debug.Log("Posicion Edificio" + posNewEdificio );
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("creadorEdificios"))
        {
            //Debug.Log("Creando Edificio");
            int n = Random.Range(0, edificiosToInstantiate.Length);
            Instantiate(edificiosToInstantiate[n], posNewEdificio, transform.rotation);

            //Vector3 posAjustada = AjustarPosicionParaNoSolapar(posNewEdificio);
            //Instantiate(edificiosToInstantiate[n], posAjustada, transform.rotation);
        }
        if (other.gameObject.CompareTag("removedorEdificios"))
        {
            //Debug.Log("Eliminando Edificio: " + this.gameObject.transform.parent.gameObject.name);
            Destroy(this.gameObject.transform.parent.gameObject);
        }
    }

    //private Vector3 AjustarPosicionParaNoSolapar(Vector3 posicionOriginal)
    //{
    //    Vector3 nuevaPosicion = posicionOriginal;
    //    Collider[] colliders;

    //    do
    //    {
    //        colliders = Physics.OverlapSphere(nuevaPosicion, 1f); // Verificar colisiones en un radio de 1 unidad
    //        bool solapado = false;

    //        foreach (Collider col in colliders)
    //        {
    //            if (col.gameObject.CompareTag("carretera"))
    //            {
    //                solapado = true;
    //                nuevaPosicion.x += offset; // Desplazar en el eje X
    //                break;
    //            }
    //        }

    //        if (!solapado)
    //        {
    //            break;
    //        }

    //    } while (true);

    //    return nuevaPosicion;
    //}
}