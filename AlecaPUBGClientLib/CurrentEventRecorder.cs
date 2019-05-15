using System;
using System.Collections.Generic;

namespace AlecaPUBGClientLib
{
    public class CurrentEventRecorder
    {
        public DateTime startTime;

        public List<RecorderEvent> events = new List<RecorderEvent>(); //Events with video (And data to help identify the telemetry event)

        public CurrentEventRecorder(DateTime _startTime)
        {
            startTime = _startTime;
        }


    }

   
    public class RecorderEvent
    {
        public string eventType = "";
        public string videoPath = "";
        public float lastX;
        public float lastY;
        public float lastZ;
    }

   
}