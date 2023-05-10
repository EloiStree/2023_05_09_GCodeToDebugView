using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

// Documentation: https://marlinfw.org/docs/gcode/M082.html

public class V0_GcodeParserMono : AbstractGcodeParserMono
{

    public AbstractGcodeParser m_parser = new V0_GcodeParser();
    public override void TryToParse(string textLine, out bool converted, out IGCodeLine gcode, out string commentary)
    {
        m_parser.TryToParse(textLine, out converted, out gcode, out commentary);
    }
}
public class V0_GcodeParser : AbstractGcodeParser
{

    public override void TryToParse(string textLine, out bool converted, out IGCodeLine gcode, out string commentary)
    {
        converted = false;
        gcode = null;
        commentary = "";
        textLine = textLine.ToUpper().Trim();

        SplitLineWithCommentary(textLine, out textLine, out commentary);

        if (textLine.Length == 0)
        { 
            gcode = new GCode_EmptyLine();
            converted = true; return;
        }

        string[] token = ClearAndSplitInToken(ref textLine);
        if (token.Length == 0) {
            return;
        
        }

        RawGcodeLine rawGcode = new RawGcodeLine();
        List<RawGcodeLineToken> tokens = 
            token.Select(k => new RawGcodeLineToken(k)).ToList();
        rawGcode.m_givenLine = textLine;
        rawGcode.m_commandType = tokens[0];
        tokens.RemoveAt(0);
        rawGcode.m_tokens = tokens.ToList();
        rawGcode.m_commentary = commentary;
        gcode = rawGcode;
        converted = true;
        return;


        if (IsLineType(token, "M140"))
        {
            ConvertTokenTo(token, out GCode_M140_HeatBed g);
            gcode = g; converted = true; return;
        }

        if (IsLineType(token, "M190"))
        {
            ConvertToken_S_To(token, out GCode_M190_WaitForHeatBed g);
            gcode = g; converted = true; return;
        }

        if (IsLineType(token, "M104"))
        {
            ConvertTokenTo(token, out GCode_M104_HeatHead g);
            gcode = g; converted = true; return;
        }

        if (IsLineType(token, "M109"))
        {
            ConvertTokenTo(token, out GCode_M109_WaitForHeatHead g);
            gcode = g; converted = true; return;
        }
        if (IsLineType(token, "M82"))
        {
            ConvertTokenTo(token, out GCode_M82_EAXisAsAbsolute g);
            gcode = g; converted = true; return;
        }
        if (IsLineType(token, "M83"))
        {
            ConvertTokenTo(token, out GCode_M83_EAXisRelative g);
            gcode = g; converted = true; return;
        }

        if (IsLineType(token, "M105"))
        {
            ConvertTokenTo(token, out GCode_M105_HeatReport g);
            gcode = g; converted = true; return;
        }
        if (IsLineType(token, "G28"))
        {
            gcode = new GCode_G28_RequestAutoHomeCalibration(); converted = true; return;
        }
        if (IsLineType(token, "G92"))
        {
            GCode_M92_AxisStepsPerUnitAllAxis g = new GCode_M92_AxisStepsPerUnitAllAxis();

            Get_Char_ValueDouble(token, 'T', out g.m_targetExtruder, 0);
            Get_Char_ValueDouble(token, 'E', out g.m_stepOfAxisEInStepPerMillimeter, 0);
            Get_Char_ValueDouble(token, 'X', out g.m_stepOfAxisXInStepPerMillimeter, 0);
            Get_Char_ValueDouble(token, 'Y', out g.m_stepOfAxisYInStepPerMillimeter, 0);
            Get_Char_ValueDouble(token, 'Z', out g.m_stepOfAxisZInStepPerMillimeter, 0);
            gcode = g;
        }
        if (IsLineType(token, "G1"))
        {
            GCode_G1_PrintTranslateAxis g = new GCode_G1_PrintTranslateAxis();

            Get_Char_ValueDouble(token, 'T', out g.m_feedRate, 0);
            Get_Char_ValueDouble(token, 'S', out g.m_laserPower, 0);
            Get_Char_ValueDouble(token, 'E', out g.m_axisExtruderInMillimeter, 0);
            Get_Char_ValueDouble(token, 'X', out g.m_axisXInMillimeter, 0);
            Get_Char_ValueDouble(token, 'Y', out g.m_axisYInMillimeter, 0);
            Get_Char_ValueDouble(token, 'Z', out g.m_axisZInMillimeter, 0);
            gcode = g;
        }
        if (IsLineType(token, "G0"))
        {
            GCode_G0_MoveTranslateAxis g = new GCode_G0_MoveTranslateAxis();
            Get_Char_ValueDouble(token, 'T', out g.m_feedRate, 0);
            Get_Char_ValueDouble(token, 'S', out g.m_laserPower, 0);
            Get_Char_ValueDouble(token, 'E', out g.m_axisExtruderInMillimeter, 0);
            Get_Char_ValueDouble(token, 'X', out g.m_axisXInMillimeter, 0);
            Get_Char_ValueDouble(token, 'Y', out g.m_axisYInMillimeter, 0);
            Get_Char_ValueDouble(token, 'Z', out g.m_axisZInMillimeter, 0);
            gcode = g;
        }
        if (IsLineType(token, "G92"))
        {
            GCode_G0_MoveTranslateAxis g = new GCode_G0_MoveTranslateAxis();
            Get_Char_ValueDouble(token, 'T', out g.m_feedRate, 0);
            Get_Char_ValueDouble(token, 'S', out g.m_laserPower, 0);
            Get_Char_ValueDouble(token, 'E', out g.m_axisExtruderInMillimeter, 0);
            Get_Char_ValueDouble(token, 'X', out g.m_axisXInMillimeter, 0);
            Get_Char_ValueDouble(token, 'Y', out g.m_axisYInMillimeter, 0);
            Get_Char_ValueDouble(token, 'Z', out g.m_axisZInMillimeter, 0);
            gcode = g;
        }
    }

    private void ConvertTokenTo(string[] token, out GCode_M83_EAXisRelative g)
    {
        g = new GCode_M83_EAXisRelative();
    }

    private void ConvertTokenTo(string[] token, out GCode_M82_EAXisAsAbsolute g)
    {
        g = new GCode_M82_EAXisAsAbsolute();
    }

    private void SplitLineWithCommentary(string givenLine, out string newLine, out string commentary)
    {
        int commentaryIndex = givenLine.IndexOf(";");
        if ( commentaryIndex> -1)
        {
            newLine = givenLine.Substring(0, commentaryIndex);
            commentary = givenLine.Substring( commentaryIndex+1);
        }
        else { 
            newLine = givenLine;
            commentary = "";
        }
    }

   

    private void ConvertTokenTo(string[] token, out GCode_M105_HeatReport g)
    {
        g = new GCode_M105_HeatReport();
    }

    private void ConvertTokenTo(string[] token, out GCode_M109_WaitForHeatHead created)
    {
        created = new GCode_M109_WaitForHeatHead();
        Get_Char_ValueDouble(token, 'S', out created.m_wantedHeatHeadInDegree, 200);
    }

    private void ConvertTokenTo(string[] token, out GCode_M104_HeatHead created)
    {
        created = new GCode_M104_HeatHead();
        Get_Char_ValueDouble(token, 'S', out created.m_heatHeadInDegree, 200);
    }

    private void ConvertToken_S_To(string[] token, out GCode_M190_WaitForHeatBed created)
    {
        created = new GCode_M190_WaitForHeatBed();
        Get_Char_ValueDouble(token, 'S', out created.m_wantedHeatBedInDegree, 200);
    }

    private bool Get_Char_ValueDouble(string[] token, char charStart, out GCodeParamsValueDouble value, double notFoundValue)
    {
        value = new GCodeParamsValueDouble();
        value.m_value = default;
        for (int i = 1; i < token.Length; i++)
        {
            if (token[i].Length > 1 && token[i][0] == charStart)
            {
                string v = token[i].Replace(charStart.ToString(), "");
                double dv = GetDoubleOf(v);
                value.SetAsUsedWithValue(dv);
                return true;
            }
        }
        value.SetAsNotUsed();
        return false;
    }
    private bool Get_Char_ValueDouble(string[] token, char charStart, out GCodeParamsValueInt value, double notFoundValue)
    {
        value = new GCodeParamsValueInt();
        value.m_value = default;
        for (int i = 1; i < token.Length; i++)
        {
            if (token[i].Length > 1 && token[i][0] == charStart)
            {
                string v = token[i].Replace(charStart.ToString(), "");
                int dv = GetIntOf(v);
                value.SetAsUsedWithValue(dv);
                return true;
            }
        }
        value.SetAsNotUsed();
        return false;
    }

    private void  ConvertTokenTo(string[] token, out GCode_M140_HeatBed created)
    {
        created = new GCode_M140_HeatBed();
        Get_Char_ValueDouble(token, 'S', out created.m_headBedInDegree, 200);
    }

    private static bool IsLineType(string[] token, string tag)
    {
        return token.Length > 0 && token[0].Equals(tag);
    }

    private double GetDoubleOf(string text)
    {
        double.TryParse(text, out double v);
        return v;
    }
    private int GetIntOf(string text)
    {
        int.TryParse(text, out int v);
        return v;
    }

    private static string[] ClearAndSplitInToken(ref string textLine)
    {
        textLine = CleanTheLine(textLine);
        string[] token = textLine.Split(" ");
        return token;
    }

    private static string CleanTheLine(string textLine)
    {
        textLine = textLine.ToUpper().Trim();
        while (textLine.IndexOf("  ") >= 0)
        {
            textLine = textLine.Replace("  ", " ");
        }

        int commentaryIndex = textLine.IndexOf(";");
        if (commentaryIndex > -1)
            textLine = textLine.Substring(0,commentaryIndex);
        return textLine;
    }
}



//M140 S50
//M105
//M190
//M105
//M82

public struct GCode_M140_HeatBed : IGCodeLine
{
    public GCodeParamsValueDouble m_headBedInDegree;
}
public struct GCode_EmptyLine : IGCodeLine
{ }

public struct GCode_CommentaryLine : IGCodeLine
{
    public string m_commentary;
}
public struct GCode_M190_WaitForHeatBed : IGCodeLine
{
    public GCodeParamsValueDouble m_wantedHeatBedInDegree;
}
public struct GCode_M104_HeatHead : IGCodeLine
{
    public GCodeParamsValueDouble m_heatHeadInDegree;
}
public struct GCode_M109_WaitForHeatHead : IGCodeLine
{
    public GCodeParamsValueDouble m_wantedHeatHeadInDegree;
}

public struct GCode_M105_HeatReport : IGCodeLine { }
public struct GCode_M82_EAXisAsAbsolute : IGCodeLine { }
public struct GCode_M83_EAXisRelative : IGCodeLine { }
public struct GCode_M92_AxisStepsPerUnitExtruder : IGCodeLine
{
    public int m_targetExtruder;
    public double m_stepOfAxisEInStepPerMillimeter;
}

public struct GCode_M92_AxisStepsPerUnitAllAxis : IGCodeLine
{
    public GCodeParamsValueInt m_targetExtruder;
    public GCodeParamsValueDouble m_stepOfAxisEInStepPerMillimeter;
    public GCodeParamsValueDouble m_stepOfAxisXInStepPerMillimeter;
    public GCodeParamsValueDouble m_stepOfAxisYInStepPerMillimeter;
    public GCodeParamsValueDouble m_stepOfAxisZInStepPerMillimeter;
}

public struct GCode_G28_RequestAutoHomeCalibration: IGCodeLine { }

public struct GCode_G1_PrintTranslateAxis : IGCodeLine
{
    public GCodeParamsValueDouble m_feedRate;
    public GCodeParamsValueDouble m_laserPower;
    public GCodeParamsValueDouble m_axisXInMillimeter;
    public GCodeParamsValueDouble m_axisYInMillimeter;
    public GCodeParamsValueDouble m_axisZInMillimeter;
    public GCodeParamsValueDouble m_axisExtruderInMillimeter;
}

public struct GCode_G0_MoveTranslateAxis : IGCodeLine
{
    public GCodeParamsValueDouble m_feedRate;
    public GCodeParamsValueDouble m_laserPower;
    public GCodeParamsValueDouble m_axisXInMillimeter;
    public GCodeParamsValueDouble m_axisYInMillimeter;
    public GCodeParamsValueDouble m_axisZInMillimeter;
    public GCodeParamsValueDouble m_axisExtruderInMillimeter;
}


[System.Serializable]
public struct GCodeParamsValueDouble
{

    public bool m_isUse;
    public double m_value;

    public void SetAsNotUsed() { m_isUse = false; m_value = 0; }
    public void SetAsUsedWithValue(double value) { m_isUse = true; m_value = value; }
    public void SetAsUsed() { m_isUse = true; }
    public void SetValue(double value) { m_value = value; }
}
[System.Serializable]
public struct GCodeParamsValueInt
{

    public bool m_isUse;
    public int m_value;

    public void SetAsNotUsed() { m_isUse = false; m_value = 0; }
    public void SetAsUsedWithValue(int value) { m_isUse = true; m_value = value; }
    public void SetAsUsed() { m_isUse = true; }
    public void SetValue(int value) { m_value = value; }
}


[System.Serializable]
public class RawGcodeLine :IGCodeLine{
    public string m_commentary;
    public string m_givenLine;
    public RawGcodeLineToken m_commandType=new RawGcodeLineToken();
    public List<RawGcodeLineToken> m_tokens = new List<RawGcodeLineToken>();

    public bool GetValueOf(string character, out RawGcodeLineToken value)
    {
        character = character.ToUpper();
        for (int i = 0; i < m_tokens.Count; i++)
        {
            if (m_tokens[i].m_char == character.ToUpper())
            {
                value = m_tokens[i];
                return true;
            }
        }
        value = new RawGcodeLineToken();
        return false;
    }

    public bool HasToken()
    {
        return m_tokens.Count > 0;
    }
}

[System.Serializable]
public struct RawGcodeLineToken {
    public string m_token;
    public string m_char;
    public int m_value;
    public int m_decimal;

    public RawGcodeLineToken(string token) : this()
    {
        SetWith(token);
    }

    public void SetWith(string textValue) {
        textValue = textValue.ToUpper().Trim();
        m_token = textValue;
        m_char = textValue[0].ToString().ToUpper();
        string t = textValue.Substring(1).Replace(",",".");
        int dotIndex = t.IndexOf(".");
        if (dotIndex < 0)
        {
            m_decimal = 0;
            int.TryParse(t, out m_value);
        }
        else {
            //I am here;
            m_decimal = t.Length-1 - dotIndex;
            int.TryParse(t.Replace(".",""), out m_value);

        }
        
    }
    public void GetValueAs(out double value) {
        value = m_value;
        for (int i = 0; i < m_decimal; i++)
        {
            value /= 10.0;
        }
        
    }
}
