using System.Collections;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target; // ссылка на объект, который облетаем

    public float rotSpeed = 1.5f;
    private float _rotY;
    private Vector3 _offset;

    void Start()
    {
        _rotY = transform.eulerAngles.y;
        _offset = target.position - transform.position; // начальное смещение между камерой и целью
    }

    void LateUpdate()
    {
        float horInput = Input.GetAxis("Horizontal");
        if (horInput != 0)
        { // медленный поворот с помощью клавиш стрелками
            _rotY += horInput * rotSpeed;
        }
        else
        { // быстрый поворот мышью
            _rotY += Input.GetAxis("Mouse X") * rotSpeed * 3;
        }

        Quaternion rotation = Quaternion.Euler(0, _rotY, 0);
        transform.position = target.position - (rotation * _offset); // поддерживаем начальное смещение, сдвигаемое в соответствии с поворотом камеры
        transform.LookAt(target); // смотрим на цель
    }
}
