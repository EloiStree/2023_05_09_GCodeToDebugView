using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNCRotationMono : MonoBehaviour
{
    public Transform m_whatToRotate;
    public float m_rotationRate=2000;
    [Range(0f,1f)]
    public float m_powerPercent;
    public bool m_isClockWise;
    public bool m_isSpindleOn;


    public void Update()
    {
        if (m_isSpindleOn) { 
        m_whatToRotate.Rotate(
            new Vector3(
                m_powerPercent*m_rotationRate *
                Time.deltaTime *
               0, (m_isClockWise?1:-1), 0)
            , Space.Self);
        }
    }

    public void SetPowerAsPercent(float percent)
    {

        m_powerPercent = percent;
    }
    public void SetPowerOnOff(bool setOn)
    {

        m_isSpindleOn = setOn;
    }
    public void SetClockWise(bool setClockWise)
    {

        m_isClockWise = setClockWise;
    }
}
