using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TruckGame
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button moveToDebrisButton;
        [SerializeField] private Button moveToDisposeSectionButton;
        [SerializeField] private Button collectDebrisButton;
        [SerializeField] private Button disposeButton;
        [SerializeField] private Button restartButton; 
        public Button MoveToDebrisButton
        {
            get => moveToDebrisButton;
            set => moveToDebrisButton = value;
        }

        public Button MoveToDisposeSection
        {
            get => moveToDisposeSectionButton;
            set => moveToDisposeSectionButton = value;
        }

        public Button CollectDebrisButton
        {
            get => collectDebrisButton;
            set => collectDebrisButton = value;
        }

        public Button DisposeButton
        {
            get => disposeButton;
            set => disposeButton = value;
        }

        public static UIManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(0);
        }
    }
}