using UnityEngine;


    public static class SilahFizigi
    {
        public static Vector3 RastgeleYonHesapla(Transform kamera, float sacilma)
        {
            Vector3 ileri = kamera.forward;
            Vector3 rastgele = kamera.right * Random.Range(-sacilma, sacilma) +
                               kamera.up * Random.Range(-sacilma, sacilma);
                           
            return (ileri + rastgele).normalized;
        }
    }
