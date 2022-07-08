using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Models
{
    #region Player

    [Serializable]
    public class PlayerSettingsModel
    {
        [Header("View Settings")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;
        public bool ViewXInverted;
        public bool ViewYInverted;

        [Header("Movement")] 
        public float ForwardSpeed;
        public float StrafeSpeed;
        public float BackwardsSpeed;

        [Header("Jumping")] 
        public float JumpingHeight;
        public float JumpingFallOff;
    }

    #endregion
}
