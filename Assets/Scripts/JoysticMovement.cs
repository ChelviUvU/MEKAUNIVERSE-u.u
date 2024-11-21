using System;
using System.IO.Ports; // Necesario para la comunicación serial
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public string portName = "COM5"; // Puerto del Arduino
    public int baudRate = 9600; // Velocidad de comunicación serial
    private SerialPort serialPort;

    private int joystick1Value = 518; // Valor inicial del joystick 1 (reposo)
    private int joystick2Value = 515; // Valor inicial del joystick 2 (reposo)

    public float movementSpeed = 5.0f; // Velocidad de la nave
    private int threshold = 50; // Margen de tolerancia para evitar ruido (ajusta según pruebas)

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
        // Leer datos del puerto serial
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    string data = serialPort.ReadLine();
                    string[] values = data.Split(',');

                    if (values.Length == 2)
                    {
                        // Leer valores crudos del Arduino
                        joystick1Value = int.Parse(values[0]);
                        joystick2Value = int.Parse(values[1]);

                        Debug.Log($"Joystick1: {joystick1Value}, Joystick2: {joystick2Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error al leer datos: {ex.Message}");
            }
        }

        // Lógica de movimiento de la nave
        bool moveLeft = joystick1Value < threshold && joystick2Value < threshold;
        bool moveRight = joystick1Value > 1023 - threshold && joystick2Value > 1023 - threshold;

        if (moveLeft) // Ambos joysticks hacia la izquierda
        {
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
            Debug.Log("Moviendo a la izquierda");
        }
        else if (moveRight) // Ambos joysticks hacia la derecha
        {
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
            Debug.Log("Moviendo a la derecha");
        }
        else
        {
            Debug.Log("Sin movimiento");
        }
    }

    void OnDestroy()
    {
        // Cierra el puerto serial al salir
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
