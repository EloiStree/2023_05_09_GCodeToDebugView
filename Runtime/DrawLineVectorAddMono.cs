using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineVectorAddMono : MonoBehaviour
{
    private List<Vector3> m_worldPosition= new List<Vector3>();

    public LineRenderer m_lineRenderer;

    public void PushNewPosition(Vector3 worldPosition) {
        m_worldPosition.Add(worldPosition);
        m_lineRenderer.positionCount++;
        m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, worldPosition);
       
    }
}
