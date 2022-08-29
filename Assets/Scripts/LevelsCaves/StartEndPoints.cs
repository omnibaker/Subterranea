using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Holds takee-off and landing positions for each cave (has been gutted over time so a bit minimalist now...)
    /// </summary>
    public class StartEndPoints : MonoBehaviour
    {
        [SerializeField] private Transform _startPlatform = default;
        [SerializeField] private Transform _endZone = default;

        public Vector3 StartPosition { get { return _startPlatform.position; } }
        public Vector3 EndPosition { get { return _endZone.position; } }

    }
}