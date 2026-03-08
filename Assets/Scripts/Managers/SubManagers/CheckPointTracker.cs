using System;
using UnityEngine;

namespace RaceGame
{
    public class CheckPointTracker : MonoBehaviour
    {
        #region Editor Fields
        
        [SerializeField]
        private CheckPoint[] _checkPoints;
        
        #endregion
        
        #region Fields

        private int _checkPointsHit = 0;
        private int _maxCheckPoints = 0;

        #endregion

        #region Properties

        public int LapCounter { get; private set; } = 1;

        public int NextCheckPointIndex { get; private set; } = 0;

        #endregion

        #region Events

        public event Action<int> OnNewLapStarted;

        #endregion

        #region Life Cycle

        private void Start()
        {
            CalculateCheckPointCount();
        }

        #endregion

        #region Methods

        private void CalculateCheckPointCount()
        {
            int indexToAssign = 0;
            for (int i = 0; i < _checkPoints.Length; ++i)
            {
                _checkPoints[i].AssignedIndex = indexToAssign;
                ++indexToAssign;
            }
            _maxCheckPoints = _checkPoints.Length;
        }

        public void CheckPointHit(int hitIndex)
        {
            if(NextCheckPointIndex != hitIndex) return;
            
            ++_checkPointsHit;
            ++NextCheckPointIndex;
            Debug.Log($"[CHECKPOINTTRACKER] Checkpoints count: {_checkPointsHit} of {_maxCheckPoints}");
            if (_checkPointsHit == _maxCheckPoints)
            {
                NextCheckPointIndex = 0;
            }
            else if (_checkPointsHit > _maxCheckPoints)
            {
                // Offset checkpoints hit by 1
                // because the starting line checkpoint is hit
                _checkPointsHit = 1;
                LapCounter++;
                OnNewLapStarted?.Invoke(LapCounter);
                Debug.Log($"[CHECKPOINTTRACKER] All checkpoints hit, starting lap: {LapCounter}");
            }
        }

        #endregion
    }
}