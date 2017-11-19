using UnityEngine;
using System.Collections;

public class ResizeSquaredMenu : MonoBehaviour
{
	float originalWidth;
	RectTransform parentRect;

	void Awake() {
		originalWidth = GetComponent<RectTransform>().rect.width;
		parentRect = transform.parent.GetComponent<RectTransform>();
		Update();
	}
	void Update()
	{
		UpdateScreenRes();
	}

	void UpdateScreenRes()
	{
		float parentWidth = parentRect.rect.width;
		float scale;
		if (parentWidth < originalWidth)
		{
			//forumulas
			scale = parentWidth / originalWidth;
		} else {
			scale = 1f;
		}
		transform.localScale = new Vector3(scale,scale,scale);
	}
}
