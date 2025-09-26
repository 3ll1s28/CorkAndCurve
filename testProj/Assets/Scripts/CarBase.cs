using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBase : MonoBehaviour
{
    int startPosition, currentPosition;

    public virtual void CalculatePosition(CarBase[] cars)
    {
        for(int i=0; i<cars.Length; i++)
        {
            for(int j=0; j<cars.Length; j++)
            {
                if (Vector3.Dot(cars[i].transform.position, cars[i].transform.forward) > Vector3.Dot(cars[j].transform.position, cars[j].transform.forward)) //calculate position of all cars by sorting them by forward displacement
                {
                    cars[j].currentPosition -= 1;
                }
            }
        }
    }
}
