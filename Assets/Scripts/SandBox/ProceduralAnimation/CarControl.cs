using System.Diagnostics;
using Tools.Utility;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

namespace SandBox.ProceduralAnimation
{
    public class CarControl : MonoBehaviour
    {
        [SerializeField] private Transform[] _wheels;
        [SerializeField] private Rigidbody   _rigidbody;
        [SerializeField] private float       _force        = 2f;
        [SerializeField] private float       _distance     = 0.5f;
        [SerializeField] private float       _forwardForce = 2f;

        private string _playerTag = @"Player";
        private LazyOnFrame<int> _frame = new LazyOnFrame<int>(() =>
        {
            Debug.Log($"invoke on frame {Time.frameCount}");
            return Time.frameCount;
        });

        public int DisplayFrame;

        public ProfilerMarker ProfilerMarker = new ProfilerMarker(nameof(FuncUpdate));
        public void FuncUpdate()
        {
            using (ProfilerMarker.Auto())
            {
                if (Time.frameCount % 100 == 0)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    for (int i = 0; i < 1000000; i++)
                    {
                        DisplayFrame = _frame;
                    }

                    sw.Stop();
                    Debug.Log($"{sw.Elapsed}");
                }
            }
        }

        void Update()
        {
            foreach (Transform w in _wheels)
            {
                var ray = new Ray(w.position, Vector3.down);

                if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, _distance))
                {
                    if (_playerTag.Equals(hit.collider.tag))
                    {
                        continue;
                    }

                    // Debug.DrawLine(ray.origin, hit.point, Color.red);
                    var force = _force * (1f - hit.distance / _distance);
                    _rigidbody.AddForceAtPosition(Vector3.up * force, w.position);
                }
            }

            if (Keyboard.current[Key.W].isPressed)
            {
                _rigidbody.AddForce(transform.forward * _forwardForce);
            }

            if (Keyboard.current[Key.S].isPressed)
            {
                _rigidbody.AddForce(-transform.forward * _forwardForce);
            }
        }
    }
}