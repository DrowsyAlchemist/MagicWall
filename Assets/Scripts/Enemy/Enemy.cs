using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyStateMachine))]
[RequireComponent(typeof(CharacterAnimator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, ITakeDamage
{
    [SerializeField] private int _initialHealth = 3;
    [SerializeField] private HealthRenderer _healthRenderer;
    [SerializeField] private int _reward = 2;
    [SerializeField] private ElementType _element;

    private EnemyStateMachine _stateMachine;
    private CharacterAnimator _animator;

    public ElementType Element => _element;
    public int Reward => _reward;

    private Health _health;

    public event Action<Enemy> Died;

    private void Awake()
    {
        _animator = GetComponent<CharacterAnimator>();
        _stateMachine = GetComponent<EnemyStateMachine>();
        _health = new Health(_initialHealth, _element);
        _healthRenderer.Render(_health);
        _health.CurrentHealthChanged += OnHealthChanged;
    }

    private void OnDestroy()
    {
        _health.CurrentHealthChanged -= OnHealthChanged;
    }

    public void TakeDamage(float damage, ElementType transmittingElement)
    {
        _health.TakeDamage(damage, transmittingElement);
    }

    private void OnHealthChanged(float health)
    {
        if (health <= 0)
        {
            StartCoroutine(Die());
            Died?.Invoke(this);
        }
    }

    private IEnumerator Die()
    {
        _stateMachine.Pause();
        _animator.PlayDie();
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(_animator.GetCurrentAnimationLength());
        Destroy(gameObject);
    }
}