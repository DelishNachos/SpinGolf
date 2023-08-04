using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimation : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private List<float> specificTimes;
    private SpriteRenderer SR;

    [SerializeField] private bool useGeneralTime;
    [SerializeField] private float timeBetweenFrames;

	private void Awake()
	{
        SR = GetComponent<SpriteRenderer>();
	}

	// Start is called before the first frame update
	void Start()
    {
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
	{
        for(int i = 0; i < sprites.Count; i++)
		{
            SR.sprite = sprites[i];
            if (useGeneralTime)
                yield return new WaitForSeconds(timeBetweenFrames);
            else
                yield return new WaitForSeconds(specificTimes[i]);
		}
        StartCoroutine(Animation());
	}
}
