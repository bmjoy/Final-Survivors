using Final_Survivors.Environment;
using UnityEngine;

namespace Final_Survivors
{
    public abstract class Crate : MonoBehaviour
    {
        private MeshRenderer mesh;
        [SerializeField] private bool DisplayByNight = false;

        public abstract void SetPowerUp();

        private void Awake()
        {
            mesh = GetComponent<MeshRenderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                SetPowerUp();
                DestroyCrate();
            }

            if (other.gameObject.CompareTag("FL_Collider") && !EnvironmentState.GetIsDay())
            {
                Display(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("FL_Collider") && !EnvironmentState.GetIsDay())
            {
                Display(false);
            }
        }

        public void DestroyCrate()
        {
            // Add effect
            Destroy(gameObject);
        }

        private void Display(bool value)
        {
            if (DisplayByNight)
            {
                mesh.enabled = value;
            }
        }
    }
}
