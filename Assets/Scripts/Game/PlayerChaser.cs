using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Place in same object as Camera and it will follow the chasedObject's position with a slight drag
    /// </summary>
    public class PlayerChaser : MonoBehaviour
    {
        [SerializeField] private float _dampenedDelay = 0.5f;

        private Transform _chasedObject;
        private Vector3 _velocity = Vector3.zero;

        private void Update()
        {
            ChasePlayer();
        }

        /// <summary>
        /// Follow player with slightly dampened delay
        /// </summary>
        private void ChasePlayer()
        {
            if (_chasedObject != null)
            {
                Vector3 newCamPosition = new Vector3(_chasedObject.position.x, _chasedObject.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, newCamPosition, ref _velocity, _dampenedDelay);
            }
        }

        /// <summary>
        /// Assign object to chase
        /// </summary>
        public void AssignChasedObject(Transform transform = null)
        {
            _chasedObject = transform;
        }
    }
}
