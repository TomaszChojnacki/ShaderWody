using UnityEngine;

public class BoatFloat : MonoBehaviour
{
   
    public float floatHeight = 5f; // Jak wysoko ma sie unosic nad fala
    public float tiltAmount = 5f;   // Maksymalne przechylenie statku
    public float bobSpeed = 1f;      // PredkoSC bujania w pionie
    public float rotateSpeed = 1f;   // PredkoSC przechylania

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        // pion fal sin
        float waveY = Mathf.Sin(Time.time * bobSpeed + pos.x + pos.z) * floatHeight;
        pos.y = startPos.y + waveY;
        transform.position = pos;

        // Przechylenie przod tyl i na boki
        float tiltX = Mathf.Cos(Time.time * rotateSpeed + pos.z) * tiltAmount;
        float tiltZ = Mathf.Sin(Time.time * rotateSpeed + pos.x) * tiltAmount;
        transform.rotation = Quaternion.Euler(tiltX, transform.rotation.eulerAngles.y, tiltZ);
    }
}
