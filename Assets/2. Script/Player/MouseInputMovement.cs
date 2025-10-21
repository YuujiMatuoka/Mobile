using System;
using UnityEngine;

public class MouseInputMovement : MonoBehaviour
{
    //[Header("Mouse Drag Settings")]
    //public float maxDragDistance = 5f;
    //public float forceMultiplier = 10f;

    //public static event Action<Vector2> OnForceApplied;

    //private Vector2 dragStartPosition;
    //private Vector2 dragCurrentPosition;
    //private bool isDragging = false;
    //private Camera mainCamera;
    //private LineRenderer dragLine;

    //void Start()
    //{
    //    mainCamera = Camera.main;

    //    // Configurar LineRenderer para visualizar el arrastre
    //    dragLine = gameObject.AddComponent<LineRenderer>();
    //    dragLine.material = new Material(Shader.Find("Sprites/Default"));
    //    dragLine.startColor = Color.blue;
    //    dragLine.endColor = Color.red;
    //    dragLine.startWidth = 0.1f;
    //    dragLine.endWidth = 0.05f;
    //    dragLine.positionCount = 2;
    //    dragLine.enabled = false;

    //    // Suscribirse a los eventos del mouse
    //    MouseInputEventDispatcher.OnMousePressed += StartDrag;
    //    MouseInputEventDispatcher.OnMouseReleased += EndDrag;
    //}

    //void OnDestroy()
    //{
    //    // Desuscribirse de los eventos para evitar memory leaks
    //    MouseInputEventDispatcher.OnMousePressed -= StartDrag;
    //    MouseInputEventDispatcher.OnMouseReleased -= EndDrag;
    //}

    //void Update()
    //{
    //    if (isDragging)
    //    {
    //        UpdateDrag();
    //    }
    //}

    //void StartDrag()
    //{
    //    isDragging = true;
    //    dragStartPosition = GetWorldMousePosition();
    //    dragLine.enabled = true;
    //    dragLine.SetPosition(0, transform.position);
    //    dragLine.SetPosition(1, transform.position);
    //}

    //void UpdateDrag()
    //{
    //    dragCurrentPosition = GetWorldMousePosition();

    //    // Actualizar la línea de arrastre
    //    dragLine.SetPosition(0, transform.position);
    //    dragLine.SetPosition(1, dragCurrentPosition);

    //    // Limitar la distancia máxima de arrastre
    //    float dragDistance = Vector2.Distance(transform.position, dragCurrentPosition);
    //    if (dragDistance > maxDragDistance)
    //    {
    //        Vector2 direction = (dragCurrentPosition - (Vector2)transform.position).normalized;
    //        dragCurrentPosition = (Vector2)transform.position + direction * maxDragDistance;
    //        dragLine.SetPosition(1, dragCurrentPosition);
    //    }
    //}

    //void EndDrag()
    //{
    //    if (isDragging)
    //    {
    //        isDragging = false;
    //        dragLine.enabled = false;

    //        // Calcular la fuerza basada en el arrastre
    //        Vector2 force = CalculateForce();

    //        // Disparar el evento con la fuerza calculada
    //        OnForceApplied?.Invoke(force);
    //    }
    //}

    //Vector2 CalculateForce()
    //{
    //    // La fuerza es en dirección opuesta al arrastre (como una honda)
    //    Vector2 direction = (Vector2)transform.position - dragCurrentPosition;
    //    float distance = Vector2.Distance(transform.position, dragCurrentPosition);

    //    // Normalizar y aplicar multiplicador de fuerza
    //    Vector2 force = direction.normalized * (distance * forceMultiplier);

    //    Debug.Log("Fuerza aplicada: " + force + " | Magnitud: " + force.magnitude);
    //    return force;
    //}

    //Vector2 GetWorldMousePosition()
    //{
    //    Vector3 mousePos = Input.mousePosition;
    //    mousePos.z = -mainCamera.transform.position.z;
    //    return mainCamera.ScreenToWorldPoint(mousePos);
    //}

    //void OnGUI()
    //{
    //    if (isDragging)
    //    {
    //        Vector2 forcePreview = CalculateForce();
    //        GUI.Label(new Rect(10, 200, 300, 20), "Fuerza preview: " + forcePreview.magnitude.ToString("F2"));
    //        GUI.Label(new Rect(10, 220, 300, 20), "Dirección: " + forcePreview.normalized.ToString());
    //    }
    //}


    //--------------------------------------------------------------------------------------------------------------------------


    [Header("Mouse Drag Settings")]
    public float maxDragDistance = 5f;
    public float forceMultiplier = 10f;

    public static event Action<Vector2> OnForceApplied;

    private Vector2 dragStartPosition;
    private Vector2 dragCurrentPosition;
    private Vector2 dragReleasePosition;
    private bool isDragging = false;
    private Camera mainCamera;
    private LineRenderer dragLine;

    void Start()
    {
        mainCamera = Camera.main;

        // Configurar LineRenderer para visualizar el arrastre
        dragLine = gameObject.AddComponent<LineRenderer>();
        dragLine.material = new Material(Shader.Find("Sprites/Default"));
        dragLine.startColor = Color.blue;
        dragLine.endColor = Color.red;
        dragLine.startWidth = 0.1f;
        dragLine.endWidth = 0.05f;
        dragLine.positionCount = 2;
        dragLine.enabled = false;

        // Suscribirse a los eventos del mouse
        MouseInputEventDispatcher.OnMousePressed += StartDrag;
        MouseInputEventDispatcher.OnMouseReleased += EndDrag;
    }

    void OnDestroy()
    {
        // Desuscribirse de los eventos para evitar memory leaks
        MouseInputEventDispatcher.OnMousePressed -= StartDrag;
        MouseInputEventDispatcher.OnMouseReleased -= EndDrag;
    }

    void Update()
    {
        if (isDragging)
        {
            UpdateDrag();
        }
    }

    void StartDrag()
    {
        isDragging = true;
        dragStartPosition = GetWorldMousePosition();
        dragLine.enabled = true;

        // La línea ahora va desde el punto de click hasta la posición actual del mouse
        dragLine.SetPosition(0, dragStartPosition);
        dragLine.SetPosition(1, dragStartPosition);
    }

    void UpdateDrag()
    {
        dragCurrentPosition = GetWorldMousePosition();

        // Actualizar la línea de arrastre (desde el punto inicial hasta la posición actual del mouse)
        dragLine.SetPosition(0, dragStartPosition);
        dragLine.SetPosition(1, dragCurrentPosition);

        // Limitar la distancia máxima de arrastre desde el punto inicial
        float dragDistance = Vector2.Distance(dragStartPosition, dragCurrentPosition);
        if (dragDistance > maxDragDistance)
        {
            Vector2 direction = (dragCurrentPosition - dragStartPosition).normalized;
            dragCurrentPosition = dragStartPosition + direction * maxDragDistance;
            dragLine.SetPosition(1, dragCurrentPosition);
        }
    }

    void EndDrag()
    {
        if (isDragging)
        {
            isDragging = false;
            dragReleasePosition = GetWorldMousePosition();
            dragLine.enabled = false;

            // Calcular la fuerza basada en la distancia entre el punto inicial y final
            Vector2 force = CalculateForce();

            // Disparar el evento con la fuerza calculada
            OnForceApplied?.Invoke(force);
        }
    }

    Vector2 CalculateForce()
    {
        // Calcular la dirección y distancia del arrastre
        Vector2 dragDirection = dragReleasePosition - dragStartPosition;
        float dragDistance = Vector2.Distance(dragStartPosition, dragReleasePosition);

        // Limitar la distancia máxima
        dragDistance = Mathf.Min(dragDistance, maxDragDistance);

        // La fuerza es en dirección OPUESTA al arrastre (como una honda)
        // Si arrastras hacia la derecha, el objeto va hacia la izquierda
        Vector2 forceDirection = -dragDirection.normalized;

        // Calcular la fuerza (distancia * multiplicador)
        Vector2 force = forceDirection * (dragDistance * forceMultiplier);

        Debug.Log("Fuerza aplicada: " + force +
                 " | Dirección arrastre: " + dragDirection.normalized +
                 " | Dirección fuerza: " + forceDirection +
                 " | Distancia: " + dragDistance.ToString("F2"));

        return force;
    }

    Vector2 GetWorldMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    void OnGUI()
    {
        if (isDragging)
        {
            // Calcular preview de fuerza basada en la posición actual
            Vector2 currentDragDirection = dragCurrentPosition - dragStartPosition;
            float currentDragDistance = Mathf.Min(Vector2.Distance(dragStartPosition, dragCurrentPosition), maxDragDistance);
            Vector2 forcePreview = -currentDragDirection.normalized * (currentDragDistance * forceMultiplier);

            GUI.Label(new Rect(10, 200, 300, 20), "Fuerza preview: " + forcePreview.magnitude.ToString("F2"));
            GUI.Label(new Rect(10, 220, 300, 20), "Dirección: " + forcePreview.normalized.ToString());
            GUI.Label(new Rect(10, 240, 300, 20), "Distancia arrastre: " + currentDragDistance.ToString("F2"));

            // Dibujar línea de ayuda para visualizar mejor
            DrawDragHelper();
        }
    }

    void DrawDragHelper()
    {
        // Dibujar un círculo en el punto de inicio
        DrawCircle(dragStartPosition, 0.2f, Color.green);

        // Dibujar un círculo en la posición actual
        DrawCircle(dragCurrentPosition, 0.1f, Color.yellow);
    }

    void DrawCircle(Vector2 position, float radius, Color color)
    {
        int segments = 12;
        float angle = 0f;

        for (int i = 0; i < segments; i++)
        {
            Vector2 start = position + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            angle += 2 * Mathf.PI / segments;
            Vector2 end = position + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

            Debug.DrawLine(start, end, color);
        }
    }
}