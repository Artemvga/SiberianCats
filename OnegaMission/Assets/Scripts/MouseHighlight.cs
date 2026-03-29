using EPOOutline;
using UnityEngine;

// -----------------------------------------------------------------------------
// Назначение файла: MouseHighlight.cs
// Путь: Assets/Scripts/MouseHighlight.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `MouseHighlight` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class MouseHighlight : MonoBehaviour
{
    private Outlinable outlinable;

    void Start()
    {
        outlinable = GetComponent<Outlinable>();
        if (outlinable == null)
        {
            outlinable = gameObject.AddComponent<Outlinable>();
            outlinable.RenderStyle = RenderStyle.FrontBack; 
            outlinable.FrontParameters.Color = Color.green;
            outlinable.BackParameters.Color = Color.red; 
            outlinable.OutlineParameters.DilateShift = 0.8f;
        }
        outlinable.OutlineParameters.DilateShift = 2f;
    }

    void OnMouseEnter()
    {
        outlinable.enabled = true;
    }

    void OnMouseExit()
    {
        outlinable.enabled = false;
    }
}