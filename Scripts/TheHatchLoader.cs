using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
using DaggerfallWorkshop.Game;
//using DaggerfallWorkshop.Game.UserInterface;
using static DaggerfallWorkshop.Game.PlayerActivate;
//using static DaggerfallWorkshop.DaggerfallLadder;
using DaggerfallWorkshop.Game.Utility.ModSupport;   //required for modding features

namespace DaggerfallWorkshop.Game
{
    public class HatchModLoader : MonoBehaviour
    {
        public static Mod mod;
        public static GameObject modObject;

        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            modObject = new GameObject(mod.Title);
            modObject.AddComponent<HatchModLoader>();

            PlayerActivate.RegisterCustomActivation(mod, 41410, 41411, OnHatchActivated);

            mod.IsReady = true;
        }

        private static void OnHatchActivated(RaycastHit hit)
        {
            DaggerfallLadder ladder = hit.transform.GetComponent<DaggerfallLadder>();
                if (hit.distance > DefaultActivationDistance)
                {
                    DaggerfallUI.SetMidScreenText(TextManager.Instance.GetLocalizedText("youAreTooFarAway"));
                    return;
                }
                else
                {
                    ladder.ClimbLadder();
                }
        }
    }
}