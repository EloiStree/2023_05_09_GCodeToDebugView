using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawPixelOnTextureMono : MonoBehaviour
{

    public int m_sizePixel = 2048;
    public Texture2D m_layer;
    public Renderer [] m_target;
    public float m_maxSize = 0.3f;
     Color [] c;
    Color b = new Color(0, 0, 0, 0);
    Color g = new Color(0, 1, 0, 1);
    // Start is called before the first frame update
    void Awake()
    {
        m_layer = new Texture2D(m_sizePixel, m_sizePixel);
        c = new Color[m_sizePixel * m_sizePixel];
        for (int i = 0; i < c.Length; i++)
            c[i] = b;
        m_layer.SetPixels(c);
        foreach (var item in m_target)
        {

            item.material.mainTexture = m_layer;
        }
    }

    public int m_currentX;
    public int m_CurrentY;
    public int m_previousX;
    public int m_previousy;

    public void PushVector(Vector3 localPoint) {

        int x = (int) ((localPoint.x / m_maxSize)* m_sizePixel);
        int y = (int) ((localPoint.z / m_maxSize)* m_sizePixel);

        x = Mathf.Clamp(x, 0, m_sizePixel-1);
        y = Mathf.Clamp(y, 0, m_sizePixel-1);
        c[y * m_sizePixel + x] = g;
        m_layer.SetPixels(c);
        m_layer.Apply();
    }
}
