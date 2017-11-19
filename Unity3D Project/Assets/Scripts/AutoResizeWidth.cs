using UnityEngine;
using System.Collections;
/*

Example

Original

const DesingHeight: 1024
const DesingWidth: 768

iPhone

DeviceHeight: 1920
DeviceWidth: 1080

1920 / 1024 = scale

1080 / width = scale

1080 = scale * width

1080 / scale = width

width = 1080 / scale


--- solution ----

forumulas
scale = DeviceHeight / DesingHeight
widthInDesingHeight = DeviceWidth / scale
widthRatio = widthInDesingHeight / DesingWidth

numbers plugged in
scale = 1920 / 1024 
widthInDesingHeight = 1080 / scale

scale = 1920 / 1024 = 1.875
widthInDesingHeight = 1080 / 1.875

*/

[RequireComponent(typeof( RectTransform ))]
public class AutoResizeWidth : MonoBehaviour
{
	float originalWidth;
	float originalHeight;
	RectTransform rectTransform;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		originalWidth = rectTransform.rect.width;
		originalHeight = rectTransform.rect.height;
		Update();
	}

	void Update()
	{
		updateScreenRes();
	}

	void updateScreenRes()
	{
		const float DesingHeight = 768f;
		const float DesingWidth = 1024f;
		float DeviceHeight = Screen.height;
		float DeviceWidth = Screen.width;

		//forumulas
		float scale = DeviceHeight / DesingHeight;
		float widthInDesingHeight = DeviceWidth / scale;
		float widthRatio = widthInDesingHeight / DesingWidth;

		//resize the width with the width ratio value
		rectTransform.sizeDelta = new Vector2(originalWidth * widthRatio,originalHeight);
	}
}
