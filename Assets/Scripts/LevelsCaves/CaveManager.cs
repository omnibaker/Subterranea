using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Controller for cave and all children objects
    /// </summary>
    [RequireComponent(typeof(StartEndPoints))]
    public class CaveManager : MonoBehaviour
    {
        [SerializeField] private CaveSO _cave = default;

        public CaveSO Cave { get { return _cave; } }

        private StartEndPoints _startEndPoints = default;

        private void Awake()
        {
            _startEndPoints = GetComponent<StartEndPoints>();
        }

        /// <summary>
        /// Destroys the cave and ends anu necessary processes
        /// </summary>
        public void RemoveCave()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Returns Vector3 where player starts on this level
        /// </summary>
        public Vector3 GetStartPosition()
        {
            return _startEndPoints.StartPosition;
        }
    }
}
