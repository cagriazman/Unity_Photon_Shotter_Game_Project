using UnityEngine;

public class Sway : MonoBehaviour
{
    [Header("Settings")]
    public float swayClamp = 0.03f;
    public float swaySmooth = 2f;
    public float returnSmooth = 5;

    [HideInInspector]
    public bool isAiming = false;

    private Vector3 origin;

    private void Start()
    {
        origin = transform.localPosition;
    }

    private void Update()
    {
        Vector3 targetPosition = origin; 

        if (!isAiming)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            input.x = Mathf.Clamp(input.x, -swayClamp, swayClamp);
            input.y = Mathf.Clamp(input.y, -swayClamp, swayClamp);

            targetPosition = origin + new Vector3(-input.x, -input.y, 0);
        }

        float smoothVal = isAiming ? returnSmooth : swaySmooth;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, smoothVal * Time.deltaTime);
    }
}