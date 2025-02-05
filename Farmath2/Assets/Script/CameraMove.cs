using UnityEngine;
using UnityEngine.UIElements;

public class CameraMove : MonoBehaviour
{
    public float speed;
    public float[] border;//minx, maxx, miny, maxy
    public GameObject Camera;
    void Update()
    {
        float X = Input.GetAxisRaw("Horizontal") * speed;
        float Y = Input.GetAxisRaw("Vertical") * speed;
        Camera.transform.Translate(Vector3.right * X * Time.deltaTime);
        Camera.transform.Translate(Vector3.up * Y * Time.deltaTime);
        
        Vector3 Pos = Camera.transform.position;
        float NorX = Mathf.Clamp(Pos.x, border[0], border[1]);
        float NorY = Mathf.Clamp(Pos.y, border[2], border[3]);
        Pos = new Vector3(NorX,NorY,Pos.z);
        Camera.transform.position = Pos;
    }
}
