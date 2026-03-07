using Unity.VisualScripting;
using UnityEngine;

namespace RaceGame
{
    public class CheckPoint : MonoBehaviour
    {
        #region Fields

        private bool _isHit = false;
        
        #endregion
        
        #region Life Cycle
        
        private void Start()
        {
            GameManager.Instance.CheckPointTracker.OnNewLapStarted += ResetCheckPoint;
        }

        private void OnDestroy()
        {
            GameManager.Instance.CheckPointTracker.OnNewLapStarted -= ResetCheckPoint;
        }

        #endregion

        #region Methods

        public void ResetCheckPoint()
        {
            _isHit = false;
        }
        
        private void OnTriggerEnter(Collider collider)
        {
            if (_isHit || collider.gameObject.layer != LayerMask.NameToLayer(Constants.PlayerLayerName)) return;
            
            Debug.Log("[CHECKPOINT] Checkpoint hit");
            _isHit = true;
            GameManager.Instance.CheckPointTracker.CheckPointHit();
        }

        #endregion
    }
}
