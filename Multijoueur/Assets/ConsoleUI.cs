using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class ConsoleUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] int             maxLineCount = 10;

    string myLog;
    int    lineCount;

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Assert:
                logString = "<color=white>" + logString + "</color>";
                break;
            case LogType.Log:
                logString = "<color=white>" + logString + "</color>";
                break;
            case LogType.Warning:
                logString = "<color=yellow>" + logString + "</color>";
                break;
            case LogType.Exception:
                logString = "<color=red>" + logString + "</color>";
                break;
            case LogType.Error:
                logString = "<color=red>" + logString + "</color>";
                break;
        }

        myLog = myLog + "\n" + logString;
        lineCount++;

        if (lineCount > maxLineCount)
        {
            lineCount--;
            // myLog = myLog.Substring(myLog.IndexOf("\n") + 1);
            myLog = DeleteLines(myLog, 1);
        }

        text.text = myLog;
    }

    string DeleteLines(string message, int linesToRemove)
    {
        return message.Split(Environment.NewLine.ToCharArray(), linesToRemove + 1).Skip(linesToRemove).FirstOrDefault();
    }
}