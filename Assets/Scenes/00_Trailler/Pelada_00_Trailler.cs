using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pelada_00_Trailler : MonoBehaviour
{
    public Transform puntoPrevio;       // Punto donde se queda esperando
    public Transform camaraObjetivo;    // Posición final hacia la cámara
    public float velocidadPrevio = 5f;  // Velocidad hasta el punto previo
    public float velocidadSalto = 50f;  // Velocidad del salto hacia cámara

    private bool moviendoAPrevio = true;
    private bool saltando = false;

    void Update()
    {
        if (moviendoAPrevio)
        {
            MoverHacia(puntoPrevio.position, velocidadPrevio, () =>
            {
                moviendoAPrevio = false; // Se queda esperando
            });
        }
        else if (saltando)
        {
            MoverHacia(camaraObjetivo.position, velocidadSalto, () =>
            {
                saltando = false; // Llegó a la cámara
            });
        }
    }

    void MoverHacia(Vector3 destino, float velocidad, System.Action alLlegar)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destino,
            velocidad * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, destino) < 0.01f)
        {
            alLlegar?.Invoke();
        }
    }

    public void IniciarSalto()
    {
        saltando = true;
    }
}
