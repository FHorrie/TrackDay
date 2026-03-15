using UnityEngine;

namespace RaceGame
{
    public class CheckPoint : MonoBehaviour
    {
        #region Properties

        public int AssignedIndex { get; set; } = -1;
        
        #endregion

        #region Methods
        
        private void OnTriggerEnter(Collider collider)
        {
            if (AssignedIndex == -1)
            {
                Debug.LogError(gameObject.name + ": CheckPoint index was never set");
                return;
            }
            
            if (collider.gameObject.layer != LayerMask.NameToLayer(Constants.PlayerLayerName)) return;
            
            Debug.Log("[CHECKPOINT] Checkpoint hit");
            GameManager.Instance.CheckPointTracker.CheckPointHit(AssignedIndex);
        }

        #endregion
    }
}
