using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UIJoystick = TopDownShooter.Joystick;

namespace PlatformCharacterController
{
    public class MobilePlayerInput : Inputs
    {
        [HideInInspector] public UIJoystick MobileJoystick;
        [HideInInspector] public MovementCharacterController MovementCharacterController;
        [HideInInspector] public HoldObjects HoldObjectsController;

        [SerializeField] private SoGameConfig gameConfig;
        private float SwipeThreshold => gameConfig != null ? gameConfig.swipeThreshold : 50f;

        private bool _jetPack;
        private bool _parachute;
        private bool _jumpThisFrame;
        private bool _dashThisFrame;

        private void Awake()
        {
            MobileJoystick = FindObjectOfType<UIJoystick>(true);
            MovementCharacterController = GetComponent<MovementCharacterController>();
            HoldObjectsController = GetComponent<HoldObjects>();
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            Touch.onFingerUp += OnFingerUp;
        }

        private void OnDisable()
        {
            Touch.onFingerUp -= OnFingerUp;
            EnhancedTouchSupport.Disable();
        }

        private void OnFingerUp(Finger finger)
        {
            var touch = finger.currentTouch;
            Vector2 delta = touch.screenPosition - finger.screenPosition;

            if (delta.magnitude < SwipeThreshold) return;

            if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
            {
                if (delta.y > 0) _jumpThisFrame = true;
                else _parachute = !_parachute;
            }
            else
            {
                _dashThisFrame = true;
            }
        }

        private void LateUpdate()
        {
            _jumpThisFrame = false;
            _dashThisFrame = false;
        }

        public override float GetHorizontal()
        {
            return MobileJoystick != null ? MobileJoystick.Horizontal : 0f;
        }

        public override float GetVertical()
        {
            return MobileJoystick != null ? MobileJoystick.Vertical : 0f;
        }

        public override bool Jump()
        {
            return _jumpThisFrame;
        }

        public override bool Dash()
        {
            return _dashThisFrame;
        }

        public override bool JetPack()
        {
            return _jetPack;
        }

        public override bool Parachute()
        {
            return _parachute;
        }

        public override bool DropCarryItem()
        {
            return false;
        }

        // Called by UI buttons
        public void MakeJump()
        {
            MovementCharacterController.Jump(MovementCharacterController.JumpHeight);
        }

        public void MakeDash()
        {
            MovementCharacterController.Dash();
        }

        public void DropOrCarryOnButton()
        {
            HoldObjectsController.DropOrCarryItem();
        }

        public void ActiveJetPack(bool active)
        {
            _jetPack = active;
        }

        public void ActiveParachute(bool active)
        {
            _parachute = active;
        }
    }
}
