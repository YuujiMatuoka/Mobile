using UnityEngine;
using System;

public class MouseInputEventDispatcher : MonoBehaviour
{
    // Eventos estáticos para que puedan ser accedidos desde cualquier clase
    public static event Action OnMousePressed;
    public static event Action OnMouseReleased;

    private bool wasMouseDown = false;

    void Update()
    {
        // Detectar si el mouse está presionado actualmente
        bool isMouseDown = Input.GetMouseButton(0);

        // Disparar evento cuando el mouse es presionado
        if (isMouseDown && !wasMouseDown)
        {
            OnMousePressed?.Invoke();
        }
        // Disparar evento cuando el mouse es soltado
        else if (!isMouseDown && wasMouseDown)
        {
            OnMouseReleased?.Invoke();
        }

        // Actualizar el estado anterior
        wasMouseDown = isMouseDown;
    }
}