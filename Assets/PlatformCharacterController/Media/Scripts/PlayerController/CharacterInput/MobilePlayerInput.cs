using System.Collections;
using System.Collections.Generic;
using TopDownShooter;
using UnityEngine;

namespace PlatformCharacterController
{
    public class MobilePlayerInput : Inputs
    {
        [HideInInspector] public Joystick MobileJoystick;
        [HideInInspector] public MovementCharacterController MovementCharacterController;
        [HideInInspector] public HoldObjects HoldObjectsController;
        private bool _jetPack;
        private bool _parachute;

        public override void OnNetworkSpawn()
        {
            MobileJoystick = FindObjectOfType<Joystick>(true);
            MovementCharacterController = GetComponent<MovementCharacterController>();
            HoldObjectsController = GetComponent<HoldObjects>();
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
            return false;
        }

        public override bool Dash()
        {
            return false;
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

        public void DropOrCarryOnButton()
        {
            HoldObjectsController.DropOrCarryItem();
        }

        public void MakeJump()
        {
            MovementCharacterController.Jump(MovementCharacterController.JumpHeight);
        }

        public void MakeDash()
        {
            MovementCharacterController.Dash();
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