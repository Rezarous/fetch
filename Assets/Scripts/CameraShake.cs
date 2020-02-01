using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;
	
	// How long the object should shake for.
	public float shakeDuration = 0.7f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.5f;
	public float decreaseFactor = 1.0f;
	
	Vector3 originalPos;

    public bool isShaking = false;
	
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}
	
	void OnEnable()
	{
		//originalPos = camTransform.localPosition;
	}

	void Update()
	{
		if (shakeDuration > 0)
		{
      isShaking = true;
			camTransform.position = transform.position + Random.insideUnitSphere * shakeAmount;
			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else
		{
            isShaking = false;
			shakeDuration = 0f;
			//camTransform.localPosition = originalPos;
		}
	}
}