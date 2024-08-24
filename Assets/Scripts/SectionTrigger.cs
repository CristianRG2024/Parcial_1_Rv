using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public Transform pos;
    public GameObject[] pisosToInstantiate;
    private Vector3 posNewPiso;

    private void Start()
    {
        posNewPiso = new Vector3(pos.position.x, pos.position.y, pos.position.z+187.5f);
        //Debug.Log("posicion nueva" + posNewPiso);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("creadorPiso"))
        {
            //Debug.Log("Creando Piso");
            int n = Random.Range(0, pisosToInstantiate.Length);
            Instantiate(pisosToInstantiate[n], posNewPiso, transform.rotation);
        }
        if (other.gameObject.CompareTag("removedorPiso"))
        {
            //Debug.Log("Eliminando Piso: " + this.gameObject.transform.parent.gameObject.name);
            Destroy(this.gameObject.transform.parent.gameObject);
        }
    }  
}
