using System;
using UnityEngine;

public class HitEffect : MonoBehaviour {
    private Animator _animator;
    private float _timer;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _timer = _animator.runtimeAnimatorController.animationClips[0].length;
    }

    private void FixedUpdate() {
        if (_timer > 0f) {
            _timer -= Time.fixedDeltaTime;
            if (_timer <= 0f) {
                Destroy(gameObject);
            }
        }
    }
}