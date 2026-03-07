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

        private int _lapCounter = 0;

        #endregion

        #region Properties

        public int LapCounter => _lapCounter;
        
        #endregion

        #region Events

        public event Action OnNewLapStarted;

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
            _maxCheckPoints = _checkPoints.Length;
        }

        public void CheckPointHit()
        {
            _checkPointsHit++;
            Debug.Log($"[CHECKPOINTTRACKER] Checkpoints count: {_checkPointsHit} of {_maxCheckPoints}");
            if (_checkPointsHit >= _maxCheckPoints)
            {
                _lapCounter++;
                _checkPointsHit = 0;
                for (int i = 0; i < _maxCheckPoints; i++)
                    _checkPoints[i].ResetCheckPoint();
                
                Debug.Log($"[CHECKPOINTTRACKER] All checkpoints hit, starting lap: {_lapCounter}");
            }
        }

        #endregion
    }
}