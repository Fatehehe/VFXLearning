using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnProjectilesScript : MonoBehaviour
{
	public bool use2D;
	public bool cameraShake;
	public Text effectName;
	public RotateToMouseScript rotateToMouse;
	public GameObject firePoint;
	public Camera mainCamera; // Use only one camera
	public List<GameObject> VFXs = new List<GameObject>();

	private int count = 0;
	private float timeToFire = 0f;
	private GameObject effectToSpawn;

	void Start()
	{
		// Camera check
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
			if (mainCamera == null)
			{
				Debug.LogError("Please assign a Camera in inspector");
				return;
			}
		}

		// Setup first effect
		if (VFXs.Count > 0)
			effectToSpawn = VFXs[0];
		else
			Debug.LogError("Please assign one or more VFXs in inspector");

		if (effectName != null && effectToSpawn != null)
			effectName.text = effectToSpawn.name;

		// Setup RotateToMouseScript
		if (rotateToMouse != null)
		{
			rotateToMouse.SetCamera(mainCamera);
			if (use2D)
				rotateToMouse.Set2D(true);
			rotateToMouse.StartUpdateRay();
		}
		else
		{
			Debug.LogWarning("RotateToMouseScript is not assigned.");
		}
	}

	void Update()
	{
		if (VFXs.Count > 0 && effectToSpawn != null)
		{
			// Fire with Space or Left Click
			if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && Time.time >= timeToFire)
			{
				timeToFire = Time.time + 1f / effectToSpawn.GetComponent<ProjectileMoveScript>().fireRate;
				SpawnVFX();
			}

			if (Input.GetKeyDown(KeyCode.D))
				Next();
			if (Input.GetKeyDown(KeyCode.A))
				Previous();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
			ToggleCameraShake();
		if (Input.GetKeyDown(KeyCode.X))
			ZoomIn();
		if (Input.GetKeyDown(KeyCode.Z))
			ZoomOut();
	}

	public void SpawnVFX()
	{
		GameObject vfx;

		var cameraShakeScript = mainCamera.GetComponent<CameraShakeSimpleScript>();
		if (cameraShake && cameraShakeScript != null)
			cameraShakeScript.ShakeCamera();

		if (firePoint != null)
		{
			vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
			if (rotateToMouse != null)
				vfx.transform.localRotation = rotateToMouse.GetRotation();
		}
		else
		{
			vfx = Instantiate(effectToSpawn);
		}

		// Ensure particle system plays
		var ps = vfx.GetComponent<ParticleSystem>();
		if (vfx.transform.childCount > 0 && ps == null)
			ps = vfx.transform.GetChild(0).GetComponent<ParticleSystem>();

		if (ps != null) ps.Play();
	}

	public void Next()
	{
		count++;
		if (count >= VFXs.Count)
			count = 0;

		effectToSpawn = VFXs[count];
		if (effectName != null)
			effectName.text = effectToSpawn.name;
	}

	public void Previous()
	{
		count--;
		if (count < 0)
			count = VFXs.Count - 1;

		effectToSpawn = VFXs[count];
		if (effectName != null)
			effectName.text = effectToSpawn.name;
	}

	public void ToggleCameraShake()
	{
		cameraShake = !cameraShake;
	}

	public void ZoomIn()
	{
		if (mainCamera == null) return;

		if (!mainCamera.orthographic)
		{
			if (mainCamera.fieldOfView > 20)
				mainCamera.fieldOfView -= 5;
		}
		else
		{
			if (mainCamera.orthographicSize > 1)
				mainCamera.orthographicSize -= 0.5f;
		}
	}

	public void ZoomOut()
	{
		if (mainCamera == null) return;

		if (!mainCamera.orthographic)
		{
			if (mainCamera.fieldOfView < 100)
				mainCamera.fieldOfView += 5;
		}
		else
		{
			if (mainCamera.orthographicSize < 10)
				mainCamera.orthographicSize += 0.5f;
		}
	}
}
