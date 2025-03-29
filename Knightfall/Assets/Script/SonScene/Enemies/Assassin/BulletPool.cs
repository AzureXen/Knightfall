using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.SonScene.Enemies.Assassin
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance;
        public GameObject bulletPrefab;
        public int poolSize = 10;

        private List<GameObject> bulletPool = new List<GameObject>();

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.SetActive(false);
                bulletPool.Add(bullet);
            }
        }

        public GameObject GetBullet()
        {
            foreach (var bullet in bulletPool)
            {
                if (!bullet.activeInHierarchy)
                {
                    bullet.SetActive(true);
                    return bullet;
                }
            }

            // Expand pool if needed
            GameObject newBullet = Instantiate(bulletPrefab);
            newBullet.SetActive(false);
            bulletPool.Add(newBullet);
            return newBullet;
        }
    }
}
