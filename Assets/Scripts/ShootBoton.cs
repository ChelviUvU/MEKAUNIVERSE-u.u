using System;
using System.IO.Ports; // Necesario para la comunicación serial
using UnityEngine;

public class ArduinoController : MonoBehaviour
{
    public string portName = "COM5"; // Puerto del Arduino
    public int baudRate = 9600; // Velocidad de comunicación serial
    private SerialPort serialPort;

    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform firePoint; // Punto de origen del disparo
    public float projectileSpeed = 10f; // Velocidad del proyectil

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 10;
            Debug.Log("Puerto serial abierto correctamente para disparo.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al abrir el puerto serial: {e.Message}");
        }
    }

    void Update()
    {
        // Leer datos del puerto serial
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    string data = serialPort.ReadLine().Trim(); // Leer línea desde Arduino

                    if (data == "FIRE") // Si se recibe el comando "FIRE"
                    {
                        Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error al leer datos de disparo: {ex.Message}");
            }
        }
    }

   

    void OnDestroy()
    {
        // Cerrar el puerto serial al destruir el objeto
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
