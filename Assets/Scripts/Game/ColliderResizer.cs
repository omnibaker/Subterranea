using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Shrinks a box collider to whatever the visible bounds an object has
    /// </summary>
    public class ColliderResizer : MonoBehaviour
    {
        private void Awake()
        {
            ResizeToTiledSprite();
        }

        /// <summary>
        /// Resizes the box collider
        /// </summary>
        private void ResizeToTiledSprite()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            Vector2 size = collider.size;
            Vector2 offset = collider.offset;
            Vector2 newOffset = offset / size;

            collider.size = sr.size;
            collider.offset = new Vector2(sr.size.x * newOffset.x, sr.size.y * newOffset.y);
        }
    }
}
