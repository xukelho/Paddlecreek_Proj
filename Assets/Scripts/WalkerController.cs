using UnityEngine;
using UnityEngine.AI;

public class WalkerController : MonoBehaviour
{
    #region Fields

    public float LineWidth = .2f;

    public float FlyingSpeed = .1f;

    public WalkerController ClosestWalker;

    public enum MovementType
    {
        Flat2D,
        Spacial3D
    };

    [Header("Set the movement type before playing")]
    public MovementType Movement = MovementType.Spacial3D;

    NavMeshAgent _agent;
    LineRenderer _lineRenderer;

    Vector3[] _positions = new Vector3[2];

    Vector3 _flyingDestination = new Vector3();

    #endregion Fields

    #region Unity

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (Movement == MovementType.Flat2D)
            _agent.SetDestination(transform.position);
        else
            Destroy(_agent);

        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.widthMultiplier = LineWidth;

        GenerateNewFlyingDestination();
    }

    void Update()
    {
        if (Movement == MovementType.Flat2D)
        {
            if (_agent.remainingDistance <= 1)
                GenerateNewWalkingDestination();
        }
        else
        {
            if ((_flyingDestination - transform.position).magnitude < 1)
                GenerateNewFlyingDestination();
            else
            {
                float step = FlyingSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _flyingDestination, step);
            }
        }

        ConnectToClosestWalker();
    }

    #endregion Unity

    #region Methods

    public void Activate()
    {
        gameObject.SetActive(true);
        _lineRenderer.enabled = true;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        _lineRenderer.enabled = false;
    }

    public void MarkAsSolo()
    {
        ClosestWalker = null;

        _positions = new Vector3[2]
            {
                transform.position,
                transform.position
            };

        _lineRenderer.SetPositions(_positions);
    }

    void GenerateNewWalkingDestination()
    {
        _agent.SetDestination(GameController.GetRandomPositionOnFloor());
    }

    void GenerateNewFlyingDestination()
    {
        _flyingDestination.x = Random.Range(-GameController.Instance.MovementLimits.x, GameController.Instance.MovementLimits.x);
        _flyingDestination.y = Random.Range(-GameController.Instance.MovementLimits.y, GameController.Instance.MovementLimits.y);
        _flyingDestination.z = Random.Range(-GameController.Instance.MovementLimits.z, GameController.Instance.MovementLimits.z);
    }

    void ConnectToClosestWalker()
    {
        if (ClosestWalker != null)
        {
            _positions = new Vector3[2]
            {
                transform.position,
                ClosestWalker.transform.position
            };
            _lineRenderer.SetPositions(_positions);
        }
        else
            MarkAsSolo();
    }

    #endregion Methods
}