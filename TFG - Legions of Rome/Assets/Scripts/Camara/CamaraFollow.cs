using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    public Transform target; // El objeto que la cámara seguirá

    public float smoothSpeed = 5f; // Velocidad de suavizado
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Desplazamiento de la cámara respecto al objetivo

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset; // Posición deseada de la cámara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); // Suavizado de la posición
        transform.position = smoothedPosition; // Actualizar la posición de la cámara
    }
}
