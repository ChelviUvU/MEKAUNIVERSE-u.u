using System;
using System.IO.Ports;
using UnityEngine;
using TMPro;
using System.Collections;

public class ArduinoController : MonoBehaviour
{
    public string portName = "COM5"; // Puerto del Arduino
    public int baudRate = 9600; // Velocidad de comunicación serial
    private SerialPort serialPort;

    public GameObject projectilePrefab; // Prefab del proyectil
    public GameObject barrierObject; // Referencia al objeto de la barrera
    public TextMeshProUGUI cooldownText; // Texto del cooldown de la barrera
    public float movementSpeed = 5.0f; // Velocidad de movimiento

    private int joystick1Value = 518; // Valor inicial del joystick izquierdo
    private int joystick2Value = 515; // Valor inicial del joystick derecho
    private int fireValue1 = 1; // Estado del botón de disparo 1
    private int fireValue2 = 1; // Estado del botón de disparo 2

    private int threshold = 50; // Margen de tolerancia para evitar ruido

    public float fireDelay = 0.5f; // Tiempo mínimo entre disparos (en segundos)
    private float lastFireTime = 0; // Momento del último disparo

    private float cooldownTime = 10f; // Tiempo de cooldown de la barrera
    private float currentCooldown = 0f; // Tiempo restante de cooldown
    private bool canActivateBarrier = true; // Controla si la barrera puede activarse

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

        // Inicializar la barrera
        barrierObject.SetActive(false);
        UpdateCooldownText();
    }

    void Update()
    {
        ReadArduinoData();
        HandleMovement();
        HandleShooting();
        HandleBarrier();
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
        if (Time.time - lastFireTime > fireDelay)
        {
            if (fireValue1 == 0 || fireValue2 == 0) // Disparo con cualquier botón
            {
                Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                lastFireTime = Time.time; // Actualizar el tiempo del último disparo

                // Enviar comando al Arduino para activar el LED
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Write("F"); // Comando para destello del LED
                }
            }
        }
    }

    void HandleBarrier()
    {
        // Activar la barrera si ambos joysticks se mueven hacia adentro
        bool activateBarrier = joystick1Value > 1023 - threshold && joystick2Value < threshold;

        if (activateBarrier && canActivateBarrier)
        {
            StartCoroutine(ActivateBarrier());
        }
    }

    IEnumerator ActivateBarrier()
    {
        canActivateBarrier = false; // Deshabilitar activación durante el cooldown
        barrierObject.SetActive(true); // Activar la barrera
        UpdateCooldownText("Activada"); // Mostrar que la barrera está activa
        yield return new WaitForSeconds(5f); // Esperar 5 segundos con la barrera activa

        barrierObject.SetActive(false); // Desactivar la barrera
        StartCoroutine(StartCooldown()); // Iniciar el cooldown
    }

    IEnumerator StartCooldown()
    {
        currentCooldown = cooldownTime; // Establecer el cooldown inicial
        while (currentCooldown > 0)
        {
            UpdateCooldownText($"{currentCooldown:F1} seg"); // Actualizar el texto con tiempo restante
            yield return new WaitForSeconds(0.1f); // Actualizar cada 0.1 segundos
            currentCooldown -= 0.1f;
        }
        canActivateBarrier = true; // Permitir activar de nuevo
        UpdateCooldownText("Lista"); // Mostrar que el cooldown terminó
    }

    void UpdateCooldownText(string text = "")
    {
        if (cooldownText != null)
        {
            cooldownText.text = string.IsNullOrEmpty(text) ? "Lista" : $"Barrera: {text}";
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
