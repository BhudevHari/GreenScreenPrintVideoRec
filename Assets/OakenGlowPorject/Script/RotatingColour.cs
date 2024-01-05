using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingColour : MonoBehaviour
{
    Color EmissionColor = new Color(1f, 0f, 0f);
    [SerializeField] private MeshRenderer m_Renderer;
    int Counter = 0;
    // Update is called once per frame
    void Update()
    {
        m_Renderer.material.SetColor("_EmissionColor", EmissionColor);
        EmissionColor = Color.HSVToRGB((Counter % 360f) / 360f, 96f / 100f, 22f / 100f);
        Counter %= 360;
        Counter+=2;
    }
}
