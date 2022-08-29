using System.Collections.Generic;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Create Dictionary of level/caves by inspecting Resources folder in the build
    /// </summary>
    public class LevelManager
    {
        public const string CAVES_FOLDER = "Caves";

        /// <summary>
        /// Creates Dictionary of cave Lists, with level being the key
        /// </summary>
        public Dictionary<int, List<GameObject>> ConstructAllLevels()
        {
            // Get list of eligible caves
            GetAllCaves(out List<GameObject> caves);

            Dictionary<int, List<GameObject>> levels = new Dictionary<int, List<GameObject>>();
            // If list has contents, loop through then all
            if (caves.Count > 0)
            {
                // Start first level
                int level = 1;
                levels.Add(1, new List<GameObject>());

                for (int cave = 0; cave < caves.Count; cave++)
                {
                    // Add to current level cave list
                    levels[level].Add(caves[cave]);

                    // If reached max caves per level
                    if (levels[level].Count == LevelSettings.CAVES_PER_LEVEL)
                    {
                        // Add current subgroup to Dictionary then start new level
                        //GameData.Levels[level].Clear();
                        level++;
                        levels.Add(level, new List<GameObject>());
                    }
                }
            }
            else
            {
                Debug.LogError("No caves found!");
                return null;
            }

            return levels;
        }

        /// <summary>
        /// Creates List of cave prefabs from allocated folder
        /// </summary>
        private void GetAllCaves(out List<GameObject> caves)
        {
            caves = new List<GameObject>();
            foreach (object cave in Resources.LoadAll(ResFolder.CAVES, typeof(GameObject)))
            {
                caves.Add((GameObject)cave);
            }
        }
    }
}