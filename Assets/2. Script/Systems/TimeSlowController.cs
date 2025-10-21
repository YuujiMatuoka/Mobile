using UnityEngine;

public class TimeSlowController : MonoBehaviour
{
    [Header("Configuración de Ralentización")]
    public float maxSlowTime = 5f; // Tiempo máximo de ralentización (5 segundos)
    public float slowTimeScale = 0.3f;
    public float normalTimeScale = 1f;
    public float regenerationRate = 1f; // Velocidad de regeneración por segundo

    private bool isSlowingTime = false;
    private float currentSlowTime;
    private bool canUseSlowTime = true;
    private bool mouseWasPressed = false; // Nuevo: para rastrear si el mouse estaba presionado

    void Start()
    {
        currentSlowTime = maxSlowTime;
    }

    void OnEnable()
    {
        // Suscribirse a los eventos del mouse
        MouseInputEventDispatcher.OnMousePressed += StartTimeSlow;
        MouseInputEventDispatcher.OnMouseReleased += StopTimeSlow;
    }

    void OnDisable()
    {
        // Desuscribirse de los eventos para evitar memory leaks
        MouseInputEventDispatcher.OnMousePressed -= StartTimeSlow;
        MouseInputEventDispatcher.OnMouseReleased -= StopTimeSlow;
    }

    void Update()
    {
        UpdateSlowTime();

        // Actualizar el estado del mouse
        mouseWasPressed = Input.GetMouseButton(0); // Asumiendo que el botón 0 es el izquierdo
    }

    void UpdateSlowTime()
    {
        if (isSlowingTime && canUseSlowTime)
        {
            // Consumir tiempo de ralentización
            currentSlowTime -= Time.unscaledDeltaTime;

            // Verificar si se agotó el tiempo
            if (currentSlowTime <= 0f)
            {
                currentSlowTime = 0f;
                canUseSlowTime = false;
                StopTimeSlow();
                Debug.Log("Tiempo de ralentización agotado");
            }
        }
        else if (!isSlowingTime && currentSlowTime < maxSlowTime && !mouseWasPressed) // MODIFICADO: Solo regenerar si el mouse no está presionado
        {
            // Regenerar tiempo de ralentización
            currentSlowTime += regenerationRate * Time.unscaledDeltaTime;
            currentSlowTime = Mathf.Clamp(currentSlowTime, 0f, maxSlowTime);

            // Si se regeneró suficiente tiempo, reactivar el uso
            if (currentSlowTime >= 0.5f && !canUseSlowTime)
            {
                canUseSlowTime = true;
                Debug.Log("Tiempo de ralentización disponible nuevamente");
            }
        }
    }

    void StartTimeSlow()
    {
        if (!isSlowingTime && canUseSlowTime && currentSlowTime > 0f)
        {
            Time.timeScale = slowTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            isSlowingTime = true;
            Debug.Log("Tiempo ralentizado - Tiempo restante: " + currentSlowTime.ToString("F1") + "s");
        }
        else if (!canUseSlowTime)
        {
            Debug.Log("No se puede usar ralentización - Tiempo agotado");
        }
    }

    void StopTimeSlow()
    {
        if (isSlowingTime)
        {
            Time.timeScale = normalTimeScale;
            Time.fixedDeltaTime = 0.02f * normalTimeScale;
            isSlowingTime = false;
            Debug.Log("Tiempo normal - Tiempo restante: " + currentSlowTime.ToString("F1") + "s");
        }
    }

    void OnGUI()
    {
        // Mostrar estado actual del tiempo y barra de progreso
        GUI.Label(new Rect(10, 10, 300, 20), "Estado: " + (isSlowingTime ? "RALENTIZADO" : "NORMAL"));
        GUI.Label(new Rect(10, 30, 300, 20), "Tiempo restante: " + currentSlowTime.ToString("F1") + "s / " + maxSlowTime + "s");

        // Barra de progreso del tiempo de ralentización
        float barWidth = 200f;
        float fillWidth = (currentSlowTime / maxSlowTime) * barWidth;

        // Fondo de la barra
        GUI.Box(new Rect(10, 50, barWidth, 20), "");

        // Relleno de la barra (verde cuando tiene tiempo, rojo cuando está agotado)
        Color fillColor = canUseSlowTime ? Color.green : Color.red;
        GUI.color = fillColor;
        GUI.Box(new Rect(10, 50, fillWidth, 20), "");
        GUI.color = Color.white;

        // Indicador de estado
        GUI.Label(new Rect(10, 75, 300, 20), "Disponible: " + (canUseSlowTime ? "SÍ" : "NO"));
        GUI.Label(new Rect(10, 95, 300, 20), "Mantén presionado para ralentizar el tiempo");
    }

    // Métodos públicos para consultar el estado (opcional, para otros scripts)
    public bool IsTimeSlowed()
    {
        return isSlowingTime;
    }

    public float GetCurrentSlowTime()
    {
        return currentSlowTime;
    }

    public float GetMaxSlowTime()
    {
        return maxSlowTime;
    }

    public bool CanUseSlowTime()
    {
        return canUseSlowTime;
    }
}