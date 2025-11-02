using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Cell: MonoBehaviour
{
    private enum State
    {
        Free,
        Busy,
        Used,
        ReadyMerge,
        NoMerge,
    }

    [SerializeField] private float _speedMoveItem = 20f;
    [SerializeField] private Color[] _colors;
    [SerializeField] private float[] _scales;
    [SerializeField] private SpriteRenderer _verificationRenderer;
    
    private BoxCollider _boxCollider;
    private SpriteRenderer _spriteRenderer;
    private Item _item;
    private State _state;
    private Vector3 _initialScale;
    private bool _verification;
    private float _alpha;

    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            _state = value == null ? State.Free : State.Busy;
            
            _verification = value && _item.NextItem == EItem.None;
        }
    }

    public bool Used
    {
        set => _state = value ? State.Used : _item == null ? State.Free : State.Busy;
    }
    
    public bool ReadyMerge
    {
        get => _state == State.ReadyMerge ? true : false;
        set => _state = value == true ? State.ReadyMerge : State.NoMerge;
    }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _initialScale = _spriteRenderer.transform.localScale;
        _boxCollider.isTrigger = true;
        
        _state = State.Free;
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Free:
                Animation(_colors[0], _scales[0]);
                break;
            case State.Busy:
                var col = _colors[1];
                
                if (Item && Item.NextItem == EItem.None)
                    col = _colors[4];
                
                Animation(col, _scales[1]);
                break;
            case State.ReadyMerge:
                Animation(_colors[2], _scales[2]);
                break;
            case State.NoMerge:
                Animation(_colors[3], _scales[3]);
                break;
        }
        
        if(!ReferenceEquals(_item, null)) 
            MoveUnitToCentre();

        ChangeColorVerification();
    }

    private void Animation(Color color, float scale)
    {
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, color, Time.deltaTime * 5f);
        _spriteRenderer.transform.localScale = Vector3.Lerp(_spriteRenderer.transform.localScale, _initialScale * scale, Time.deltaTime * 5f);
    }

    private void ChangeColorVerification()
    {
        _alpha = Mathf.MoveTowards(_alpha, _verification ? 1f : 0f, Time.deltaTime * 5f);
        if(_alpha != _verificationRenderer.color.a)
            _verificationRenderer.SetAlpha(_alpha);
    }
    
    private void MoveUnitToCentre()
    {
        var unitPos = _item.transform.position;
        
        if (unitPos != transform.position)
        {
            var speed = Vector3.Distance(unitPos, transform.position) * _speedMoveItem * Time.smoothDeltaTime;
            _item.transform.position = Vector3.MoveTowards(unitPos, transform.position, speed);
        }
    }

    public void Clear()
    {
        if(_item == null) return;
        
        PoolManager.SetPool(_item);
        Item = null;
    }
}