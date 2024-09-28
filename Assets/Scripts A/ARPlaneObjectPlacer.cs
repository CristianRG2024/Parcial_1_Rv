using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaneObjectPlacer : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;    // Referencia al ARPlaneManager
    public ARAnchorManager arAnchorManager;  // Referencia al ARAnchorManager
    public GameObject objectToPlacePrefab;   // Prefab del objeto 3D a generar

    private List<ARAnchor> anchors = new List<ARAnchor>();  // Lista para almacenar anclas creadas

    void Start()
    {
        // Suscribirnos al evento cuando se detecten nuevos planos
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    void OnDestroy()
    {
        // Desuscribirnos del evento cuando se destruya el objeto
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    // Evento que se llama cada vez que hay un cambio en los planos detectados
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Revisar si hay planos recién agregados
        if (args.added != null && args.added.Count > 0)
        {
            // Iteramos sobre los nuevos planos detectados
            foreach (ARPlane plane in args.added)
            {
                if (plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    // Llamar a la función para generar el modelo 3D en el plano
                    PlaceObjectOnPlane(plane);
                    break;  // Colocar el modelo solo en el primer plano detectado
                }
            }
        }
    }

    // Función que genera un objeto 3D sobre un plano detectado
    private void PlaceObjectOnPlane(ARPlane plane)
    {
        // Crear un Pose basado en la posición y rotación del plano
        Pose pose = new Pose(plane.transform.position, plane.transform.rotation);

        // Crear un ancla en el plano usando el Pose calculado
        ARAnchor anchor = arAnchorManager.AttachAnchor(plane, pose);

        if (anchor != null)
        {
            // Instanciar el objeto 3D sobre la posición del ancla
            Vector3 pos = new Vector3(anchor.transform.position.x, anchor.transform.position.y, anchor.transform.position.z+15);
            GameObject placedObject = Instantiate(objectToPlacePrefab, pos, anchor.transform.rotation);

            // Hacer que el objeto 3D sea hijo del ancla para que siga su posición
            placedObject.transform.SetParent(anchor.transform);

            // Guardar el ancla en la lista
            anchors.Add(anchor);
        }
    }

    // Opcional: Limpiar las anclas y objetos colocados
    public void ClearAnchors()
    {
        foreach (ARAnchor anchor in anchors)
        {
            Destroy(anchor.gameObject);  // Destruir el ancla y sus objetos hijos
        }
        anchors.Clear();  // Limpiar la lista de anclas
    }
}

