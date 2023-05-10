using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class Test_PushGCodeSequenceMono : MonoBehaviour
{
    public string m_textPath;
    public TextAsset m_textAsset;
    [TextArea(0,10)]
    public string m_textArea;
    public enum UseTextType { Path, TextAssets, TextArea}
    public UseTextType m_textTypeUsed;
    public float m_timeBetweenLine=0.1f;

    public US m_onGcodeLinePush;
    [System.Serializable] public class US : UnityEvent<string> { }

    string[] m_tokens;
    public int m_index;
    IEnumerator Start()
    {
        string text = "";
        if (m_textTypeUsed == UseTextType.Path)
        {
            if (File.Exists(m_textPath))
                text = File.ReadAllText(m_textPath);
        }
        if (m_textTypeUsed == UseTextType.TextArea)
        {
            text = m_textArea;
        }
        if (m_textTypeUsed == UseTextType.TextAssets)
        {
            text = m_textAsset.text;
        }

            m_tokens = text.Split('\n');

            while (m_index< m_tokens.Length) {
                if(m_timeBetweenLine>0f)
                    yield return new WaitForSecondsRealtime(m_timeBetweenLine);
                if (m_index < m_tokens.Length)
                {
                    m_onGcodeLinePush.Invoke(m_tokens[m_index]);
                    m_index++;
                }
            }
        
    }

   
}
