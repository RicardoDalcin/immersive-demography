using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class FirstPersonUFF : MonoBehaviour
{
    public GameObject characterPrefab;
    public GameObject mainCamera;
    
    public float characterYPosition = 0.0f;
    public List<GameObject> people;

    public const int FIRST_YEAR = 2010;
    public const int DEFAULT_YEAR = 2018;
    public const int DEFAULT_SEMESTER = 1;


    List<SemesterUFF> semestersList = null;

    int loadedYear = DEFAULT_YEAR;
    int loadedSemester = DEFAULT_SEMESTER;
    
    void Start()
    {
        ParseDataset("Assets/Resources/Data/uff.json");
        LoadPoints(DEFAULT_YEAR, DEFAULT_SEMESTER);
    }

    void Update()
    {
        float objectScale = gameObject.transform.localScale.x;
        
        Vector3 cameraForward = mainCamera.transform.forward;

        float movement = 1.0f * (objectScale - 1.0f);

        gameObject.transform.position = new Vector3(
            gameObject.transform.position.x - movement * cameraForward.x,
            gameObject.transform.position.y,
            gameObject.transform.position.z - movement * cameraForward.z
        );

        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void ParseDataset(string datasetPath)
    {
        string text = File.ReadAllText(datasetPath);

        SemestersUFF semesters = JsonUtility.FromJson<SemestersUFF>(text);

        semestersList = semesters.Semester;
    }

    public void LoadPoints(int year, int semester)
    {
        SemesterUFF requestedSemester = null;

        foreach (SemesterUFF semesterItem in semestersList)
        {
            if (semesterItem.year == year && semesterItem.period == semester)
            {
                requestedSemester = semesterItem;
                break;
            }
        }

        if (requestedSemester == null)
        {
            Debug.Log("Semester not found");
            return;
        }

        foreach (CourseUFF course in requestedSemester.courses)
        {
            if (course.name == "CIENCIA DA COMPUTACAO") {
                int courseWomen = 0;
                int courseMen = 0;

                foreach (ClassificationUFF sex in course.sex) {
                    if (sex.name == "F") {
                        courseWomen = sex.total;
                    }

                    if (sex.name == "M") {
                        courseMen = sex.total;
                    }
                }
                
                for (int i = 0; i < courseWomen; i++) {
                    
                    GameObject character = Instantiate(characterPrefab);
                    character.transform.parent = gameObject.transform;
                    character.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = new Color(0.4f, 0.1f, 1.0f, 1.0f);
                    character.transform.position = new Vector3(Random.Range(-10, 10), characterYPosition, Random.Range(-10, 10));

                    people.Add(character);
                }

                for (int i = 0; i < courseMen; i++) {
                    GameObject character = Instantiate(characterPrefab);
                    character.transform.parent = gameObject.transform;
                    character.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.6f, 0.1f, 1.0f);
                    character.transform.position = new Vector3(Random.Range(-10, 10), characterYPosition, Random.Range(-10, 10));

                    people.Add(character);
                }
            }
        }
    }

    public void DumpPoints()
    {
        foreach (GameObject person in people)
        {
            Destroy (person);
        }
    }

    public void OnSliderUpdated(SliderEventData eventData)
    {
        var arbitraryValue = float.Parse($"{eventData.NewValue:F2}");
        var value = arbitraryValue * 25;

        var isEven = (value % 2) == 0;

        var year = FIRST_YEAR + (int)(value / 2);
        var semester = isEven ? 1 : 2;

        if (year == loadedYear && semester == loadedSemester) {
            return;
        }

        loadedYear = year;
        loadedSemester = semester;

        DumpPoints();
        LoadPoints(year, semester);
    }
}
