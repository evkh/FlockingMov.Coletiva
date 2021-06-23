using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    //criando a var para colocar o manager
    public FlockManager myManager;
    //setando speed
    public float speed;
    //var para verificar se o peixe esta virando
    bool turning = false;

    private void Start()
    {
        //seta a speed com uma velocidade aleatoria da minSpeed e Maxspeed do manager
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
    }

    void Update()
    {
        //limita o espaço de nado dos peixes
        Bounds b = new Bounds(myManager.transform.position, myManager.swinLimits * 2);

        //raycast usado nos peixes
        RaycastHit hit = new RaycastHit();

        //direção para os peixes
        Vector3 direction = myManager.transform.position - transform.position;

        if (!b.Contains(transform.position))
        {
            turning = true;
            direction = myManager.transform.position - transform.position;
        }
        else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit))//usa o raycast para não acontecer a colisão com o pilar
        {
            turning = true;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        }
        else
            turning = false;

        if (turning)
        {
            //faz a rotação
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);

        }
        else
        {
            //setando a speed random de cada peixe
            if (Random.Range(0, 100) < 10) speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
            if (Random.Range(0, 100) < 20) ApplyRules();
        }
        transform.Translate(0, 0, Time.deltaTime * speed);

    }

    void ApplyRules()
    {
        //pegando a info allfish do outro script
        GameObject[] gos;
        gos = myManager.allFish;

        //calculo de ponto medio entre os peixes
        Vector3 vcentre = Vector3.zero;
        //evitando colisão entre os peixes
        Vector3 vavoid = Vector3.zero;

        float gSpeed = 0.01f;

        //para verificar a distancia
        float nDistance;

        //tamanho do grupo
        int groupSize = 0;
        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= myManager.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;
                    //verifica a distancia e evita colisões
                    if (nDistance < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }
                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }
        if (groupSize > 0) //seta varios parametros se o numero do grupo for maior que 0
        {
            vcentre = vcentre / groupSize + (myManager.goalPos - this.transform.position);
            speed = gSpeed / groupSize;
            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero) //deixa a rotação natural
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);
            }
        }
    }
}

