using UnityEngine;
using System.Collections;

namespace TerrainView
{
    public class CameraControl : MonoBehaviour
    {

        public float panRate = 10.0f;
        private Vector3 panOrigin;
        private bool panning = false;

        public float rotateRate = 50.0f;
        private Vector3 rotateOrigin;
        private Vector3 rotateMousePos;
        private bool rotating = false;

        public float zoomSpeed = 40.0f;

        private Plane hitPlane;

        void Start()
        {
            hitPlane = new Plane(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1));
        }

        void Update()
        {
            if (!GameControl.Instance.BackgroundMode)
            {
                if (!DoMousePan()) DoKeyPan();
                if (!DoMouseRotate()) DoKeyRotate();
                if (!DoMouseZoom()) DoKeyZoom();
            }
        }

        bool DoMousePan()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 pos;
                if (MouseHitTest(out pos))
                {
                    panning = true;
                    panOrigin = pos;
                }
            }

            if (!Input.GetMouseButton(1))
            {
                panning = false;
            }

            if (panning)
            {
                Vector3 pos;
                if (MouseHitTest(out pos))
                {
                    Vector3 move = panOrigin - pos;
                    move.y = 0;
                    transform.Translate(move, Space.World);
                }
            }

            return panning;
        }

        void DoKeyPan()
        {
            Vector3 move = new Vector3();

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                move.x -= panRate;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                move.x += panRate;
            }
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                move.z += panRate;
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                move.z -= panRate;
            }

            if (move != Vector3.zero)
            {
                float angle = -transform.eulerAngles.x;
                var moveVector = Quaternion.Euler(new Vector3(angle, 0, 0)) * new Vector3(move.x, 0, move.z);
                transform.Translate(moveVector, Space.Self);
            }
        }

        bool DoMouseRotate()
        {
            if (Input.GetMouseButtonDown(2))
            {
                Vector3 pos;
                if (CameraHitTest(out pos))
                {
                    rotating = true;
                    rotateOrigin = pos;
                    rotateMousePos = Input.mousePosition;
                }
            }

            if (!Input.GetMouseButton(2))
            {
                rotating = false;
            }

            if (rotating)
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - rotateMousePos);

                transform.Translate(new Vector3(pos.x * rotateRate, pos.y * rotateRate, 0));
                transform.LookAt(rotateOrigin);
            }

            return rotating;
        }

        void DoKeyRotate()
        {
            if (!CameraHitTest(out rotateOrigin))
                return;

            Vector3 pos = new Vector3();
            float keyRotateRate = rotateRate / 10;

            if (Input.GetKey(KeyCode.Q))
            {
                pos.x -= keyRotateRate;
            }
            if (Input.GetKey(KeyCode.E))
            {
                pos.x += keyRotateRate;
            }
            if (Input.GetKey(KeyCode.R))
            {
                pos.y += keyRotateRate;
            }
            if (Input.GetKey(KeyCode.F))
            {
                pos.y -= keyRotateRate;
            }

            if (pos != Vector3.zero)
            {
                transform.Translate(new Vector3(pos.x, pos.y, 0));
                transform.LookAt(rotateOrigin);
            }
        }

        bool DoMouseZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                transform.Translate(new Vector3(0, 0, scroll * zoomSpeed * 10), Space.Self);
                return true;
            }

            return false;
        }

        void DoKeyZoom()
        {
            if (Input.GetKey(KeyCode.Equals))
            {
                transform.Translate(new Vector3(0, 0, zoomSpeed), Space.Self);
            }
            if (Input.GetKey(KeyCode.Minus))
            {
                transform.Translate(new Vector3(0, 0, -zoomSpeed), Space.Self);
            }
        }

        private bool MouseHitTest(out Vector3 pos)
        {
            float hitDist = 0;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (hitPlane.Raycast(ray, out hitDist))
            {
                pos = ray.GetPoint(hitDist);
                return true;
            }

            pos = new Vector3();
            return false;
        }

        private bool CameraHitTest(out Vector3 pos)
        {
            float hitDist = 0;
            Ray ray = new Ray(transform.position, transform.forward);
            if (hitPlane.Raycast(ray, out hitDist))
            {
                pos = ray.GetPoint(hitDist);
                return true;
            }

            pos = new Vector3();
            return false;
        }
    }
}