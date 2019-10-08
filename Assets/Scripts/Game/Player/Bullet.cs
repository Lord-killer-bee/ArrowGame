using Core;
using System;
using UnityEngine;

namespace ArrowGame
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private float _BulletVelocity = 50f;
        private Transform SpawnLocation;

        private void OnEnable()
        {
            GetComponent<Rigidbody2D>().velocity = transform.up * _BulletVelocity;
        }

        private void Update()
        {
            if (gameObject.GetComponent<Rigidbody2D>().velocity == new Vector2(0, 0))
            {
               Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            SpawnLocation = other.gameObject.transform;

            if (other.gameObject.CompareTag(GameConsts.PLATFORM_TAG))
            {
                GameEventManager.Instance.TriggerSyncEvent(new ArrowGame.InGameEvents.CreateMonsterEvent(SpawnLocation.position + new Vector3(0, 1, 0), MonsterType.SimpleMonster));
            }

            Destroy(gameObject);
        }
    }
}