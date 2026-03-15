using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaceGame
{
    public class GameManager : MonoBehaviour
    {
        #region Fields

        private CheckPointTracker _checkPointTracker;

        #endregion

        #region Properties

        // Singleton format
        public static GameManager Instance { get; private set; }

        public CheckPointTracker CheckPointTracker => _checkPointTracker;
        
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
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
                Instance = this;
            }
        }

        private void Start()
        {
            InitializeManager();
            
            if (Application.isMobilePlatform)
            {
                Application.targetFrameRate = 60;
                QualitySettings.vSyncCount = 0;
            }
        }

        #endregion

        #region Methods

        private void InitializeManager()
        {
            _checkPointTracker = FindFirstObjectByType<CheckPointTracker>();
            if (_checkPointTracker == null)
            {
                Debug.Log("[GAMEMANAGER] No CheckPointTracker found inside " + SceneManager.GetActiveScene().name);
            }
            else
                Debug.Log("[GAMEMANAGER] CheckPointTracker found: " + SceneManager.GetActiveScene().name);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
            {
                // Re-initialize the manager
                // when we go to a new scene
                InitializeManager();
            }
        }

        #endregion
    }
}