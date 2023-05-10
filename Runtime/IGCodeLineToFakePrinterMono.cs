using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static IGCodeLineToFakePrinterMono;

public class IGCodeLineToFakePrinterMono : MonoBehaviour
{
    public Transform m_headToMove;
    

    public float m_laserPower;
    public float m_feedRate;
    public float m_extrudeStateDebug;

    public enum MoveType { Relative, Absolute}
    public MoveType m_moveType;

    public bool m_toolStateOnOff;
    public UEBool m_toolStateOnOffEvent;
    public float m_toolPowerPercent255;
    public UEFloat m_toolPowerPercent255Event;
    public UEVector3 m_newWorldPosition;
    public List<string> m_wasNotHandleProperly= new List<string>();

    [System.Serializable]
    public class UEBool : UnityEvent<bool> { }
    [System.Serializable]
    public class UEFloat : UnityEvent<float> { }

    [System.Serializable]
    public class UEVector3 : UnityEvent<Vector3> { }

    public Vector3 m_lastCoordinate;
    public void Interpret(IGCodeLine codeLine) {

        bool isHandleByTheCode = false;
        if (codeLine is RawGcodeLine )
        {
            RawGcodeLine g = (RawGcodeLine)codeLine;
            if (IsCommand(g, "G", 91))
            {
                m_moveType = MoveType.Relative;
                isHandleByTheCode = true;
            }
            if (IsCommand(g, "G", 90))
            {
                m_moveType = MoveType.Absolute;
                isHandleByTheCode = true;
            }
            if (IsCommand(g, "G", 28) )
            {
                
                SetToolStateAndNotify(false);
                isHandleByTheCode = true;

                 m_headPosition=Vector3.zero;
                m_headToMove.localPosition = Vector3.zero;
                m_newWorldPosition.Invoke(m_headToMove.position);

                if (g.HasToken())
                {
                    isHandleByTheCode = false;
                    WasNotHandleProperly(codeLine);
                    return;
                }
            }
            if (IsCommand(g, "M", 5))
            {
                SetToolStateAndNotify(false);
                isHandleByTheCode = true;
            }
            if (IsCommand(g, "M", 3))
            {
                if (g.GetValueOf("O", out RawGcodeLineToken value))
                {
                    value.GetValueAs(out double v);
                    m_toolPowerPercent255 =(float) v;
                    m_toolPowerPercent255Event.Invoke((float)(v / 255.0));
                }
                isHandleByTheCode = true;
            }

            if (g.m_commandType.m_char == "G" &&
                (g.m_commandType.m_value == 1 || g.m_commandType.m_value == 0))
            {


                if (g.m_commandType.m_value == 0)
                {
                    m_toolStateOnOff = false;
                    m_toolStateOnOffEvent.Invoke(m_toolStateOnOff);
                }


                Vector3 coordinate = new Vector3(); RawGcodeLineToken value;
                if (g.GetValueOf("X", out value))
                {
                    value.GetValueAs(out double v);
                    coordinate.x = (float)(v / 1000.0f);
                }
                else coordinate.x = float.MaxValue;

                if (g.GetValueOf("Y", out value))
                {
                    value.GetValueAs(out double v);
                    coordinate.z = (float)(v / 1000.0f);
                }
                else coordinate.z = float.MaxValue;
                if (g.GetValueOf("Z", out value))
                {
                    value.GetValueAs(out double v);
                    coordinate.y = (float)(v / 1000.0f);
                }
                else
                    coordinate.y = float.MaxValue;
                if (g.GetValueOf("E", out value))
                {
                    value.GetValueAs(out double v);
                    m_extrudeStateDebug = (float)(v / 1000.0f);
                }
                if (g.GetValueOf("S", out value))
                {
                    value.GetValueAs(out double v);
                    m_laserPower = (float)(v);
                }
                if (g.GetValueOf("F", out value))
                {
                    value.GetValueAs(out double v);
                    m_feedRate = (float)(v);
                }

                if (m_moveType == MoveType.Relative)
                {

                    if (coordinate.x != float.MaxValue)
                        m_headPosition.x += coordinate.x;
                    if (coordinate.y != float.MaxValue)
                        m_headPosition.y += coordinate.y;
                    if (coordinate.z != float.MaxValue)
                        m_headPosition.z += coordinate.z;
                }
                else if (m_moveType == MoveType.Absolute)
                {
                    if (coordinate.x != float.MaxValue)
                        m_headPosition.x = coordinate.x;
                    if (coordinate.y != float.MaxValue)
                        m_headPosition.y = coordinate.y;
                    if (coordinate.z != float.MaxValue)
                        m_headPosition.z = coordinate.z;
                }
                m_headToMove.localPosition = m_headPosition;

                m_lastCoordinate = coordinate;
                m_newWorldPosition.Invoke(m_headToMove.position);


                isHandleByTheCode = true;
            }

        }


        if (!isHandleByTheCode)
        {
            WasNotHandleProperly(codeLine);
        }
    }
    
    public Vector3 m_headPosition;

    private void WasNotHandleProperly(IGCodeLine codeLine)
    {
        m_wasNotHandleProperly.Insert(0, codeLine.GetType() + "\n" + JsonUtility.ToJson(codeLine, true));
    }

    private static bool IsCommand(RawGcodeLine g, string character, int command)
    {
        return g.m_commandType.m_char == character &&
                        (g.m_commandType.m_value == command);
    }

    private void SetToolStateAndNotify(bool isOn)
    {
        m_toolStateOnOff = isOn;
        m_toolStateOnOffEvent.Invoke(m_toolStateOnOff);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
