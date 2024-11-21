using System;
using System.IO.Ports; // Necesario para comunicación serial
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public string portName = "COM3"; // Cambia al puerto correspondiente
    public int baudRate = 9600; // Debe coincidir con el del Arduino
    private SerialPort serialPort;

    private float joystick1Value = 0;
    private float joystick2Value = 0;

    public float movementSpeed = 5.0f; // Velocidad de movimiento de la nave

    void Start()
    {
        try
        {
            // Abre el puerto serial
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 100; // Tiempo de espera para la lectura
            Debug.Log("Puerto Serial abierto correctamente");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al abrir el puerto serial: {e.Message}");
        }
    }

    void Update()
    {
        // Leer datos del Arduino
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine(); // Leer línea del puerto serial
                Debug.Log($"Datos recibidos: {data}"); // Muestra los datos en consola

                string[] values = data.Split(','); // Divide los datos por comas
                if (values.Length == 2)
                {
                    // Convierte los valores de texto a flotantes y mapea de 0-1023 a -1 a 1
                    joystick1Value = Map(float.Parse(values[0]), 0, 1023, -1f, 1f);
                    joystick2Value = Map(float.Parse(values[1]), 0, 1023, -1f, 1f);

                    Debug.Log($"Joystick1: {joystick1Value}, Joystick2: {joystick2Value}");
                }
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("Tiempo de espera excedido al leer el puerto serial.");
            }
        }

        // Mover la nave
        if (joystick1Value > 0.5f && joystick2Value > 0.5f) // Mover a la derecha
        {
            Debug.Log("Moviendo a la derecha");
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        }
        else if (joystick1Value < -0.5f && joystick2Value < -0.5f) // Mover a la izquierda
        {
            Debug.Log("Moviendo a la izquierda");
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }
    }

    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
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
