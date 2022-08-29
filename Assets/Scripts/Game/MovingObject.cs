using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Cave obstacles that move in and out from the walls, threatening to hit and damage the player
    /// </summary>
    public class MovingObject : MonoBehaviour
    {
        [SerializeField] private MovementType _movementType = MovementType.Oscilate;
        [SerializeField] private Transform _rotatableObject = null;
        [SerializeField] private float _distance = 2f;
        [SerializeField] private float _rate = 2f;
        [SerializeField] private float _startTimeOffset = 0;

        private enum MovementType { Oscilate, Force }
        private Vector3 _startPosition;
        private Rigidbody _rb;
        private bool _pushLeft;
        private float _waitTime;
        private float _oscFactor;

        private void Awake()
        {
            // Only needed if using Unity Physics force to move
            _rb = _rotatableObject.GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _startPosition = _rotatableObject.localPosition;
        }

        private void Update()
        {
            if (!Gameplay.I.IsPaused)
            {
                switch (_movementType)
                {
                    case MovementType.Oscilate: OscillateBackAndForth(); break;
                    case MovementType.Force: ForceIt(); break;
                }
            }
        }

        /// <summary>
        /// Push/pulls obstacle object using trigonometry
        /// </summary>
        private void OscillateBackAndForth()
        {
            _oscFactor = Mathf.Sin((Time.time + _startTimeOffset) * _rate);
            _rotatableObject.localPosition = _startPosition + Vector3.up * _oscFactor * _distance;
        }

        /// <summary>
        /// Push/pulls obstacle object using trigonometry using Rigidbody
        /// </summary>
        private void ForceIt()
        {
            if(_rb != null)
            {
                if (_waitTime <= 0)
                {
                    float multiplier = _pushLeft ? -20f : 20f;
                    _pushLeft = !_pushLeft;
                    _rb.AddForce(Vector3.forward * multiplier, ForceMode.Impulse);
                    _waitTime = 2f;
                }
                else
                {
                    _waitTime -= Time.deltaTime;
                }
            }
        }
    }
}