﻿using CleverCrow.Fluid.SimpleSettings;
using UnityEngine;

namespace GameJammers.GGJ2025.FloppyDisks {
    /// <summary>
    /// A global hot loaded singleton with centralized game settings.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Custom/GameSettings")]
    public class GameSettings : SettingsBase<GameSettings> {
        [Tooltip("Declare all valid room layers")]
        [SerializeField]
        LayerMask _roomLayer;

        [Tooltip("Declare all valid level layers")]
        [SerializeField]
        LayerMask _levelLayer;

        [Tooltip("Declare valid disk placement layers")]
        [SerializeField]
        LayerMask _diskPlacementLayer;

        public LayerMask RoomLayer => _roomLayer;
        public LayerMask LevelLayer => _levelLayer;
        public LayerMask DiskPlacementLayer => _diskPlacementLayer;
    }
}
