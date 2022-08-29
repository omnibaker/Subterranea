using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subterranea
{
    [CreateAssetMenu(menuName = "BoosterBoy/Cave")]
    public class CaveSO : ScriptableObject
    {
        [Tooltip("Special cave name, should this cave require to be displayed in UI with a label")]
        public string CaveName = "Default";

        [Tooltip("Level fails when time runs out")]
        public TimeLimit TimeLimit = TimeLimit.MIN_2;

        [Tooltip("Special comments pertaining to this cave")]
        [TextArea] public string CavesNotes;
    }
}
