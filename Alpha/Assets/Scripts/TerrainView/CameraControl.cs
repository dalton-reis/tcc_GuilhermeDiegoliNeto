using UnityEngine;
using System.Collections;

namespace TerrainView
{
    public class CameraControl : MonoBehaviour
    {

        private Vector3 panOrigin;
        private bool panning = false;

        public float rotateSpeed = 50.0f;
        private Vector3 rotateOrigin;
        private Vector3 rotateMousePos;
        private bool rotating = false;

        public float zoomSpeed = 10.0f;

        private Plane hitPlane;

        void Start()
        {
            hitPlane = new Plane(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1));
        }

        void Update()
        {
            if (!GameControl.Instance.BackgroundMode)
            {
                DoPan();
                DoRotate();
                DoZoom();
            }
        }

        void DoPan()
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
        }

        void DoRotate()
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

                transform.Translate(new Vector3(pos.x * rotateSpeed, pos.y * rotateSpeed, 0));
                transform.LookAt(rotateOrigin);
            }
        }

        void DoZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                Camera.main.fieldOfView -= scroll * zoomSpeed;
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