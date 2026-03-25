using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace PlatformCharacterController
{
    public class MovementCharacterController : NetworkBehaviour
    {
        [Header("Player Controller Settings")] [Tooltip("Speed for the player.")]
        public float RunningSpeed = 5f;

        [Tooltip("Slope angle limit to slide.")]
        public float SlopeLimit = 45;

        [Tooltip("Slide friction.")] [Range(0.1f, 0.9f)]
        public float SlideFriction = 0.3f;

        [Tooltip("Gravity force.")] [Range(0, -100)]
        public float Gravity = -30f;

        [Tooltip("Max speed for the player when fall.")] [Range(0, 100)]
        public float MaxDownYVelocity = 15;

        [Tooltip("Can the user control the player?")]
        public bool CanControl = true;

        [Header("Jump Settings")] [Tooltip("This allow the character to jump.")]
        public bool CanJump = true;

        [Tooltip("Jump max elevation for the character.")]
        public float JumpHeight = 2f;

        [Tooltip("This allow the character to jump in air after another jump.")]
        public bool CanDoubleJump = true;

        [Header("Dash Settings")] [Tooltip("The player have dash?.")]
        public bool CanDash = true;

        [Tooltip("Cooldown for the dash.")] public float DashCooldown = 3;

        [Tooltip("Force for the dash, a greater value more distance for the dash.")]
        public float DashForce = 5f;

        [Header("JetPack")] [Tooltip("Player have jetpack?")]
        public bool Jetpack = true;

        [Tooltip("The fuel maxima capacity for the jetpack.")]
        public float JetPackMaxFuelCapacity = 90;

        [Tooltip("The current fuel for the jetpack, if 0 the jet pack off.")]
        public float JetPackFuel;

        [Tooltip("The force for the jetpack, this impulse the player up.")]
        public float JetPackForce;

        [Tooltip("Jet pack consume this quantity by second active.")]
        public float FuelConsumeSpeed;

        [Header("SlowFall")] [Tooltip("This allow the player a slow fall, you can use an item like a parachute.")]
        public bool HaveSlowFall;

        [Tooltip("Speed vertical for the slow fall.")] [Range(0, 5)]
        public float SlowFallSpeed = 1.5f;

        [Tooltip("Slow fall forward speed.")] [Range(0, 1)]
        public float SlowFallForwardSpeed = 0.1f;

        [Header("Push settings:")]
        [Tooltip(
            "True to only move the object in the opposite direction to the pushed face. False to move the object based on where it is pushed.")]
        public bool PushInFixedDirections;

        [Tooltip("Force of pushing objects")] public float PushPower = 2.0f;

        [Tooltip("This is the drag force for the character, a standard value are (8, 0, 8). ")]
        public Vector3 DragForce;

        [Tooltip("Player Status: Holds or not an object")]
        public bool HoldingObject;
        [Tooltip("Player Status: Swimming")]
        public bool Swimming;

        [Tooltip("This is the animator for you character.")]
        public Animator PlayerAnimator;


        [Header("Effects")] [Tooltip("This position is in the character feet and is use to instantiate effects.")]
        public Transform LowZonePosition;

        public GameObject JumpEffect;
        public GameObject DashEffect;
        public GameObject JetPackObject;
        public GameObject SlowFallObject;

        [Header("Use this to capture inputs")] public Inputs PlayerInputs;

        [Header("Platforms")] public Transform CurrentActivePlatform;

        private Vector3 _moveDirection;
        private Vector3 _activeGlobalPlatformPoint;
        private Vector3 _activeLocalPlatformPoint;
        private Quaternion _activeGlobalPlatformRotation;
        private Quaternion _activeLocalPlatformRotation;
        private Transform _characterTransform;

        //private vars
        private CharacterController _controller;
        private Vector3 _velocity;

        //Input.
        private float _horizontal;
        private float _vertical;

        private bool _jump;
        private bool _dash;
        private bool _flyJetPack;
        private bool _slowFall;

        //get direction for the camera
        private Transform _cameraTransform;
        private Vector3 _forward;
        private Vector3 _right;

        //temporal vars
        private float _originalRunningSpeed;
        private float _dashCooldown;
        private float _gravity;
        private bool _doubleJump;
        private bool _invertedControl;
        private bool _isCorrectGrounded;
        private bool _isGrounded;
        private bool _activeFall;
        private Vector3 _hitNormal;
        private Vector3 _move;
        private Vector3 _direction;

        private CameraManager cameraManager;
        private LobbyPage lobbyPage;
        private LobbyManager lobbyManager;
        private GameManager gameManager;
        private NetworkEvents networkEvents;

        private bool isJetPackEnabled;

        public bool IsJetPackEnabled
        {
            get => isJetPackEnabled;
            set
            {
                if (isJetPackEnabled.Equals(value))
                {
                    return;
                }
                isJetPackEnabled = value;
                JetPackSetActiveServerRPC(value);
            }
        }

        private bool isFallObjectEnabled;

        public bool IsFallObjectEnabled
        {
            get => isFallObjectEnabled;
            set
            {
                if (isFallObjectEnabled.Equals(value))
                {
                    return;
                }
                isFallObjectEnabled = value;
                FallObjectSetActiveServerRPC(value);
            }
        }
        
        public override void OnNetworkSpawn()
        {
            Debug.Log("OnNetworkSpawn MCC");
            _controller = GetComponent<CharacterController>();
            cameraManager = ReferenceManager.Get<CameraManager>();
            lobbyPage = ReferenceManager.Get<LobbyPage>();
            lobbyManager = ReferenceManager.Get<LobbyManager>();
            gameManager = ReferenceManager.Get<GameManager>();
            networkEvents = ReferenceManager.Get<NetworkEvents>();
            
#if UNITY_EDITOR
            PlayerInputs = gameManager.TestInMobile ? GetComponent<MobilePlayerInput>() : GetComponent<Inputs>();
#elif UNITY_ANDROID
            PlayerInputs = GetComponent<MobilePlayerInput>();
#else
            PlayerInputs = GetComponent<Inputs>();
#endif
            
            _characterTransform = transform;
            _originalRunningSpeed = RunningSpeed;
            
            _dashCooldown = DashCooldown;
            _gravity = Gravity;
            
            _cameraTransform = cameraManager.GetMainCamera().transform;
            
            if (IsOwner)
            {
                PlayerManager.LocalPlayerInstance = this;
                
                _controller.enabled = false;
                _characterTransform.position = new Vector3(0, 1, -100);
                _characterTransform.eulerAngles = Vector3.zero;
                _controller.enabled = true;
                
                networkEvents.OnSceneDataInitialized += delegate
                {
                    CanControl = false;
                };
                
                networkEvents.OnGameStarted += delegate(object sender, EventArgs args)
                {
                    _controller.enabled = false;
                    _characterTransform.position = new Vector3(gameManager.PlayerLaneXPosition[Unity.Netcode.NetworkManager.Singleton.LocalClientId], 0, 10);
                    _characterTransform.eulerAngles = Vector3.zero;
                    _controller.enabled = true;
                };
    
                networkEvents.OnRaceStarted += delegate
                {
                    CanControl = true;
                }; 

                networkEvents.LocalPlayerCompleteRace += delegate
                {
                    CanControl = false;
                };

                networkEvents.OnPlayerNetworkObjectSpawned();
            }
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
            
            CheckGroundStatus();
            
            _horizontal = PlayerInputs.GetHorizontal();
            _vertical = PlayerInputs.GetVertical();
            _jump = PlayerInputs.Jump();
            _dash = PlayerInputs.Dash();
            _flyJetPack = PlayerInputs.JetPack();
            _activeFall = PlayerInputs.Parachute();
 
            if (_invertedControl)
            {
                _horizontal *= -1;
                _vertical *= -1;
                _jump = PlayerInputs.Dash();
                _dash = PlayerInputs.Jump();
            }

            if (_jump && !HoldingObject)
            {
                Jump(JumpHeight);
            }

            if (_dash && !HoldingObject)
            {
                Dash();
            }

            #region Jetpack

            if (CanControl)
            {
                if (Jetpack && _flyJetPack && JetPackFuel > 0 && !HoldingObject)
                {
                    if (_slowFall)
                    {
                        _slowFall = false;
                        //SlowFallObject.SetActive(false);
                        IsFallObjectEnabled = false;
                    }

                    FlyByJetPack();
                }

                if (_activeFall)
                {
                    _slowFall = !_slowFall;
                    _activeFall = false;
                }
            }
            else
            {
                _horizontal = 0;
                _vertical = 0;
            }

            #endregion

            #region Dash

            if (DashCooldown > 0)
            {
                DashCooldown -= Time.fixedDeltaTime;
            }
            else
            {
                DashCooldown = 0;
            }

            #endregion
            

            SetRunningAnimation((Math.Abs(_horizontal) > 0 || Math.Abs(_vertical) > 0));

            if (!CurrentActivePlatform || !CurrentActivePlatform.CompareTag(Tags.Platform.ToString())) return;
            if (CurrentActivePlatform)
            {
                var newGlobalPlatformPoint = CurrentActivePlatform.TransformPoint(_activeLocalPlatformPoint);
                _moveDirection = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
                if (_moveDirection.magnitude > 0.01f)
                {
                    _controller.Move(_moveDirection);
                }

                if (!CurrentActivePlatform) return;

                var newGlobalPlatformRotation = CurrentActivePlatform.rotation * _activeLocalPlatformRotation;
                var rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(_activeGlobalPlatformRotation);
                
                rotationDiff = Quaternion.FromToRotation(rotationDiff * Vector3.up, Vector3.up) * rotationDiff;
                _characterTransform.rotation = rotationDiff * _characterTransform.rotation;
                _characterTransform.eulerAngles = new Vector3(0, _characterTransform.eulerAngles.y, 0);

                UpdateMovingPlatform();
            }
            else
            {
                if (!(_moveDirection.magnitude > 0.01f)) return;
                _moveDirection = Vector3.Lerp(_moveDirection, Vector3.zero, Time.deltaTime);
                _controller.Move(_moveDirection);
            }
        }

        private void FixedUpdate()
        {
            if (!IsOwner)
            {
                return;
            }
            
            if (CanControl)
            {
                //JetPackObject.SetActive(Jetpack && _flyJetPack && JetPackFuel > 0 && !HoldingObject);
                IsJetPackEnabled = Jetpack && _flyJetPack && JetPackFuel > 0 && !HoldingObject;
                
                if (HaveSlowFall && !_isGrounded && _slowFall)
                {
                    SlowFall();
                }
                else
                {
                    //SlowFallObject.SetActive(false);
                    IsFallObjectEnabled = false;
                    _slowFall = false;
                }
            }

            _forward = _cameraTransform.TransformDirection(Vector3.forward);
            _forward.y = 0f;
            _forward = _forward.normalized;
            _right = new Vector3(_forward.z, 0.0f, -_forward.x);

            _move = (_horizontal * _right + _vertical * _forward);
            _direction = (_horizontal * _right + _vertical * _forward);
            
            if (!_isCorrectGrounded && _isGrounded)
            {
                _move.x += (1f - _hitNormal.y) * _hitNormal.x * (1f - SlideFriction);
                _move.z += (1f - _hitNormal.y) * _hitNormal.z * (1f - SlideFriction);
            }

            _move.Normalize();
            
            if (!_slowFall && _controller.enabled)
            {
                _controller.Move(Time.deltaTime * RunningSpeed * _move);
            }

            _isCorrectGrounded = (Vector3.Angle(Vector3.up, _hitNormal) <= SlopeLimit);

            if (_direction != Vector3.zero)
            {
                transform.forward = _direction;
            }

            if (_velocity.y >= -MaxDownYVelocity)
            {
                _velocity.y += Gravity * Time.deltaTime;
            }
            
            if (Swimming)
            {
                _velocity.y = 0;
            }

            _velocity.x /= 1 + DragForce.x * Time.deltaTime;
            _velocity.y /= 1 + DragForce.y * Time.deltaTime;
            _velocity.z /= 1 + DragForce.z * Time.deltaTime;
            if (_controller.enabled)
            {
                _controller.Move(_velocity * Time.deltaTime);
            }

            SetGroundedState();
        }

        #region Rpc
        
        public void ChangeDifficulty(int value)
        {
            gameManager.Difficulty = (GameDifficulty)value;
            lobbyPage.GameDifficultyChangedByServer(value);
        }
        
        [ServerRpc(RequireOwnership = false)]
        void JetPackSetActiveServerRPC(bool value)
        {
            JetPackSetActiveClientRPC(value);
        }
        
        [ClientRpc(RequireOwnership = false)]
        void JetPackSetActiveClientRPC(bool value)
        {
            JetPackObject.SetActive(value);
        }

        [ServerRpc(RequireOwnership = false)]
        void FallObjectSetActiveServerRPC(bool value)
        {
            FallObjectSetActiveClientRPC(value);
        }
        
        [ClientRpc(RequireOwnership = false)]
        void FallObjectSetActiveClientRPC(bool value)
        {
            SlowFallObject.SetActive(value);
        }

        [ServerRpc(RequireOwnership = false)]
        void SpawnJumpEffectServerRpc()
        {
            SpawnJumpEffectClientRpc();
        }
        
        [ClientRpc]
        void SpawnJumpEffectClientRpc()
        {
            Instantiate(JumpEffect, transform.position, _characterTransform.rotation);
        }
        
        #endregion
        
        #region Abilities

        public void Jump(float jumpHeight)
        {
            if (!CanJump || !CanControl)
            {
                return;
            }

            CurrentActivePlatform = null;
            _slowFall = false;
            //SlowFallObject.SetActive(false);
            IsFallObjectEnabled = false;

            if (_isGrounded)
            {
                _hitNormal = Vector3.zero;
                SetJumpAnimation();
                _doubleJump = true;
                _velocity.y = 0;
                _velocity.y += Mathf.Sqrt(jumpHeight * -2f * Gravity);

                if (JumpEffect)
                {
                    //GameObject jumpEffect = Instantiate(JumpEffect, LowZonePosition.position, LowZonePosition.rotation);
                    SpawnJumpEffectServerRpc();
                }
            }
            else if (CanDoubleJump && _doubleJump)
            {
                _doubleJump = false;
                _velocity.y = 0;
                _velocity.y += Mathf.Sqrt(jumpHeight * -2f * Gravity);

                if (JumpEffect)
                {
                    //GameObject jumpEffect = Instantiate(JumpEffect, LowZonePosition.position, LowZonePosition.rotation);
                    SpawnJumpEffectServerRpc();
                }
            }
        }

        public void Dash()
        {
            if (!CanDash || DashCooldown > 0 || _flyJetPack || !CanControl)
            {
                return;
            }

            DashCooldown = _dashCooldown;

            if (DashEffect)
            {
                //GameObject jumpEffect = Instantiate(JumpEffect, transform.position, _characterTransform.rotation);
                SpawnJumpEffectServerRpc();
            }

            SetDashAnimation();
            StartCoroutine(Dashing(DashForce / 10));
            _velocity += Vector3.Scale(transform.forward,
                DashForce * new Vector3((Mathf.Log(1f / (Time.deltaTime * DragForce.x + 1)) / -Time.deltaTime),
                    0, (Mathf.Log(1f / (Time.deltaTime * DragForce.z + 1)) / -Time.deltaTime)));
        }

        private void FlyByJetPack()
        {
            JetPackFuel -= Time.deltaTime * FuelConsumeSpeed;
            _velocity.y = 0;
            _velocity.y += Mathf.Sqrt(JetPackForce * -2f * Gravity);
        }

        private void SlowFall()
        {
            //SlowFallObject.SetActive(true);
            IsFallObjectEnabled = true;
            
            _controller.Move(transform.forward * SlowFallForwardSpeed);
            _velocity.y = 0;
            _velocity.y += -SlowFallSpeed;
        }

        public void AddFuel(float fuel)
        {
            JetPackFuel += fuel;
            if (JetPackFuel > JetPackMaxFuelCapacity)
            {
                JetPackFuel = JetPackMaxFuelCapacity;
            }

            Debug.Log("Fuel +" + fuel);
        }

        public void ResetOriginalSpeed()
        {
            var holdComponent = GetComponent<HoldObjects>();
            if (holdComponent && holdComponent.Busy)
            {
                RunningSpeed = holdComponent.CarryMovementSpeed;
            }
            else
            {
                RunningSpeed = _originalRunningSpeed;
            }
        }

        public void LookAtTarget(Transform target)
        {
            transform.LookAt(target, Vector3.up);
            transform.rotation = Quaternion.Euler(0, _characterTransform.eulerAngles.y, 0);
        }

        public void ChangeSpeed(float speed)
        {
            RunningSpeed = speed;
        }

        public void ChangeSpeedInTime(float speedPlus, float time)
        {
            StartCoroutine(ModifySpeedByTime(speedPlus, time));
        }

        public void InvertPlayerControls(float invertTime)
        {
            if (!_invertedControl)
            {
                StartCoroutine(InvertControls(invertTime));
            }
        }

        #endregion

        #region Activate Deactivate

        public void ActivateDeactivateJump(bool canJump)
        {
            CanJump = canJump;
        }

        public void ActivateDeactivateDoubleJump(bool canDoubleJump)
        {
            if (canDoubleJump)
            {
                CanJump = true;
            }

            CanDoubleJump = canDoubleJump;
        }

        public void ActivateDeactivateDash(bool canDash)
        {
            CanDash = canDash;
        }

        public void ActivateDeactivateSlowFall(bool canSlowFall)
        {
            HaveSlowFall = canSlowFall;
        }

        public void ActivateDeactivateJetpack(bool haveJetPack)
        {
            Jetpack = haveJetPack;
        }

        #endregion

        #region Collision

        private void UpdateMovingPlatform()
        {
            _activeGlobalPlatformPoint = transform.position;
            _activeLocalPlatformPoint = CurrentActivePlatform.InverseTransformPoint(_characterTransform.position);
            
            _activeGlobalPlatformRotation = transform.rotation;
            _activeLocalPlatformRotation =
                Quaternion.Inverse(CurrentActivePlatform.rotation) * _characterTransform.rotation;
        }

        private void CheckGroundStatus()
        {
#if UNITY_EDITOR
            Debug.DrawLine(
                transform.position + (Vector3.up * 0.1f),
                transform.position + Vector3.down * (_controller.height / 2 + 0.2f),
                Color.red
            );
#endif
            
            var grounded = Physics.Raycast(transform.position, Vector3.down, _controller.height / 2 + 0.2f);

            _isGrounded = grounded || _controller.isGrounded;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _hitNormal = hit.normal;

            if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.41)
            {
                if (CurrentActivePlatform == hit.collider.transform) return;
                CurrentActivePlatform = hit.collider.transform;
                UpdateMovingPlatform();
            }
            else
            {
                CurrentActivePlatform = null;
            }

            var body = hit.collider.attachedRigidbody;

            if (body == null || body.isKinematic)
            {
                return;
            }

            if (hit.moveDirection.y < -0.3)
            {
                GetComponent<HoldObjects>().BadPosition = true;
                return;
            }

            GetComponent<HoldObjects>().BadPosition = false;

            var pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            if (PushInFixedDirections)
            {
                body.linearVelocity = pushDir * PushPower;
            }
            else
            {
                body.AddForceAtPosition(pushDir * (PushPower * 10), hit.point);
            }
        }

        #endregion
        
        #region Animator

        private void SetRunningAnimation(bool run)
        {
            PlayerAnimator.SetBool(AnimatorParameters.Running.ToString(), run);
        }

        private void SetJumpAnimation()
        {
            PlayerAnimator.SetTrigger(AnimatorParameters.Jump.ToString());
        }

        private void SetDashAnimation()
        {
            PlayerAnimator.SetTrigger(AnimatorParameters.Dash.ToString());
        }

        private void SetGroundedState()
        {
            if (PlayerAnimator.GetBool(AnimatorParameters.Grounded.ToString()) != _isGrounded)
            {
                PlayerAnimator.SetBool(AnimatorParameters.Grounded.ToString(), _isGrounded);
            }
        }

        #endregion

        #region Coroutine

        public IEnumerator DeactivatePlayerControlByTime(float time)
        {
            _controller.enabled = false;
            CanControl = false;
            yield return new WaitForSeconds(time);
            CanControl = true;
            _controller.enabled = true;
        }

        private IEnumerator Dashing(float time)
        {
            CanControl = false;
            if (!_isGrounded)
            {
                Gravity = 0;
                _velocity.y = 0;
            }

            yield return new WaitForSeconds(time);
            CanControl = true;
            Gravity = _gravity;
        }

        private IEnumerator ModifySpeedByTime(float speedPlus, float time)
        {
            if (RunningSpeed + speedPlus > 0)
            {
                RunningSpeed += speedPlus;
            }
            else
            {
                RunningSpeed = 0;
            }

            yield return new WaitForSeconds(time);
            RunningSpeed = _originalRunningSpeed;
        }

        private IEnumerator InvertControls(float invertTime)
        {
            yield return new WaitForSeconds(0.1f);
            _invertedControl = true;
            yield return new WaitForSeconds(invertTime);
            _invertedControl = false;
        }

        #endregion

        
        
    }
}

public enum AnimatorParameters
{
    Running,
    Jump,
    Dash,
    Grounded
}