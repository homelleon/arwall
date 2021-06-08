using ARWall.Scripts.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARWall.Scripts.AR
{
    [RequireComponent(typeof(ARSession))]
    public class ARSceneController : MonoBehaviour
    {
        private ARSession _arSession;
        [SerializeField] private ARPlaneManager planeManager;
        [SerializeField] private ARPointCloudManager cloudManager;
        [SerializeField] private Transform objectsContainer;
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private float wallPlaceOffset;
        [SerializeField] private UIManager uiManager;

        IEnumerator Start()
        {
            _arSession = GetComponent<ARSession>();
            if (ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
                yield return ARSession.CheckAvailability();

            if (ARSession.state != ARSessionState.Unsupported)
                StartARTracking();
            else
                ForceFloarPlacing();

            Debug.Log("AR initialization completed!");
        }

        private void ForceFloarPlacing()
        {
            Debug.LogError("AR is not supported!");
            PlaceObjectsOnFloor(0);
        }

        private bool _arStarted;

        private void StartARTracking()
        {
            _arSession.enabled = true;
            cloudManager.enabled = true;
            _arStarted = true;
            planeManager.planesChanged += OnPlaneChanged;
            uiManager.ChangeState(UIState.Scanning);
        }

        private void OnDestroy() => StopTracking();

        private void OnPlaneChanged(ARPlanesChangedEventArgs args)
        {
            var addedList = args.added;
            if (addedList.Count == 0) return;

            var floor = addedList[0];
            var floorHeight = floor.transform.position.y;

            StopTracking();

            PlaceObjectsOnFloor(floorHeight);
        }

        private void StopTracking()
        {
            cloudManager.pointCloudPrefab = null;
            foreach (var trackable in cloudManager.trackables)
                trackable.gameObject.SetActive(false);

            planeManager.planePrefab = null;
            cloudManager.enabled = false;
            planeManager.enabled = false;

            if (!_arStarted) return;
            planeManager.planesChanged -= OnPlaneChanged;
        }

        private void PlaceObjectsOnFloor(float height)
        {
            uiManager.ChangeState(UIState.Playing);
            var wallGO = Instantiate(wallPrefab, objectsContainer);
            var camera = Camera.main;

            var finalOffset = camera.transform.forward.normalized * wallPlaceOffset;
            var finalPosition = camera.transform.position + finalOffset;
            finalPosition.y = height + wallGO.transform.lossyScale.y / 2;

            var rotationEuler = new Vector3(0, camera.transform.rotation.eulerAngles.y, 0);
            var rotationQuaternion = Quaternion.Euler(rotationEuler.x, rotationEuler.y, rotationEuler.z);

            wallGO.transform.SetPositionAndRotation(finalPosition, rotationQuaternion);
        }
    }
}
