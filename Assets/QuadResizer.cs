using UnityEngine;

public class QuadResizer : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public bool alignToSides = true;
    void Update()
    {
        // Calculate the new position, which is the midpoint between the two points
        Vector3 newPosition = (point1.position + point2.position) / 2;

        // Calculate the distance between the two points
        float distance = Vector2.Distance(new Vector2(point1.position.x, point1.position.z), new Vector2(point2.position.x, point2.position.z));


        // Calculate the angle between the two points in the XZ plane
        Vector3 diff = point2.position - point1.position;
        float angle = Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;

        float scale;
        if (alignToSides)
        {
            // Align to sides
            scale = distance;
        }
        else
        {
            // Align to corners
            scale = distance / Mathf.Sqrt(2);
            angle += 45;
        }

        // Update the quad's position, scale, and rotation
        transform.position = newPosition;
        transform.localScale = new Vector3(scale, scale, scale);  // Scale proportionately in all dimensions
        transform.rotation = Quaternion.Euler(90, -angle, 0);
    }
}