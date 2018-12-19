using UnityEngine;
using System.Collections;

namespace Forge3D
{
    public class F3DWarpJumpTunnel : MonoBehaviour
    {
        public float RotationSpeed;

        private void Start() {
            transform.localRotation = transform.localRotation * Quaternion.Euler(0, 0, Random.Range(-360, 360));
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
        }
    }
}