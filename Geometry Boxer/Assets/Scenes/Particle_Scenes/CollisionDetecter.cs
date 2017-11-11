using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetecter : MonoBehaviour {

	public Rigidbody Character;
	public Rigidbody Enemy;
	public Canvas DamageUI;

//	void Start(){
//		if( DamageUI != null )
//		{
//			var myScriptReference = DamageUI.GetComponent<FloatingText>();
//			if( myScriptReference != null )
//			{
//				myScriptReference.SetText(Damage);
//			}
//		}
//	}

	void OnCollisionEnter (Collision Col)
	{
		if (Col.gameObject == Enemy ) {
			Instantiate (DamageUI);
			Destroy (DamageUI, 1);
		}
	}
}
