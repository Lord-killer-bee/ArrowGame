using Core;
using System;
using UnityEngine;

namespace ArrowGame
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private float _BulletVelocity = 50f;

        //[SerializeField]
        //private GameObject _MonsterPrefab;

        private void OnEnable()
        {
            print("Bullet");
            GetComponent<Rigidbody2D>().velocity = transform.up * _BulletVelocity;
        }

        private void Update()
        {
            if (gameObject.GetComponent<Rigidbody2D>().velocity == new Vector2(0, 0))
            {
               Destroy(gameObject);
            }
        }

        private Transform SpawnLocation;
        private void OnCollisionEnter(Collision other)
        {
            SpawnLocation = other.gameObject.transform;

            if (other.gameObject.CompareTag("Monster"))
            {
                Destroy(other.gameObject);
            }

            else
            {
                GameEventManager.Instance.TriggerSyncEvent(new ArrowGame.InGameEvents.CreateMonsterEvent(SpawnLocation.position + new Vector3(0, 1, 0)));
                //Instantiate(_MonsterPrefab, SpawnLocation.position + new Vector3(0,1,0), SpawnLocation.rotation);
            }

            Destroy(gameObject);
        }



    }
}