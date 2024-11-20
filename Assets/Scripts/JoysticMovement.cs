using System.IO.Ports;
using UnityEngine;

public class ArduinoInput : MonoBehaviour
{
    // Puerto serial para la comunicaci�n con el Arduino
    SerialPort serialPort = new SerialPort("COM6", 9600); // Ajusta el puerto COM seg�n sea necesario

    // Variables para controlar el movimiento de la nave
    public float moveSpeed = 5f;  // Velocidad de movimiento de la nave
    private Vector3 moveDirection;  // Direcci�n de movimiento

    void Start()
    {
        // Abre el puerto serial
        try
        {
            serialPort.Open();
            Debug.Log("Conexi�n al Arduino establecida");
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
                // Lee la l�nea completa desde el puerto serial
                string data = serialPort.ReadLine();
                Debug.Log("Datos recibidos: " + data);  // Verificar los datos recibidos

                // Divide los datos en un array usando la coma como separador
                string[] values = data.Split(',');

                if (values.Length >= 1) // Aseg�rate de que haya al menos 1 valor (el comando de movimiento)
                {
                    // Recibe el comando para mover la nave (deber�as tener un valor como "derecha", "izquierda" o "no mover")
                    string movementCommand = values[0];
                    Debug.Log("Comando de movimiento: " + movementCommand);  // Verificar el comando recibido

                    // Mover la nave dependiendo del comando recibido
                    if (movementCommand == "derecha")
                    {
                        moveDirection = Vector3.right;  // Direcci�n hacia la derecha
                    }
                    else if (movementCommand == "izquierda")
                    {
                        moveDirection = Vector3.left;   // Direcci�n hacia la izquierda
                    }
                    else
                    {
                        moveDirection = Vector3.zero;   // No moverse
                    }
                }
                else
                {
                    Debug.LogWarning("Datos no v�lidos recibidos: " + data);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error al procesar los datos: " + ex.Message);
            }
        }

        // Mover la nave bas�ndonos en la direcci�n calculada
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
