using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCharacterController
{
    public class SwimmingController : MonoBehaviour
    {
        public MovementCharacterController MovementCharacterController;


        private void SetSwimmingState(bool swimming)
        {
            MovementCharacterController.Swimming = swimming;
            MovementCharacterController.PlayerAnimator.SetTrigger("Swim");
            MovementCharacterController.PlayerAnimator.SetBool("Swimming", swimming);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Tags.Water.ToString())) return;
            SetSwimmingState(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(Tags.Water.ToString())) return;
            SetSwimmingState(false);
        }
    }
}