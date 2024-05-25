using UnityEngine;
namespace ClaraMundi
{
  public class Rotation : MonoBehaviour
  {
    public float speed;
    private float yRotataion;

    // Update is called once per frame
    void Update()
    {
      yRotataion += speed * Time.deltaTime;
      gameObject.transform.rotation = Quaternion.Euler(0f, yRotataion, 0f);
    }
  }
}