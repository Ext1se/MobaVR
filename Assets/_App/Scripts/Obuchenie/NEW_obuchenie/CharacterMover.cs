using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterMover : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    public AudioClip footstepsSound; // Zvuk shagov
    private AudioSource footstepsSource; //Komponent AudioSource dlya zvuka shagov

    public NavMeshAgent Agent => agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        footstepsSource = gameObject.AddComponent<AudioSource>();
        footstepsSource.clip = footstepsSound;
        footstepsSource.loop = true; // Зацикливаем звук шагов
    }

    
    private void Update()
    {
        if (agent.velocity.magnitude > 0f) // Если персонаж двигается
        {
            if (!footstepsSource.isPlaying)
            {
                footstepsSource.Play(); // Воспроизводим звук шагов
            }
            animator.SetBool("isWalking", true); // Активируем анимацию ходьбы
        }
        else
        {
            if (footstepsSource.isPlaying)
            {
                footstepsSource.Stop(); // Останавливаем звук шагов
            }
            animator.SetBool("isWalking", false); // Останавливаем анимацию ходьбы
        }
    }
    
    
    // Метод для передвижения к указанной точке
    public void MoveToPoint(Vector3 destination)
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (agent != null)
        {
            agent.SetDestination(destination);
            
        }
    }

    // Метод проверки, достиг ли персонаж пункта назначения
    public bool HasReachedDestination()
    {
        if (agent == null || !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}