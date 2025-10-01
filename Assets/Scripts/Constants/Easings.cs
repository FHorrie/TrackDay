using UnityEngine;

namespace RaceGame.Helpers
{
    public static class Easings
    {
        public static float InOutSine(float t)
        {
            return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
        }
    }
}