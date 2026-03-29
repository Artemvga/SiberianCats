using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Player;
using EPOOutline;
using Items;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Info")]
    [SerializeField] private string _doorName = "Дверь";
    [SerializeField] private string _doorType = "Дверь";
    [SerializeField] [TextArea] private string _description = "Обычная дверь";
    [SerializeField] [TextArea] private string _lore = "";

    [Header("Animation")]
    [SerializeField] private float _rotationAngle = 90f;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Transform _hinge;                     // объект, который поворачивается
    [SerializeField] private Vector3 _rotationAxis = Vector3.forward; // поворот вокруг Z

    [Header("Events")]
    public UnityEvent OnDoorOpened;
    public UnityEvent OnDoorClosed;

    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private bool _isOpen = false;
    private bool _isAnimating = false;
    private Outlinable _outlinable;

    public string ItemName => _doorName;
    public string ItemType => _doorType;
    public string Description => _description;
    public string Lore => _lore;
    public bool ShouldShowRequirement => true;

    private void Start()
    {
        // Подсветка
        _outlinable = GetComponent<Outlinable>();
        if (_outlinable == null)
            _outlinable = gameObject.AddComponent<Outlinable>();
        _outlinable.enabled = false;

        if (_hinge == null) _hinge = transform;

        // Сохраняем начальный поворот
        _closedRotation = _hinge.localRotation;
        _openRotation = _closedRotation * Quaternion.AngleAxis(_rotationAngle, _rotationAxis);
    }

    public void OnFocus()
    {
        if (_outlinable != null) _outlinable.enabled = true;
    }

    public void OnDefocus()
    {
        if (_outlinable != null) _outlinable.enabled = false;
    }

    public bool CanInteract(PlayerTools tools) => true;

    public void Interact()
    {
        if (_isAnimating) return;
        StartCoroutine(RotateDoor(!_isOpen));
    }

    public string GetInteractionMessage()
    {
        return _isOpen ? "Нажмите E, чтобы закрыть дверь" : "Нажмите E, чтобы открыть дверь";
    }

    private IEnumerator RotateDoor(bool open)
    {
        _isAnimating = true;
        Quaternion startRot = _hinge.localRotation;
        Quaternion targetRot = open ? _openRotation : _closedRotation;
        float elapsed = 0f;

        while (elapsed < _animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _animationDuration;
            _hinge.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        _hinge.localRotation = targetRot;
        _isAnimating = false;
        _isOpen = open;
        if (_isOpen) OnDoorOpened?.Invoke();
        else OnDoorClosed?.Invoke();
    }
}