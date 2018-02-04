using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPiece : MonoBehaviour
{
    public void DestroyThisPiece(float time)
    {
        Destroy(this.gameObject, time);
    }
}
