using UnityEngine;
using TMPro;

public class TextCamFollow : MonoBehaviour
{
    public float Distance = 1;
    private TextMeshPro _textMeshPro;
    private Transform _transform;
    private Camera _camera;

    public static void Init(TextMeshPro textMeshPro, Transform transform, Camera camera)
    {
        TextCamFollow instance = textMeshPro.gameObject.AddComponent<TextCamFollow>();

        instance._textMeshPro = textMeshPro;
        instance._transform = transform;
        instance._camera = camera;
    }

    void Update()
    {
        if (_textMeshPro == null || _transform == null || _camera == null) return;

        var direction = _transform.position - _camera.transform.position;
        direction.Normalize();

        _textMeshPro.transform.localPosition =  (direction * Distance);
        _textMeshPro.transform.rotation = Quaternion.LookRotation(direction);
    }
}