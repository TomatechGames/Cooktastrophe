using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RigidbodyMoveProvider : ActionBasedContinuousMoveProvider
{
    Rigidbody m_Rigidbody;
    bool m_AttemptedGetRigidbody;

    protected override void MoveRig(Vector3 translationInWorldSpace)
    {
        var xrOrigin = system.xrOrigin;
        if (xrOrigin == null)
            return;

        FindRigidbody();

        if (m_Rigidbody != null)
        {
            if (CanBeginLocomotion() && BeginLocomotion())
            {
                // Note that calling Move even with Vector3.zero will have an effect by causing isGrounded to update
                m_Rigidbody.velocity = (translationInWorldSpace / Time.deltaTime);
                EndLocomotion();
            }
            else
                m_Rigidbody.velocity = Vector3.zero;
        }
        else
            base.MoveRig(translationInWorldSpace);
    }

    void FindRigidbody()
    {
        var xrOrigin = system.xrOrigin;
        if (xrOrigin == null)
            return;

        // Save a reference to the optional CharacterController on the rig GameObject
        // that will be used to move instead of modifying the Transform directly.
        if (m_Rigidbody == null && !m_AttemptedGetRigidbody)
        {
            m_Rigidbody = xrOrigin.Origin.GetComponent<Rigidbody>();
            m_AttemptedGetRigidbody = true;
        }
    }
}
