using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private int randomNumberPosition = 0;
    private GameManager levelGenerationManager;
    private Transform levelBin;

    public GameObject[] levels;
    public GameObject levelEnd;
    // Start is called before the first frame update
    void Start()
    {
        levelGenerationManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        levelBin = GameObject.Find("LevelBin").GetComponent<Transform>();

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        // randomNumberPosition = Random.Range(0,transform.GetChild(0).childCount - 1);
        randomNumberPosition = 1;
        int randomLevelNumber = Random.Range(0, levels.Length);
        if(randomLevelNumber == levels.Length)
        {
            randomLevelNumber = levels.Length - 1;
        }

        if(levelGenerationManager.GetlevelGenCurrentNumber() > 0)
        {
            levelGenerationManager.SubfromGenLevelNumber();
            
            int locationNumber = levelGenerationManager.GetpreviousLocationNumber();
            locationNumber = 1;
            // if(locationNumber == 0 || locationNumber == 1)
            // {
            //     randomNumberPosition = 2;

            //     levelGenerationManager.AssignpreviousLocationNumber(randomNumberPosition);
            // }
            // else
            // {
            //     levelGenerationManager.AssignpreviousLocationNumber(randomNumberPosition);
            // }

            if(levelGenerationManager.GetlevelGenCurrentNumber() == 0)
            {
                Instantiate(levelEnd, transform.GetChild(0).transform.GetChild(randomNumberPosition).transform.position, Quaternion.identity, levelBin); 
            }
            else
            {
                Instantiate(levels[randomLevelNumber], transform.GetChild(0).transform.GetChild(randomNumberPosition).transform.position, Quaternion.identity, levelBin);       
            }
        }
    }
}
