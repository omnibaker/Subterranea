using System.Collections;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// When added to gameObject, checks parameter TagName enum and links to the universal tag names
    /// </summary>
    public class AddTag : MonoBehaviour
    {
        [SerializeField] private TagNames _tagRef;

        private void Awake()
        {
            StartCoroutine(UpdateTagWhenLabelsAreReady());
        }

        /// <summary>
        /// Updates tag for gameObject
        /// </summary>
        public void UpdateTag(GameObject objectBeingUpdated = null)
        {
            if (objectBeingUpdated == null)
            {
                gameObject.tag = GameTags.GetTagByEnum(_tagRef);
            }
            else
            {
                objectBeingUpdated.tag = GameTags.GetTagByEnum(_tagRef);
            }
        }


        /// <summary>
        /// Runs after checking game has initialised properly
        /// </summary>
        private IEnumerator UpdateTagWhenLabelsAreReady()
        {
            yield return new WaitUntil(() => GameData.IsInitialised);
            UpdateTag();
        }
    }
}
