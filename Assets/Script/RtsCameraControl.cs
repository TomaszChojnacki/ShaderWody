using UnityEngine;

public class RtsCameraControl : MonoBehaviour
{
	public float predkoscPrzesuwania = 20f;
	public float predkoscScrollowania = 500f;
	public float maxWysokosc = 180f;
	public float minWysokosc = 80f;

	public float minX = 400f;
	public float maxX = 600f;
	public float minZ = 455f;
	public float maxZ = 655f;

	private Vector3 startowaPozycja;

	void Start()
	{
		startowaPozycja = new Vector3(500f, 80f, 555f); // Ustawienie pocz¹tkowej pozycji kamery
	}

	void Update()
	{
		Vector3 pozycja = transform.position;

		// Poruszanie kamer¹
		if (Input.GetKey("w"))
			pozycja.z += predkoscPrzesuwania * Time.deltaTime;
		if (Input.GetKey("s"))
			pozycja.z -= predkoscPrzesuwania * Time.deltaTime;
		if (Input.GetKey("d"))
			pozycja.x += predkoscPrzesuwania * Time.deltaTime;
		if (Input.GetKey("a"))
			pozycja.x -= predkoscPrzesuwania * Time.deltaTime;

		// Zoom kamery
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		pozycja.y -= scroll * predkoscScrollowania * Time.deltaTime;

		// Ograniczenia poruszania
		pozycja.y = Mathf.Clamp(pozycja.y, minWysokosc, maxWysokosc);
		pozycja.x = Mathf.Clamp(pozycja.x, minX, maxX);
		pozycja.z = Mathf.Clamp(pozycja.z, minZ, maxZ);

		transform.position = pozycja;
	}
}
