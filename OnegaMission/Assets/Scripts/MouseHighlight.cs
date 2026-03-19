using EPOOutline;
using UnityEngine;

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