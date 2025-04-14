using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreAnim : MonoBehaviour
{
    public Animator animator;
   
    public void Anima()
    {
        animator.SetBool("isIn", true);
    }

    public void Close()
    {
        animator.SetBool("isIn", false);
    }
}
