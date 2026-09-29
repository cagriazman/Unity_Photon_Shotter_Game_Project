using StarterAssets;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    private StarterAssetsInputs _input;
    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.05f;
    public float smoothPosition = 6f;

    public float rotationAmount = 3f;
    public float maxRotationAmount = 5f;
    public float smoothRotation = 8f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        _input = GetComponentInParent<StarterAssetsInputs>();
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        CalculateSway();
    }

    private void CalculateSway()
    {
        float moveX = _input.move.x;
        float moveY = _input.move.y;

        float targetRotX = Mathf.Clamp(moveY * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float targetRotZ = Mathf.Clamp(moveX * rotationAmount, -maxRotationAmount, maxRotationAmount);
        Quaternion targetRotation = Quaternion.Euler(new Vector3(targetRotX, 0, targetRotZ));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation * targetRotation, Time.deltaTime * smoothRotation);
    }
}