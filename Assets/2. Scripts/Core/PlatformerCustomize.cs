﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace StonesGaming
{
    public class PlatformerCustomize : MonoBehaviour
    {
        // Core
        [SerializeField] PlatformerEngine _engine;
        [SerializeField] AudioSource _audioSource;

        // Attack
        public int turnAttack = -1;
        public bool attackDirection;
        [SerializeField] GameObject _fire1;
        [SerializeField] GameObject _fire2;
        [SerializeField] GameObject _fire3;
        [SerializeField] GameObject _fireRun1;
        [SerializeField] GameObject _fireRun2;
        [SerializeField] GameObject _fireRun3;
        [SerializeField] Vector3 _firePointRight;
        [SerializeField] Vector3 _firePointLeft;
        [SerializeField] AudioClip _attackClip;

        // Push
        [SerializeField] float _pushSpeed = 0.5f;
        float _groundSpeed;

        // Hurt || hit
        [SerializeField] GameObject _playerHitPrefab;
        [SerializeField] AudioClip _hitClip;

        // Jump
        public bool isSkipJumpSe;
        [SerializeField] GameObject _jumpVfx1;
        [SerializeField] GameObject _jumpVfx2;
        [SerializeField] AudioClip _jumpClip;

        // Health
        [SerializeField] int _originalHealth = 30;
        public static int health;
        [SerializeField] int _damagePlayer = 10;

        bool _toggle = false;
        Vector3 _offset = new Vector3(0.4f, 0, 0);
        Vector3 _playerOriginPosition;
        
        public enum EngineCustomizeState
        {
            Attack,
            Hurt,
            Death,
            Ladder,
            HighJump,
            Teleport,
            Portal
        }

        void Awake()
        {
            _engine.onJump += OnJump;
            _groundSpeed = _engine.groundSpeed;
            health = _originalHealth;
        }

        void Start()
        {
            _playerOriginPosition = transform.position;
        }

        public void Dead()
        {
            gameObject.SetActive(false);
            var cameraShake = FindObjectOfType<CameraShaker>();
            cameraShake.Shake();

            Invoke("OnRetry", 2f);
            Instantiate(_playerHitPrefab, transform.position, Quaternion.identity);

            _audioSource.PlayOneShot(_hitClip);
        }

        public void Attack()
        {
            turnAttack++;
            turnAttack = turnAttack % 3;
            _audioSource.PlayOneShot(_attackClip);

            if (turnAttack == 0)
            {
                if (Globals.AttackDirection)
                {
                    if (_engine.velocity.sqrMagnitude > 0)
                    {
                        SimplePool.Spawn(_fireRun1, transform.position + _firePointRight, transform.rotation);
                    }
                    else
                    {
                        SimplePool.Spawn(_fire1, transform.position + _firePointRight, transform.rotation);
                    }
                }
                else
                {
                    if (_engine.velocity.sqrMagnitude > 0)
                    {
                        SimplePool.Spawn(_fireRun1, transform.position + _firePointLeft, transform.rotation);
                    }
                    else
                    {
                        SimplePool.Spawn(_fire1, transform.position + _firePointLeft, transform.rotation);
                    }
                }
            }
            else if (turnAttack == 1)
            {
                if (Globals.AttackDirection)
                {
                    if (_engine.velocity.sqrMagnitude > 0)
                    {
                        SimplePool.Spawn(_fireRun2, transform.position + _firePointRight, transform.rotation);
                    }
                    else
                    {
                        SimplePool.Spawn(_fire2, transform.position + _firePointRight, transform.rotation);
                    }
                }
                else
                {
                    if (_engine.velocity.sqrMagnitude > 0)
                    {
                        SimplePool.Spawn(_fireRun2, transform.position + _firePointLeft, transform.rotation);
                    }
                    else
                    {
                        SimplePool.Spawn(_fire2, transform.position + _firePointLeft, transform.rotation);
                    }
                }
            }
            else if (turnAttack == 2)
            {
                if (Globals.AttackDirection)
                {
                    if (_engine.velocity.sqrMagnitude > 0)
                    {
                        SimplePool.Spawn(_fireRun3, transform.position + _firePointRight, transform.rotation);
                    }
                    else
                    {
                        SimplePool.Spawn(_fire3, transform.position + _firePointRight, transform.rotation);
                    }
                }
                else
                {
                    if (_engine.velocity.sqrMagnitude > 0)
                    {
                        SimplePool.Spawn(_fireRun3, transform.position + _firePointLeft, transform.rotation);
                    }
                    else
                    {
                        SimplePool.Spawn(_fire3, transform.position + _firePointLeft, transform.rotation);
                    }
                }
            }
        }

        public void Jump()
        {
            SimplePool.Spawn(_jumpVfx1, transform.position, transform.rotation);
        }

        public void OnRetry()
        {
            Globals.HurtFlag = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void OnJump()
        {
            if (!isSkipJumpSe)
            {
                _audioSource.PlayOneShot(_jumpClip);
            }
            isSkipJumpSe = false;
        }

        public bool IsDead()
        {
            if (health <= 0)
                return true;
            return false;
        }

        public void TakeDamage()
        {
            health -= _damagePlayer;
        }

        public void Revival()
        {
            health = _originalHealth;

            if (Globals.Checkpoint != Vector3.zero)
            {
                StartCoroutine(Globals.SetCameraFade(transform, Globals.Checkpoint));
            }
            else
            {
                StartCoroutine(Globals.SetCameraFade(transform, _playerOriginPosition));
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("ToggleMovingPlatform"))
            {
                _toggle = !_toggle;

                if (_toggle)
                {
                    _engine.movingPlatformLayerMask = LayerMask.GetMask("Moving Platforms");
                }
                else
                {
                    _engine.movingPlatformLayerMask = 0;
                }
            }

            if (collision.CompareTag("FireOfBoss"))
            {
                TakeDamage();
                Globals.HurtFlag = true;
            }
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Barrel"))
            {
                if (Mathf.Abs(_engine.velocity.x) > 0)
                {
                    Globals.PushFlag = true;
                    _engine.groundSpeed = _pushSpeed;
                }
                else
                {
                    _engine.groundSpeed = _groundSpeed;
                    Globals.PushFlag = false;
                }
            }
            else if (collision.gameObject.CompareTag("Rock"))
            {
                if (Mathf.Abs(_engine.velocity.x) > 0)
                {
                    Globals.PushFlag = true;
                    _engine.groundSpeed = 0.0001f;
                }
                else
                {
                    _engine.groundSpeed = _groundSpeed;
                    Globals.PushFlag = false;
                    transform.position -= _offset;
                }
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Barrel") || collision.gameObject.CompareTag("Rock"))
            {
                Globals.PushFlag = false;
                _engine.groundSpeed = _groundSpeed;
            }
        }
    }
}