using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlatformCharacterController
{
    public class JumpingPlatform : Platform
    {
        [Tooltip("This is the jumping forze of this plataform")]
        public float jumpForce = 4;

        public Animator platformAnimator;

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag(Tags.Player.ToString())) return;
            //make the player jump
            other.GetComponent<MovementCharacterController>().Jump(jumpForce);
            //animate platform if exist animator
            if (platformAnimator)
            {
                platformAnimator.SetTrigger("In");
            }
        }
    }
}