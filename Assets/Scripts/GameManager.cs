using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace TruckGame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> debrisParentsList;
        [SerializeField] private TextMeshProUGUI collectedDebrisCount;
        public UnityEvent OnAllDebrisCleared;

        private void Start()
        {
            UIManager.Instance.DisposeButton.onClick.AddListener(CheckDebrisParents);
            UpdateDebrisCountText(); 
        }

        private void OnDestroy()
        {
            UIManager.Instance.DisposeButton.onClick.RemoveListener(CheckDebrisParents);
        }

        private void CheckDebrisParents()
        {
            int totalChildCount = 0;
            int disabledChildCount = 0;
            
            foreach (GameObject parent in debrisParentsList)
            {
                foreach (Transform child in parent.transform)
                {
                    totalChildCount++;
                    if (!child.gameObject.activeSelf)
                    {
                        disabledChildCount++;
                    }
                }
            }

            if (totalChildCount == disabledChildCount)
            {
                OnAllDebrisCleared.Invoke();
            }
            
            UpdateDebrisCountText(disabledChildCount, totalChildCount); 
        }

        private void UpdateDebrisCountText(int disabledCount = 0, int totalCount = 0)
        {
            if (disabledCount == 0 && totalCount == 0)
            {
                foreach (GameObject parent in debrisParentsList)
                {
                    foreach (Transform child in parent.transform)
                    {
                        totalCount++;
                        if (!child.gameObject.activeSelf)
                        {
                            disabledCount++;
                        }
                    }
                }
            }

            collectedDebrisCount.text = $"Debris Collected: {disabledCount}/{totalCount}";
        }
    }
}
