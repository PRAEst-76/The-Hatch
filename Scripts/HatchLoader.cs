using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;   //required for modding features

namespace LocationLoader
{
    public static class HatchModLoader
    {
        public static Mod mod { get; private set; }
        public static GameObject modObject { get; private set; }

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            // Get mod
            mod = initParams.Mod;

            modObject = new GameObject("LocationLoader");
            modObject.AddComponent<LocationLoader>();
            mod.SaveDataInterface = modObject.AddComponent<LocationSaveDataInterface>();
            modObject.AddComponent<LocationResourceManager>();
            mod.IsReady = true;

            const int hatchModelId = 41410;
            PlayerActivate.RegisterCustomActivation(mod, hatchModelId, OnHatchActivated);
        }

        static void OnHatchActivated(RaycastHit hit)
        {
            Transform hatchTransform = hit.transform;
            Transform prefabTransform = hatchTransform.parent;
            GameObject prefabObject = prefabTransform.gameObject;
            LocationData data = prefabObject.GetComponent<LocationData>();

            PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
            bool foundBottom = data.FindClosestMarker(EditorMarkerTypes.LadderBottom, playerMotor.transform.position, out Vector3 bottomMarker);
            bool foundTop = data.FindClosestMarker(EditorMarkerTypes.LadderTop, playerMotor.transform.position, out Vector3 topMarker);

            Vector2 hatchPlanarPos = new Vector2(hatchTransform.position.x, hatchTransform.position.z);
            Vector2 bottomMarkerPlanarPos = new Vector2(bottomMarker.x, bottomMarker.z);
            Vector2 topMarkerPlanarPos = new Vector2(topMarker.x, topMarker.z);

            float bottomPlanarDistance = Vector2.Distance(hatchPlanarPos, bottomMarkerPlanarPos);
            float topPlanarDistance = Vector2.Distance(hatchPlanarPos, topMarkerPlanarPos);

            const float MaxMarkerDistance = PlayerActivate.DefaultActivationDistance * 2;
            foundBottom = foundBottom && bottomPlanarDistance < MaxMarkerDistance;
            foundTop = foundTop && topPlanarDistance < MaxMarkerDistance;

            float bottomDistance = Vector3.Distance(playerMotor.transform.position, bottomMarker);
            float topDistance = Vector3.Distance(playerMotor.transform.position, topMarker);

            // Teleport to top marker
            if (foundTop && (!foundBottom || topDistance > bottomDistance))
            {
                playerMotor.transform.position = topMarker;
                playerMotor.FixStanding();
            }
            else if (foundBottom)
            {
                playerMotor.transform.position = bottomMarker;
                playerMotor.FixStanding();
            }
        }
    }
}