using UnityEngine;
using UnityEngine.InputSystem;

namespace PlatformCharacterController
{
    [RequireComponent(typeof(MovementCharacterController))]
    public class PlayerInput : Inputs
    {
        public override float GetHorizontal()
        {
            var kb = Keyboard.current;
            if (kb == null) return 0f;
            float h = 0f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
            return h;
        }

        public override float GetVertical()
        {
            var kb = Keyboard.current;
            if (kb == null) return 0f;
            float v = 0f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v += 1f;
            return v;
        }

        public override bool Jump()
        {
            return Keyboard.current?.spaceKey.wasPressedThisFrame ?? false;
        }

        public override bool Dash()
        {
            return Keyboard.current?.fKey.wasPressedThisFrame ?? false;
        }

        public override bool JetPack()
        {
            return Keyboard.current?.xKey.isPressed ?? false;
        }

        public override bool Parachute()
        {
            return Keyboard.current?.rKey.wasPressedThisFrame ?? false;
        }

        public override bool DropCarryItem()
        {
            return Keyboard.current?.kKey.wasPressedThisFrame ?? false;
        }
    }
}
