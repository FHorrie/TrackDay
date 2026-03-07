using UnityEngine;

namespace RaceGame
{
    public class GameManager : MonoBehaviour
    {
        #region Editor Fields
        
        [SerializeField]
        private CheckPointTracker _checkPointTracker;
        
        #endregion
        
        #region Fields

        public CheckPointTracker CheckPointTracker => _checkPointTracker;

        #endregion

        #region Properties

        // Singleton format
        public static GameManager Instance { get; private set; }

        #endregion

        #region Life Cycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                InitializeManager();
                Instance = this;
            }
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        #endregion

        #region Methods

        private void InitializeManager()
        {
            
        }

        #endregion
    }
}