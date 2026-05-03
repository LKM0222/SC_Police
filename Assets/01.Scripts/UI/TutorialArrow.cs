using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    [SerializeField] float speed;
    private void OnEnable()
    {
        StartCoroutine(UpDownCoroutine());
    }

    IEnumerator UpDownCoroutine()
    {
        while (true)
        {
            for (float y = 0; y < 0.5f; y += speed)
                {
                    transform.localPosition += Vector3.up * speed;
                    yield return new WaitForSeconds(0.01f);
                }

            for (float y = 0.5f; y > 0f; y -= speed)
            {
                transform.localPosition -= Vector3.up * speed;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
