using UnityEngine;
using TMPro;
using System.Collections;

public class BarrierController : MonoBehaviour
{
    public GameObject barrierObject; // Referencia al objeto de la barrera
    public TextMeshProUGUI cooldownText; // Referencia al texto del cooldown
    private bool canActivateBarrier = true; // Controla si la barrera puede activarse
    private float cooldownTime = 10f; // Tiempo de cooldown total
    private float currentCooldown = 0f; // Tiempo restante de cooldown

    void Start()
    {
        barrierObject.SetActive(false); // Desactivar la barrera al inicio
        UpdateCooldownText(); // Inicializar el texto del cooldown
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && canActivateBarrier)
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
}
