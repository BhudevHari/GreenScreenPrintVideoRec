using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotatingColourImage : MonoBehaviour
{
    [SerializeField] private Image m_Image;
    int Counter = 0;
    // Update is called once per frame
    void Update()
    {
        m_Image.color = Color.HSVToRGB((Counter % 360f) / 360f, 22f / 100f, 98f / 100f);
        Counter %= 360;
        Counter += 2;
    }
}
