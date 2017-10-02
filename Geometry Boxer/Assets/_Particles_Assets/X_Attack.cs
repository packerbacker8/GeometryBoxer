using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class X_Attack : MonoBehaviour {
	public Rigidbody m_Shell;                   // Prefab of the shell.
	public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
	public float m_MaxLaunchForce = 30f;

	private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.

	private void OnEnable()
	{
		// When the tank is turned on, reset the launch force and the UI
		m_CurrentLaunchForce = m_MaxLaunchForce;
	}
		


	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			// ... launch the shell.
			Fire ();
		}
	}


	private void Fire ()
	{
	// Create an instance of the shell and store a reference to it's rigidbody.
		Rigidbody shellInstance =
			Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
		shellInstance.transform.Rotate (0f, 90f, 0f);

		// Set the shell's velocity to the launch force in the fire position's forward direction.
		shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; ;

		// Change the clip to the firing clip and play it.

		// Reset the launch force.  This is a precaution in case of missing button events.
		m_CurrentLaunchForce = m_MaxLaunchForce;
		Destroy (shellInstance, 2);
	}
}
