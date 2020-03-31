using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GStar.Prepare
{
    public class PlayerCamera : MonoBehaviour
    {
        public enum OperateMode
        {
            Follow,
            Free,
        }

        [System.Serializable]
        public class FollowCam
        {
            // The target camera are following
            public Transform _cameraFollowTarget;

            // The distance in the x-z plane to the target
            public float _distanceToTraget = 10.0f;

            // the height we want the camera to be above the target
            public float _heightAboveTarget = 5.0f;
            public float _rotationDamping;
            public float _heightDamping;
            public float _zoomSpeed;
            public float _minHeight, _maxHeight;
            public float _smoothTime; //the smooth time;
        }

        /*this script is used for the camera to follow the target when terget move out of a regular range;
         * writer:sunset;
         * date:2015.8.4;
         */
        [SerializeField] public FollowCam _followCam = new FollowCam();
        public OperateMode _operateMode = OperateMode.Follow;
        public float _minY, _maxY;
        public float _speed;
        private Vector3 _movePosition;
        private Camera _playerCamera;
        private Vector3 _velocity = Vector3.zero;
        private GameObject[] _heroTarget;
        private float Yvalue;


        void Awake()
        { 
            if (_playerCamera == null)
                _playerCamera = gameObject.GetComponent<Camera>();
        }

        void Start()
        {
//            if(_followCam == null)_followCam = new FollowCam();
//            if (_followCam._cameraFollowTarget == null)
//                _followCam._cameraFollowTarget = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void LateUpdate()
        {
            if(_followCam == null)_followCam = new FollowCam();
            if (_followCam._cameraFollowTarget == null)
                _followCam._cameraFollowTarget = GameObject.FindGameObjectWithTag("Player").transform;
            
            if (_operateMode == OperateMode.Follow)
            {
                CameraFollowMove();
            }

            _followCam._heightAboveTarget -= Input.GetAxis("Mouse ScrollWheel") * _followCam._zoomSpeed;
            _followCam._heightAboveTarget = Mathf.Clamp(_followCam._heightAboveTarget, _followCam._minHeight, _followCam._maxHeight);
            if (Input.GetKey(KeyCode.X))
            {
                Yvalue = transform.rotation.y;
                Yvalue += Input.GetAxis("Mouse X") * _followCam._smoothTime * 0.02f;
                // Yvalue = Mathf.Clamp(Yvalue, _minY, _maxY);
                Yvalue = LimitAngle(Yvalue, _minY, _maxY);
                Debug.LogError("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX: " + Yvalue);
                // transform.rotation = new Quaternion(transform.rotation.x, Yvalue, transform.rotation.z, transform.rotation.w);
                transform.rotation = Quaternion.Euler(0, Yvalue, 0);
            }
        }

        private float LimitAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }

            if (angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }


        /// <summary>
        /// camera move mode when the mode is Follow_Mode;
        /// </summary>
        void CameraFollowMove()
        {
            // Early out if we don't have a target
            if (!_followCam._cameraFollowTarget)
            {
                ReFindFollowTarget();
                if (!_followCam._cameraFollowTarget)
                    return;
            }
            else
            {
                var wantedRotationAngle = _followCam._cameraFollowTarget.eulerAngles.y;
                var wantedHeight = _followCam._cameraFollowTarget.position.y + _followCam._heightAboveTarget;

                var currentRotationAngle = transform.eulerAngles.y;
                var currentHeight = transform.position.y;

                // Damp the rotation around the y-axis
                currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, _followCam._rotationDamping * Time.deltaTime);

                // Damp the height
                currentHeight = Mathf.Lerp(currentHeight, wantedHeight, _followCam._heightAboveTarget * Time.deltaTime);

                // Convert the angle into a rotation
                var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

                // Set the position of the camera on the x-z plane to:
                // distance meters behind the target
                transform.position = _followCam._cameraFollowTarget.position;
                transform.position -= currentRotation * Vector3.forward * _followCam._distanceToTraget;

                // Set the height of the camera
                transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

                // Always look at the target
                transform.LookAt(_followCam._cameraFollowTarget);
            }
        }

        public void ReFindFollowTarget()
        {
            _heroTarget = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject _ht in _heroTarget)
            {
                if (_ht.activeSelf)
                {
                    print(_ht.gameObject.name);
                    _followCam._cameraFollowTarget = _ht.transform;
                }
            }
        }
    }
}