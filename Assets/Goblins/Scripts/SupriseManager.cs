using System;
using UnityEngine;

public class SupriseManager : MonoBehaviour
{
    [SerializeField] GameObject goblin;
    [SerializeField] GameObject topPoint;
    [SerializeField] GameObject bottomPoint;
    [SerializeField] float delay = 2f;

    private float reverseTimer = 0f;

    private void Start()
    {
        topPoint.SetActive(false);
        bottomPoint.SetActive(false);
        GameEventsBus.Instance.Subscribe<GoblinSurprise>(OnSupriseEvent);
    }

    private void OnSupriseEvent(GoblinSurprise surprise)
    {
        if (goblin.gameObject == surprise.goblin.gameObject)
        {
            topPoint.SetActive(true);
            bottomPoint.SetActive(true);
            reverseTimer = delay;
        }
    }

    private void Update()
    {
        if (reverseTimer > 0)
        {
            reverseTimer -= Time.deltaTime;

            //Wiggle();

            if (reverseTimer <= 0)
            {
                topPoint.SetActive(false);
                bottomPoint.SetActive(false);
            }
        }
    }

    private void Wiggle()
    {
        var angles = topPoint.transform.rotation.eulerAngles;
        angles.z = 15 * Mathf.Sign(reverseTimer);
        topPoint.transform.rotation = Quaternion.Euler(angles);

        var angles2 = bottomPoint.transform.rotation.eulerAngles;
        angles2.z = 15 * Mathf.Sign(reverseTimer);
        bottomPoint.transform.rotation = Quaternion.Euler(angles2);
    }
}
