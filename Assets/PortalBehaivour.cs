using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaivour : MonoBehaviour
{
    Animator _animator;
    bool isDone = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.gm.countPunchCards == 5 && !isDone)
        {
            isDone = true;
            _animator.SetBool("Complete", true);
        }       
    }
}
