using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [RequireComponent(typeof(CharacterController))] // контекст размещения метода RequireComponent
    [RequireComponent(typeof(Animator))]
public class RelativeMovement : MonoBehaviour
{
    [SerializeField] private Transform target; // объект, относительно которого происходит перемещение

    public float rotSpeed = 15.0f;
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f; // минимальная скорость падения

    private CharacterController _charController;
    private float _vertSpeed; // вертикальная скорость
    private ControllerColliderHit _contact; // столкновения
    private Animator _animator;

    private void Start()
    {
        _charController = GetComponent<CharacterController>();
        _vertSpeed = minFall;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;

        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        if (horInput != 0 || vertInput != 0)
        {
            movement.x = horInput * moveSpeed;
            movement.z = vertInput * moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed); // ограничиваем движение

            Quaternion tmp = target.rotation; // сохраняем начальную ориентацию, чтобы вернуться к ней после завершения работы с целевым объектом
            target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
            movement = target.TransformDirection(movement); // преобразуем направление движения из локальных в глобальные координаты
            target.rotation = tmp;

            Quaternion direction = Quaternion.LookRotation(movement); // вычисляет кватернион, смотрящий в этом направлении
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime); // плавный поворот
        }

        _animator.SetFloat("Speed", movement.sqrMagnitude);

        bool hitGround = false;
        RaycastHit hit;
        // проверяем, падает ли персонаж
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (_charController.height + _charController.radius) / 1.9f; // расстояние, с которым производится сравнение - слегка выходит за нижнюю часть капсулы
            hitGround = hit.distance <= check;
        }
        
        if (hitGround) // соприкасается ли с поверхностью 
        {
            if (Input.GetButtonDown("Jump"))
            {
                _vertSpeed = jumpSpeed;
            }
            else
            {
                _vertSpeed = minFall;
                _animator.SetBool("Jumping", false);
            }
        }
        else
        {
            _vertSpeed += gravity * 5 * Time.deltaTime; // если персонаж не стоит на поверхности, то применяем гравитацию пока не будет достигнута предельная скорость
            if (_vertSpeed < terminalVelocity)
            {
                _vertSpeed = terminalVelocity;
            }
            if (_contact != null)
            {
                _animator.SetBool("Jumping", true);
            }

            if (_charController.isGrounded)
            {
                if (Vector3.Dot(movement, _contact.normal) < 0)
                {
                    movement = _contact.normal * moveSpeed;
                }
                else
                {
                    movement += _contact.normal * moveSpeed;
                }
            }
        }

        movement.y = _vertSpeed;

        movement *= Time.deltaTime;
        _charController.Move(movement);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
    }
}
