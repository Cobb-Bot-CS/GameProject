using Codice.CM.Common;
using UnityEngine;

public class BulletBill : MonoBehaviour
{
   [SerializeField] float speed;

   // Update is called once per frame
   void Update()
   {
      if (transform.position.x > 80)
      {
         transform.position = new Vector3(Random.Range(-20, -40), Random.Range(0, 8), 0);
      }
      else
      {
         transform.Translate(speed * Time.deltaTime * Vector3.right);
      }
    }
}
