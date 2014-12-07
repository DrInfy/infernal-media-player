using System;

namespace InfernalWorkOutTracker.Workout
{
    [Serializable]
    public class Exercise
    {
        public string Name;
        public string Intensity;
        public string Description;
        public TimeSpan Duration;
        public int Result;
    }
}