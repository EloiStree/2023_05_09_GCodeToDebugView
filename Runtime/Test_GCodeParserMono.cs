using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Test_GCodeParserMono : MonoBehaviour
{
    
    public AbstractGcodeParserMono m_parser;

    public string m_lastLine;
    [TextArea(0,10)]
    public string m_lastGcode;

    public List<string> m_receivedLine;
    [TextArea(0,5)]
    public List<string> m_applyGCode;

    public bool m_stopAtNotConvert=true;

    public UnityEvent<IGCodeLine> m_onGCodeEmitted;


    public void PushCommand(string line)
    {
        m_parser.TryToParse(line, out bool converted, out IGCodeLine gcode, out string commentary);
        if(!string.IsNullOrEmpty(commentary))
            InsertGCode(line, converted, new GCode_CommentaryLine() { m_commentary = commentary });
        InsertGCode(line, converted, gcode );

        if (!converted && m_stopAtNotConvert) { 
            Debug.LogError("Impossible to convert previous",this.gameObject);
            Debug.Break();
        }
        m_onGCodeEmitted.Invoke(gcode);
    }

    private void InsertGCode(string line, bool converted, IGCodeLine gcode)
    {
        m_lastLine = line;
        m_lastGcode = converted ? gcode.GetType() + "\n" + JsonUtility.ToJson(gcode, true) : "Fail\n" + line;

        m_receivedLine.Insert(0, line);
        m_applyGCode.Insert(0, m_lastGcode);
    }
  
}


