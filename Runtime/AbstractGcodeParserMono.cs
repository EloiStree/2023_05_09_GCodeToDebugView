
using UnityEngine;

public interface IGcodeParser
{
    public void TryToParse(string textLine, out bool converted, out IGCodeLine gcode, out string commentary);
}


public abstract class AbstractGcodeParserMono : MonoBehaviour, IGcodeParser
{
    public abstract void TryToParse(string textLine, out bool converted, out IGCodeLine gcode, out string commentary);
}
public abstract class AbstractGcodeParser : IGcodeParser
{
    public abstract void TryToParse(string textLine, out bool converted, out IGCodeLine gcode, out string commentary);
}


public interface IGCodeLine
{

}
public interface IGCodeLineRawSource
{
    public void GetLineRawText(out string gcodeLine);
}

public class GCodeLine : IGCodeLine, IGCodeLineRawSource
{
    public string m_gcodeLine;
    public void GetLineRawText(out string gcodeLine)
    {
        gcodeLine = m_gcodeLine;
    }
}

/// <summary>
/// Move the head with a speed
/// </summary>
public struct GCode_G1_Translate : IGCodeLine
{
    public Vector3 m_axisToMoveInMm;
    public float m_moveSpeedMMPerMinute;
}
public struct GCode_G90_SetRelativeCoordinate : IGCodeLine
{ }
public struct GCode_G91_SetAbsoluteCoordinate : IGCodeLine
{

}
public enum GcodeAxeType { X, Y, Z }
public struct GCode_G92_InitialiserAxe : IGCodeLine
{
    //G92 Z0
    public GcodeAxeType m_axeType;
    public float m_axePositionInMm;
}
public struct GCode_G28_InitialiserAxe : IGCodeLine
{
    ///// <summary>
    /// G28 set to zer
    /// G28 X10 Y10 set x and y to 10
    /// </summary>
    public Vector3 m_axePositionInMm;
}


