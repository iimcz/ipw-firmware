using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class FlagComponent : MonoBehaviour
{
    public Transform Target;
    public string Text;
    public TextMeshPro TextMesh;

    public UnityEvent OnActivate;
    public UnityEvent OnSelect;

    public FlagComponent NextFlag;
    public FlagComponent PreviousFlag;
    
    private LineRenderer _renderer;
    private Vector3 _lastPos;
    private Quaternion _lastRot;

    public void Activate()
    {
        OnActivate.Invoke();
    }

    public void Select()
    {
        OnSelect.Invoke();
        // TODO: Somehow optionally highlight (probably text color)
    }
    
    void Start()
    {
        _renderer = GetComponent<LineRenderer>();
        _renderer.useWorldSpace = true;
        _renderer.positionCount = 3;

        _lastPos = transform.position;
        _lastRot = transform.rotation;
    }

    void Update()
    {
        if (_lastPos == transform.position && _lastRot == transform.rotation && Text == TextMesh.text) return;
        _lastPos = transform.position;
        _lastRot = transform.rotation;
        
        TextMesh.text = Text;
        TextMesh.ForceMeshUpdate();

        var bounds = TextMesh.textBounds;
        var textTransform = TextMesh.transform;
        
        _renderer.SetPosition(0, Target.position);
        var right = transform.right;
        right.Scale(bounds.min);
        
        _renderer.SetPosition(1, textTransform.TransformPoint(bounds.min));
        _renderer.SetPosition(2, textTransform.TransformPoint(bounds.min + new Vector3(bounds.size.x, 0, 0)));
    }
}
