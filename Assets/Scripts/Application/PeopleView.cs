using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class PeopleView : MonoBehaviour
{    
    public GameObject characterPrefab;
    public GameObject environmentDesk;
    public float characterYPosition = 0.0f;
    
    public List<GameObject> people;

    public const int FIRST_YEAR = 2017;
    public const int DEFAULT_YEAR = 2017;
    public const int DEFAULT_SEMESTER = 1;

    public const int DEFAULT_COURSE_ID = 1;

    List<SemesterUFF> semestersList = null;

    Boundaries boundaries;

    int loadedYear = DEFAULT_YEAR;
    int loadedSemester = DEFAULT_SEMESTER;
    int loadedCourseId = DEFAULT_COURSE_ID;
    
    void Start()
    {
        Collider selfCollider = gameObject.GetComponent<Collider>();

        Collider deskCollider = environmentDesk.GetComponent<Collider>();

        float minX = deskCollider.bounds.min.x;
        float maxX = deskCollider.bounds.max.x;
        float minZ = deskCollider.bounds.min.z;
        float maxZ = deskCollider.bounds.max.z;

        boundaries = new Boundaries(minX, maxX, minZ, maxZ);

        // spawn a cube on each corner of the boundaries
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(minX, 0.77f, minZ);
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(minX, 0.77f, maxZ);
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(maxX, 0.77f, minZ);
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(maxX, 0.77f, maxZ);
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        ParseDataset("Assets/Resources/Data/data-by-semesters.json");
        LoadPoints(DEFAULT_YEAR, DEFAULT_SEMESTER, DEFAULT_COURSE_ID);
    }

    void Update()
    {
        gameObject.transform.rotation = Quaternion.identity;
        
        if (gameObject.transform.localScale.x > 1.0f)
        {
            gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            gameObject.transform.position = new Vector3(0.0f, 0.75f, 0.0f);
        } else if (gameObject.transform.localScale.x < 1.0f)
        {
            // get z from the scale of the object
            float z = gameObject.transform.localScale.z;

            // if z is 1, then object's z position is 0
            // if z is 0, then object's z position is 0.2
            // otherwise, object's z position is 0.2 * (1 - z)
            float zPosition = 0.5f * (1 - z);

            gameObject.transform.position = new Vector3(0.0f, 0.75f, zPosition);
        }
    }

    public void ParseDataset(string datasetPath)
    {
        string text = File.ReadAllText(datasetPath);

        SemestersUFF semesters = JsonUtility.FromJson<SemestersUFF>(text);

        semestersList = semesters.Semester;
    }

    public void LoadPoints(int year, int semester, int courseId, bool grouped = false)
    {
        if (people != null) DumpPoints();
        
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
            if (course.id == courseId) {
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
                
                if (grouped) {
                    float womenGroupRadius = courseWomen / 3.5f;
                    float menGroupRadius = courseMen / 3.5f;

                    Vector2 womenGroupCenterXZ = new Vector2(0, -womenGroupRadius - 1);
                    Vector2 menGroupCenterXZ = new Vector2(0, menGroupRadius + 1);

                    
                    for (int i = 0; i < courseWomen; i++) {
                        GameObject character = Instantiate(characterPrefab);
                        character.transform.parent = gameObject.transform;
                        character.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = new Color(0.4f, 0.1f, 1.0f, 1.0f);
                        
                        float random = Random.Range(0, womenGroupRadius);
                        float theta = Random.Range(0, 2 * Mathf.PI);

                        float x = Mathf.Sqrt(random * womenGroupRadius) * Mathf.Cos(theta);
                        float z = Mathf.Sqrt(random * womenGroupRadius) * Mathf.Sin(theta);

                        float xRelativeToCenter = x + womenGroupCenterXZ.x;
                        float zRelativeToCenter = z + womenGroupCenterXZ.y;

                        character.transform.position = new Vector3(xRelativeToCenter, characterYPosition, zRelativeToCenter);
                    }

                    for (int i = 0; i < courseMen; i++) {
                        GameObject character = Instantiate(characterPrefab);
                        character.transform.parent = gameObject.transform;
                        character.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.6f, 0.1f, 1.0f);
                        
                        float random = Random.Range(0, menGroupRadius);
                        float theta = Random.Range(0, 2 * Mathf.PI);

                        float x = Mathf.Sqrt(random * menGroupRadius) * Mathf.Cos(theta);
                        float z = Mathf.Sqrt(random * menGroupRadius) * Mathf.Sin(theta);

                        float xRelativeToCenter = x + menGroupCenterXZ.x;
                        float zRelativeToCenter = z + menGroupCenterXZ.y;

                        character.transform.position = new Vector3(xRelativeToCenter, characterYPosition, zRelativeToCenter);
                    }
                } else {
                    for (int i = 0; i < courseWomen; i++) {
                        GameObject character = Instantiate(characterPrefab);
                        character.transform.SetParent(gameObject.transform, true);
                        character.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = new Color(0.4f, 0.1f, 1.0f, 1.0f);
                        character.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                        
                        Vector2 horizontalPosition = boundaries.GetRandomPoint();
                        character.transform.localPosition = new Vector3(horizontalPosition.x, 0.02f, horizontalPosition.y);

                        people.Add(character);
                    }

                    for (int i = 0; i < courseMen; i++) {
                        GameObject character = Instantiate(characterPrefab);
                        character.transform.parent = gameObject.transform;
                        character.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.6f, 0.05f, 1.0f);
                        character.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                        
                        Vector2 horizontalPosition = boundaries.GetRandomPoint();
                        character.transform.localPosition = new Vector3(horizontalPosition.x, 0.02f, horizontalPosition.y);

                        people.Add(character);
                    }
                }
            }
        }
    }

    public void DumpPoints()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnSliderUpdated(SliderEventData eventData)
    {
        var arbitraryValue = float.Parse($"{eventData.NewValue:F2}");
        var value = arbitraryValue * 10;

        var isEven = (value % 2) == 0;

        var year = FIRST_YEAR + (int)(value / 2);
        var semester = isEven ? 1 : 2;

        if (year == loadedYear && semester == loadedSemester) {
            return;
        }

        loadedYear = year;
        loadedSemester = semester;

        DumpPoints();
        LoadPoints(year, semester, loadedCourseId);
    }

    public void OnCourseSliderUpdated(SliderEventData eventData)
    {
        var arbitraryValue = float.Parse($"{eventData.NewValue}");
        var courseId = (int)(arbitraryValue * 80) + 1;

        if (courseId == loadedCourseId) {
            return;
        }

        loadedCourseId = courseId;

        DumpPoints();
        LoadPoints(loadedYear, loadedSemester, courseId);
    }
}
