using UnityEngine;

[ExecuteAlways]
public class KeyUseZone : MonoBehaviour
{
    [SerializeField] private string requiredKeyID;
    private static string currentZoneKeyID = null;

    public static bool PlayerIsInKeyZone => currentZoneKeyID != null;
    public static string CurrentKeyZoneID => currentZoneKeyID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentZoneKeyID = requiredKeyID;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentZoneKeyID = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.yellow;

        // Aplicar la matriz del transform actual
        Gizmos.matrix = transform.localToWorldMatrix;

        // Dibujar el cubo en el espacio local del objeto
        Gizmos.DrawWireCube(box.center, box.size);

#if UNITY_EDITOR
        // Desactivamos la matriz para no afectar otras cosas
        Gizmos.matrix = Matrix4x4.identity;

        // Etiqueta (esta sí en mundo global)
        Vector3 worldCenter = transform.TransformPoint(box.center);
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(worldCenter + Vector3.up * 0.5f, $"Requiere llave: {requiredKeyID}");
#endif
    }
#endif

}
