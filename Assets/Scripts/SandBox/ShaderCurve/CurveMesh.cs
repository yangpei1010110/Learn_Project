using UnityEngine;

namespace SandBox.ShaderCurve
{
    public class CurveMesh : MonoBehaviour
    {
        [SerializeField] private Vector3[]   _points    = new Vector3[3];
        private                  Vector3[]   _oldPoints = new Vector3[3];
        [SerializeField] private Transform[] _controls;

        private Mesh         _mesh;
        private MeshFilter   _meshFilter;
        private MeshCollider _meshCollider;

        void Start()
        {
            _points = new Vector3[3];
            _oldPoints = new Vector3[3];
            _points[0] = 0.5f * (Vector3.left + Vector3.down);
            _points[1] = 0.5f * (Vector3.right + Vector3.up);
            _points[2] = 0.5f * (Vector3.right + Vector3.down);

            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
            _meshCollider.sharedMesh = _mesh;
            _mesh.name = "CurveMesh";
            _mesh.vertices = _points;
            _mesh.uv = new Vector2[] { Vector2.zero, Vector2.one, Vector2.right * 0.5f, };
            _mesh.triangles = new int[] { 0, 1, 2, };
        }

        // Update is called once per frame
        void Update()
        {
            if (IsChange())
            {
                _mesh.vertices = _points;
                _mesh.RecalculateNormals();
            }
        }

        private bool IsChange()
        {
            _points[0] = _controls[0].localPosition;
            _points[1] = _controls[1].localPosition;
            _points[2] = _controls[2].localPosition;
            return !(_points[0] == _oldPoints[0] && _points[1] == _oldPoints[1] && _points[2] == _oldPoints[2]);
        }
    }
}