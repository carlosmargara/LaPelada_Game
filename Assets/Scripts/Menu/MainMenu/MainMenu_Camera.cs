using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Camera : MonoBehaviour
{
    [SerializeField] private float speedRot = 30f;

    void Update()
    {
        transform.Rotate(0, speedRot * Time.deltaTime, 0);
    }
}
