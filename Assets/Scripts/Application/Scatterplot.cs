using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

[AddComponentMenu("Scripts/MRTK/Examples/Scatterplot")]
public class Scatterplot : MonoBehaviour
{
    public GameObject pointPrefab;

    public const int FIRST_YEAR = 2017;
    public const int DEFAULT_YEAR = 2017;
    public const int DEFAULT_SEMESTER = 1;

    List<SemesterUFF> semestersList = null;

    List<ScatterplotDataPoint> scatterplotPoints = null;

    int loadedYear = DEFAULT_YEAR;
    int loadedSemester = DEFAULT_SEMESTER;

    void Start()
    {
        ParseDataset("Assets/Resources/Data/data-by-semesters.json");
        LoadPoints(DEFAULT_YEAR, DEFAULT_SEMESTER);

        // disable Object Manipultor
        // gameObject.GetComponent<ObjectManipulator>().enabled = false;


        // disable MeshRenderer of all points
        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            // point.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void Update()
    {
    }

    public void ParseDataset(string datasetPath)
    {
        string text = File.ReadAllText(datasetPath);

        SemestersUFF semesters = JsonUtility.FromJson<SemestersUFF>(text);

        semestersList = semesters.Semester;
    }

    public void LoadPoints(int year, int semester)
    {
        if (scatterplotPoints != null) DumpPoints();

        scatterplotPoints = new List<ScatterplotDataPoint>();

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

        int maxWomen = 0;
        int maxMen = 0;
        int maxDiff = 0;

        foreach (CourseUFF course in requestedSemester.courses)
        {
            int courseWomen = 0;
            int courseMen = 0;
            int courseDiff = 0;

            foreach (ClassificationUFF sex in course.sex) {
                if (sex.name == "F") {
                    courseWomen = sex.total;
                }

                if (sex.name == "M") {
                    courseMen = sex.total;
                }
            }

            if (courseWomen == 0 || courseMen == 0) {
                courseDiff = 1;
            } else {
                if (courseWomen > courseMen) {
                    courseDiff = Mathf.Abs(Mathf.RoundToInt(courseWomen / courseMen));
                } else {
                    courseDiff = Mathf.Abs(Mathf.RoundToInt(courseMen / courseWomen));
                }
            }

            if (courseWomen > maxWomen)
            {
                maxWomen = courseWomen;
            }
            if (courseMen > maxMen)
            {
                maxMen = courseMen;
            }
            if (courseDiff > maxDiff)
            {
                maxDiff = courseDiff;
            }
        }

        foreach (CourseUFF course in requestedSemester.courses)
        {
            float normalizationFactor = 0.5f;

            int courseWomen = 0;
            int courseMen = 0;
            int courseDiff = 0;
            
            foreach (ClassificationUFF sex in course.sex) {
                if (sex.name == "F") {
                    courseWomen = sex.total;
                }

                if (sex.name == "M") {
                    courseMen = sex.total;
                }
            }

            if (courseWomen == 0 || courseMen == 0) {
                courseDiff = 1;
            } else {
                if (courseWomen > courseMen) {
                    courseDiff = Mathf.Abs(Mathf.RoundToInt(courseWomen / courseMen));
                } else {
                    courseDiff = Mathf.Abs(Mathf.RoundToInt(courseMen / courseWomen));
                }
            }

            float x =
                normalizationFactor *
                System.Convert.ToSingle(courseWomen) /
                maxWomen;
            float y =
                normalizationFactor *
                System.Convert.ToSingle(courseDiff) /
                maxDiff;
            float z =
                normalizationFactor *
                System.Convert.ToSingle(courseMen) /
                maxMen;

            ScatterplotDataPoint newDataPoint =
                Instantiate(pointPrefab,
                new Vector3(x, y, z),
                Quaternion.identity).GetComponent<ScatterplotDataPoint>();

            newDataPoint.transform.position += this.transform.position;
            newDataPoint.transform.parent = this.transform;
            newDataPoint.gameObject.name = course.name;

            newDataPoint.dataClass = course.id.ToString();

            newDataPoint.textLabel.text =
                course.name + " (" + newDataPoint.dataClass + ")";

            Color newColor = new Color();

            newColor.a = 1f;

            newColor.r = x / normalizationFactor;
            newColor.g = y / normalizationFactor;
            newColor.b = z / normalizationFactor;

            newDataPoint.GetComponent<Renderer>().material.color = newColor;
            newDataPoint.pointColor = newColor;

            scatterplotPoints.Add (newDataPoint);
        }

        // Should also adjust size of scatterplot collider box here based on points positions
    }

    public void DumpPoints()
    {
        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            Destroy(point.gameObject);
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
        LoadPoints(year, semester);
    }
}
