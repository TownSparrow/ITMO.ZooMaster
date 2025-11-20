using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DogReaction : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private Animator _animator;
  [SerializeField] private Aviaries _aviaries;

  private Vector3 _startEulerAngles;
  private Coroutine _turnCoroutine;
  private Coroutine _spinCoroutine;

  private void Awake()
  {
    if (_animator == null)
      _animator = GetComponent<Animator>();
    _startEulerAngles = transform.eulerAngles;
  }

  private void OnEnable()
  {
    if (_aviaries == null) return;

    _aviaries.GoodAction += OnGoodAction;
    _aviaries.ReleasedAnimals += OnReleasedAnimals;
    _aviaries.Interacted += OnInteracted;
    _aviaries.BadAction += OnBadAction;
  }

  private void OnDisable()
  {
    if (_aviaries == null) return;

    _aviaries.GoodAction -= OnGoodAction;
    _aviaries.ReleasedAnimals -= OnReleasedAnimals;
    _aviaries.Interacted -= OnInteracted;
    _aviaries.BadAction -= OnBadAction;
  }

  private void OnGoodAction()
  {
    JumpAndSpin();
  }
  private void OnReleasedAnimals(System.Collections.Generic.List<Animal> animals)
  {
    //Jump();
  }
  private void OnInteracted()
  {
    //Jump();
  }
  private void OnBadAction()
  {
    //Jump();
    TurnLeftRight();
  }

  public void Jump(float delay = 0f)
  {
    PlayAnimation("jump", delay);
  }

  public void JumpAndSpin(float delay = 0f)
  {
    Jump(delay);
    SpinSmooth();
  }

  private Coroutine _animationTask;

  public void PlayAnimation(string name, float delay = 0f)
  {
    if (_animationTask != null)
      StopCoroutine(_animationTask);
    _animationTask = StartCoroutine(PlayAnimationAfter(name, delay));
  }

  private IEnumerator PlayAnimationAfter(string name, float delay)
  {
    if (delay > 0f)
      yield return new WaitForSeconds(delay);
    if (_animator != null)
      _animator.SetTrigger(name);
  }

  private void SpinSmooth()
  {
    if (_spinCoroutine != null)
      StopCoroutine(_spinCoroutine);
    _spinCoroutine = StartCoroutine(SpinSmoothRoutine());
  }

  private IEnumerator SpinSmoothRoutine()
  {
    float duration = 0.5f;
    float startY = transform.eulerAngles.y;
    float targetY = startY + 360f;
    float timer = 0f;
    while (timer < duration)
    {
      float t = timer / duration;
      float currentY = Mathf.Lerp(startY, targetY, t);
      transform.eulerAngles = new Vector3(_startEulerAngles.x, currentY, _startEulerAngles.z);
      timer += Time.deltaTime;
      yield return null;
    }
    transform.eulerAngles = new Vector3(_startEulerAngles.x, targetY, _startEulerAngles.z);
    _spinCoroutine = null;
  }

  private void TurnLeftRight()
  {
    if (_turnCoroutine != null)
      StopCoroutine(_turnCoroutine);
    _turnCoroutine = StartCoroutine(TurnLeftRightRoutine());
  }

  private IEnumerator TurnLeftRightRoutine()
  {
    float angle = 30f;
    float totalDuration = 0.3f;
    float thirdDuration = totalDuration / 3f;
    Vector3 start = _startEulerAngles;
    Vector3 left = start + new Vector3(0, -angle, 0);
    Vector3 right = start + new Vector3(0, angle, 0);

    float timer = 0;
    while (timer < thirdDuration)
    {
      float t = timer / thirdDuration;
      transform.eulerAngles = Vector3.Slerp(start, left, t);
      timer += Time.deltaTime;
      yield return null;
    }
    transform.eulerAngles = left;

    timer = 0;
    while (timer < thirdDuration)
    {
      float t = timer / thirdDuration;
      transform.eulerAngles = Vector3.Slerp(left, right, t);
      timer += Time.deltaTime;
      yield return null;
    }
    transform.eulerAngles = right;

    timer = 0;
    while (timer < thirdDuration)
    {
      float t = timer / thirdDuration;
      transform.eulerAngles = Vector3.Slerp(right, start, t);
      timer += Time.deltaTime;
      yield return null;
    }
    transform.eulerAngles = start;
    _turnCoroutine = null;
  }
}
