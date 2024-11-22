using System;
using System.IO.Ports;
using UnityEngine;

public class ArduinoController : MonoBehaviour
{
    public string portName = "COM5"; // Puerto del Arduino
    public int baudRate = 9600; // Velocidad de comunicación serial
    private SerialPort serialPort;

    public GameObject projectilePrefab; // Prefab del proyectil
    public float movementSpeed = 5.0f; // Velocidad de movimiento

    private int joystick1Value = 518; // Valor inicial del joystick 1 (reposo)
    private int joystick2Value = 515; // Valor inicial del joystick 2 (reposo)
    private int fireValue1 = 1; // Estado del botón de disparo
    private int fireValue2 = 1; // Estado del botón de disparo

    private int threshold = 50; // Margen de tolerancia para evitar ruido

    public float fireDelay = 0.5f; // Tiempo mínimo entre disparos (en segundos)
    private float lastFireTime = 0; // Momento del último disparo

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 10;
            Debug.Log("Puerto serial abierto correctamente.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al abrir el puerto serial: {e.Message}");
        }
    }

    void Update()
    {
        ReadArduinoData();
        HandleMovement();
        HandleShooting();
    }

    void ReadArduinoData()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    string data = serialPort.ReadLine();
                    string[] values = data.Split(',');

                    if (values.Length == 4)
                    {
                        joystick1Value = int.Parse(values[0]);
                        joystick2Value = int.Parse(values[1]);
                        fireValue1 = int.Parse(values[2]);
                        fireValue2 = int.Parse(values[3]);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error al leer datos: {ex.Message}");
            }
        }
    }

    void HandleMovement()
    {
        bool moveLeft = joystick1Value < threshold && joystick2Value < threshold;
        bool moveRight = joystick1Value > 1023 - threshold && joystick2Value > 1023 - threshold;

        if (moveLeft)
        {
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }
        else if (moveRight)
        {
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        }
    }

    void HandleShooting()
    {
        if(Time.time - lastFireTime > fireDelay)
        {
            if (fireValue1 == 0 && Time.time - lastFireTime > fireDelay) // Verificar el delay
            {
                Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                lastFireTime = Time.time; // Actualizar el tiempo del último disparo
            }
            if (fireValue2 == 0 && Time.time - lastFireTime > fireDelay) // Verificar el delay
            {
                Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                lastFireTime = Time.time; // Actualizar el tiempo del último disparo
            }
        }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
