using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Fields

    public static GameController Instance;

    [Space(20)]
    public List<WalkerController> Walkers = new List<WalkerController>();

    [Space(20)]
    [Tooltip("Defines the area in wich the objects can fly around")]
    public Vector3 MovementLimits;

    [Space(20)]
    public GameObject Floor;

    [SerializeField] WalkerController WalkerPrefab;
    [SerializeField] GameObject WalkersHolder;
    [SerializeField] TMP_InputField InputField;

    [SerializeField] List<WalkerController> _deactivatedWalkers = new List<WalkerController>();

    #endregion Fields

    #region Unity

    void Start()
    {
        Instance = this;
        InputField.text = "0";
    }

    #endregion Unity

    #region Methods

    public void IncreaseWalkers()
    {
        if (_deactivatedWalkers.Count == 0)
            CreateNewWalker();
        else
        {
            WalkerController firstDeactivated = _deactivatedWalkers[0];

            firstDeactivated.Activate();

            _deactivatedWalkers.Remove(firstDeactivated);
            Walkers.Add(firstDeactivated);
        }

        InputField.text = Walkers.Count().ToString();
    }

    public void DecreaseWalkers()
    {
        if (Walkers.Count == 0)
            return;

        var lastWalker = Walkers.Last();
        Walkers.RemoveAt(Walkers.Count() - 1);

        lastWalker.Deactivate();
        _deactivatedWalkers.Add(lastWalker);

        InputField.text = Walkers.Count().ToString();
    }

    public void SetWalkersNumber()
    {
        int newWalkersCount = int.Parse(InputField.text);

        if (newWalkersCount == Walkers.Count)
            return;

        if (newWalkersCount > Walkers.Count)
        {
            int newWalkersNumber = newWalkersCount - Walkers.Count;

            if (newWalkersNumber <= _deactivatedWalkers.Count)
            {
                for (int i = 0; i < newWalkersNumber; i++)
                {
                    var _deactivatedWalker = _deactivatedWalkers[i];

                    _deactivatedWalker.Activate();
                    Walkers.Add(_deactivatedWalker);
                }

                _deactivatedWalkers.RemoveRange(0, newWalkersNumber);
            }
            else
            {
                foreach (var _deactivatedWalker in _deactivatedWalkers)
                {
                    _deactivatedWalker.Activate();
                    Walkers.Add(_deactivatedWalker);
                }

                newWalkersNumber -= _deactivatedWalkers.Count;

                for (int i = 0; i < newWalkersNumber; i++)
                    CreateNewWalker();

                _deactivatedWalkers.Clear();
            }
        }
        else
        {
            int howManyToRemove = Walkers.Count - newWalkersCount;

            List<WalkerController> toDisable = Walkers.Skip(Walkers.Count() - howManyToRemove).Take(howManyToRemove).ToList();

            Walkers = Walkers.Except(toDisable).ToList();
            _deactivatedWalkers.AddRange(toDisable);

            foreach (var item in toDisable)
                item.Deactivate();
        }
    }

    public static Vector3 GetRandomPositionOnFloor()
    {
        float width = Instance.Floor.transform.localScale.x / 2;
        float length = Instance.Floor.transform.localScale.z / 2;

        Vector3 newDestination = new Vector3(Random.Range(-width, width), Instance.Floor.transform.position.y, Random.Range(-length, length));

        return newDestination;
    }

    void CreateNewWalker()
    {
        WalkerController newWalker = GameObject.Instantiate(WalkerPrefab);
        newWalker.transform.position = GetRandomPositionOnFloor();
        newWalker.transform.parent = WalkersHolder.transform;
        Walkers.Add(newWalker);

        newWalker.name += " " + Walkers.Count;
    }

    #endregion Methods
}