using System.Collections;
using UnityEngine;

public class FindNearestNeighbour : MonoBehaviour
{
    #region Fields

    WalkerController _walkerController;

    #endregion Fields

    #region Unity

    void Start()
    {
        _walkerController = GetComponent<WalkerController>();

        StartCoroutine(FindClosestWalkerForEachWalker());
    }

    void OnEnable()
    {
        _walkerController?.MarkAsSolo();
        StartCoroutine(FindClosestWalkerForEachWalker());
    }

    #endregion Unity

    #region Methods

    IEnumerator FindClosestWalkerForEachWalker()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f);

            if (GameController.Instance.Walkers.Count == 1)
            {
                _walkerController.MarkAsSolo();
                continue;
            }

            if (GameController.Instance.Walkers.Count < 2)
                continue;

            foreach (var walker in GameController.Instance.Walkers)
            {
                WalkerController closest = FindClosestWalker(walker);
                walker.ClosestWalker = closest;
            }
        }
    }

    WalkerController FindClosestWalker(WalkerController walker)
    {
        float closestDistance = Mathf.Infinity;

        WalkerController closestWalker = null;

        foreach (var walkerToCheck in GameController.Instance.Walkers)
        {
            if (walker == walkerToCheck)
                continue;

            float distance = (walkerToCheck.transform.position - walker.transform.position).magnitude;

            if (distance < closestDistance)
            {
                closestWalker = walkerToCheck;
                closestDistance = distance;
            }
        }

        return closestWalker;
    }

    #endregion Methods
}