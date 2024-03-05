
using UnityEngine;

namespace CustomLogger
{

    public class Logger
    {

        public static LoggingLevel masterLevel = LoggingLevel.Detailed;

        public static void Log(LoggingLevel messageLevel, string text, LoggingLevel objectLoggingLevel = LoggingLevel.Detailed)
        {
            //print only things that are:
            //lower or same as master level
            //lower or same as object level

            if (masterLevel == LoggingLevel.Off) return;
            if (masterLevel < messageLevel) return;
            if (objectLoggingLevel < messageLevel) return;

            //check if we are on errors or warning, as we only will return those
            if (objectLoggingLevel == LoggingLevel.Errors || masterLevel == LoggingLevel.Errors)
            {
                if (messageLevel == LoggingLevel.Errors)
                {
                    Debug.Log(text);
                }
            }
            else if(objectLoggingLevel == LoggingLevel.Warnings || masterLevel == LoggingLevel.Warnings)
            {
                if (messageLevel == LoggingLevel.Warnings)
                {
                    Debug.Log(text);
                }
                if (messageLevel == LoggingLevel.Errors)
                {
                    Debug.Log(text);
                }
            }
            else
            {
                Debug.Log(text);
            }

        }

        

    }
}

public enum LoggingLevel
{
    Off,
    User,
    Normal,
    Detailed,
    Errors,
    Warnings
}