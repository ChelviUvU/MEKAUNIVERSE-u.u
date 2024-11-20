using System.IO.Ports;
using UnityEngine;

public class ArduinoInput : MonoBehaviour
{
    // Puerto serial para la comunicación con el Arduino
    SerialPort serialPort = new SerialPort("COM6", 9600); // Ajusta el puerto COM según sea necesario

    // Variables para controlar el movimiento de la nave
    public float moveSpeed = 5f;  // Velocidad de movimiento de la nave
    private Vector3 moveDirection;  // Dirección de movimiento

    void Start()
    {
        // Abre el puerto serial
        try
        {
            serialPort.Open();
            Debug.Log("Conexión al Arduino establecida");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("No se pudo abrir el puerto serial: " + ex.Message);
        }
    }

    void Update()
    {
        // Verifica si hay datos disponibles en el puerto
        if (serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            try
            {
                // Lee la línea completa desde el puerto serial
                string data = serialPort.ReadLine();
                Debug.Log("Datos recibidos: " + data);  // Verificar los datos recibidos

                // Divide los datos en un array usando la coma como separador
                string[] values = data.Split(',');

                if (values.Length >= 1) // Asegúrate de que haya al menos 1 valor (el comando de movimiento)
                {
                    // Recibe el comando para mover la nave (deberías tener un valor como "derecha", "izquierda" o "no mover")
                    string movementCommand = values[0];
                    Debug.Log("Comando de movimiento: " + movementCommand);  // Verificar el comando recibido

                    // Mover la nave dependiendo del comando recibido
                    if (movementCommand == "derecha")
                    {
                        moveDirection = Vector3.right;  // Dirección hacia la derecha
                    }
                    else if (movementCommand == "izquierda")
                    {
                        moveDirection = Vector3.left;   // Dirección hacia la izquierda
                    }
                    else
                    {
                        moveDirection = Vector3.zero;   // No moverse
                    }
                }
                else
                {
                    Debug.LogWarning("Datos no válidos recibidos: " + data);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error al procesar los datos: " + ex.Message);
            }
        }

        // Mover la nave basándonos en la dirección calculada
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void OnDestroy()
    {
        // Cierra el puerto serial al salir
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
