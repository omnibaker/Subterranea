using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// This is added on the test gameObject holding temporary Camera/EventSystem objects in non-Main scenes
    /// </summary>
    public class TestingBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            DieIfNotWanted();
        }

        /// <summary>
        /// Kills itself if game has been initialised properly (i.e. from Main scene)
        /// </summary>
        private void DieIfNotWanted()
        {
            if (GameData.IsInitialised)
            {
                Destroy(gameObject);
            }
        }
    }
}