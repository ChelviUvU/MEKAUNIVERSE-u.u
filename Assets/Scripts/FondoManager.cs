using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FondoManager : MonoBehaviour
{
    public float velocidad;
    public Renderer fondo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fondo.material.mainTextureOffset = fondo.material.mainTextureOffset + new Vector2(0, velocidad) * Time.deltaTime;
    }
}
