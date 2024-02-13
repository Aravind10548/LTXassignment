using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TruckGame
{
    public class TruckController : MonoBehaviour
    {
        [SerializeField] private List<Transform> debrisPositions;
        [SerializeField] private Transform disposeSection;
        [SerializeField] private List<Transform> wheels;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private List<GameObject> debrisVisualOnTruck;

        private Coroutine _moveCoroutine;
        private Transform _currentDebrisTarget;
        private int _enabledDebrisVisualCount = 0;

        private void Start()
        {
            UIManager.Instance.MoveToDebrisButton.onClick.AddListener(MoveTowardsNextDebris);
            UIManager.Instance.MoveToDisposeSection.onClick.AddListener(MoveTowardsDisposeSection);
            UIManager.Instance.CollectDebrisButton.onClick.AddListener(CollectDebris);
            UIManager.Instance.DisposeButton.onClick.AddListener(DisposeDebris);
            UIManager.Instance.CollectDebrisButton.interactable = false;
            UIManager.Instance.MoveToDisposeSection.interactable = false;
        }

        private void OnDestroy()
        {
            UIManager.Instance.MoveToDebrisButton.onClick.RemoveListener(MoveTowardsNextDebris);
            UIManager.Instance.MoveToDisposeSection.onClick.RemoveListener(MoveTowardsDisposeSection);
            UIManager.Instance.CollectDebrisButton.onClick.RemoveListener(CollectDebris);
            UIManager.Instance.DisposeButton.onClick.RemoveListener(DisposeDebris);
        }


        private void MoveTowardsNextDebris()
        {
            if (debrisPositions.Count > 0 && _moveCoroutine == null && _enabledDebrisVisualCount < 2)
            {
                UIManager.Instance.MoveToDebrisButton.interactable = false;
                UIManager.Instance.CollectDebrisButton.interactable = false;
                _currentDebrisTarget = debrisPositions[0];
                _moveCoroutine = StartCoroutine(MoveAndRotateTowardsTarget(_currentDebrisTarget.position, 180f, () =>
                {
                    UIManager.Instance.CollectDebrisButton.interactable = _enabledDebrisVisualCount < 2;
                    _moveCoroutine = null;
                    UIManager.Instance.MoveToDisposeSection.interactable = true;
                }));
            }
        }

        private void MoveTowardsDisposeSection()
        {
            if (_moveCoroutine == null)
            {
                UIManager.Instance.CollectDebrisButton.interactable = false;
                UIManager.Instance.MoveToDisposeSection.interactable = false;
                Vector3 disposePosition = disposeSection.position;
                _moveCoroutine = StartCoroutine(MoveAndRotateTowardsTarget(disposePosition, 0f, () =>
                {
                    if (Vector3.Distance(transform.position, disposePosition) <= 0.2f)
                    {
                        UIManager.Instance.DisposeButton.interactable = _enabledDebrisVisualCount > 0;
                    }
                    else
                    {
                        UIManager.Instance.DisposeButton.interactable = false;
                    }
                    
                    if (debrisPositions.Count > 0)
                    {
                        UIManager.Instance.MoveToDebrisButton.interactable = true;
                    }

                    _moveCoroutine = null;
                }));
            }
        }


        private IEnumerator MoveAndRotateTowardsTarget(Vector3 targetPosition, float targetRotationY,
            System.Action onComplete)
        {
            Quaternion endRotation =
                Quaternion.Euler(transform.eulerAngles.x, targetRotationY, transform.eulerAngles.z);

            while (Quaternion.Angle(transform.rotation, endRotation) > 0.01f)
            {
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, endRotation,
                        moveSpeed * Time.deltaTime * 100); 
                yield return null;
            }

            Vector3 startPosition = transform.position;
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                float step = moveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
                RotateWheels(step * moveSpeed * 360); 
                yield return null;
            }

            transform.position = targetPosition;

            onComplete?.Invoke();
        }


        private void RotateWheels(float rotationAmount)
        {
            foreach (Transform wheel in wheels)
            {
                wheel.Rotate(Vector3.right, rotationAmount);
            }
        }

        private void CollectDebris()
        {
            if (_currentDebrisTarget != null && _enabledDebrisVisualCount < 2)
            {
                if (_enabledDebrisVisualCount < debrisVisualOnTruck.Count)
                {
                    debrisVisualOnTruck[_enabledDebrisVisualCount].SetActive(true);
                    _enabledDebrisVisualCount++;
                }

                if (_currentDebrisTarget.childCount > 0)
                {
                    _currentDebrisTarget.GetChild(0).gameObject.SetActive(false);
                }

                debrisPositions.Remove(_currentDebrisTarget);
                _currentDebrisTarget = null;
                UIManager.Instance.CollectDebrisButton.interactable = false;

                if (_enabledDebrisVisualCount >= 2)
                {
                    UIManager.Instance.CollectDebrisButton.interactable = false;
                }
                else
                {
                    UIManager.Instance.MoveToDebrisButton.interactable = debrisPositions.Count > 0;
                }
            }
        }

        private void DisposeDebris()
        {
            _enabledDebrisVisualCount = 0;

            foreach (var debrisVisual in debrisVisualOnTruck)
            {
                debrisVisual.SetActive(false);
            }

            UIManager.Instance.MoveToDisposeSection.interactable = false;
            UIManager.Instance.DisposeButton.interactable = false;
            if (debrisPositions.Count > 0)
            {
                UIManager.Instance.MoveToDebrisButton.interactable = true;
            }
        }
    }
}