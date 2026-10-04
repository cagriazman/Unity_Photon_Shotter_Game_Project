using UnityEngine;


    public static class myPyhsic
    {
        public static Vector3 calculateDir(Transform kamera, float sacilma)
        {
            Vector3 ileri = kamera.forward;
            Vector3 rastgele = kamera.right * Random.Range(-sacilma, sacilma) +
                               kamera.up * Random.Range(-sacilma, sacilma);
                           
            return (ileri + rastgele).normalized;
        }

        public static bool hasSpeed(CharacterController controller)
        {
            Vector3 horizontalVelocity = new Vector3(controller.velocity.x, controller.velocity.y, controller.velocity.z);
            float currentSpeed = horizontalVelocity.magnitude;
            if (currentSpeed > 0.1f )
            {
                return true;

            }
            return false;
        }
    }
