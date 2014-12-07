using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfernalWorkOutTracker.Workout
{
    [Serializable]
    public class WorkoutPlan
    {
        public List<Exercise> Exercises { get; set; }
    }
}
