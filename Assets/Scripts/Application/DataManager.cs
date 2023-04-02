using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct SexClassification
{
    public int totalWomen;
    public int totalMen;
    public float difference;

    public SexClassification(int totalWomen, int totalMen, float difference)
    {
        this.totalWomen = totalWomen;
        this.totalMen = totalMen;
        this.difference = difference;
    }
}

public class DataManager : Singleton<DataManager>
{
    const string DATASET_PATH = "Assets/Resources/Data/data-by-semesters.json";

    List<SemesterUFF> semestersList = null;

    void Start()
    {
        ParseDataset(DATASET_PATH);
    }

    void ParseDataset(string datasetPath)
    {
        string text = File.ReadAllText(datasetPath);

        SemestersUFF semesters = JsonUtility.FromJson<SemestersUFF>(text);

        semestersList = semesters.Semester;
    }

    public List<SemesterUFF> GetSemestersList()
    {
        return semestersList;
    }

    public SemesterUFF GetSemester(int year, int semester)
    {
        foreach (SemesterUFF semesterItem in semestersList)
        {
            if (semesterItem.year == year && semesterItem.period == semester)
            {
                return semesterItem;
            }
        }

        return null;
    }

    public CourseUFF GetCourse(int year, int semester, int courseId)
    {
        SemesterUFF requestedSemester = GetSemester(year, semester);

        if (requestedSemester == null)
        {
            return null;
        }

        foreach (CourseUFF courseItem in requestedSemester.courses)
        {
            if (courseItem.id == courseId)
            {
                return courseItem;
            }
        }

        return null;
    }

    public SexClassification GetSexClassification(CourseUFF course)
    {
        int totalWomen = 0;
        int totalMen = 0;

        float difference = 0.0f;

        foreach (ClassificationUFF sex in course.sex)
        {
            if (sex.name == "F")
            {
                totalWomen = sex.total;
            }

            if (sex.name == "M")
            {
                totalMen = sex.total;
            }
        }

        if (totalWomen == 0 || totalMen == 0)
        {
            difference = 1;
        }
        else
        {
            if (totalWomen > totalMen)
            {
                difference = Mathf.Abs(Mathf.RoundToInt(totalWomen / totalMen));
            }
            else
            {
                difference = Mathf.Abs(Mathf.RoundToInt(totalMen / totalWomen));
            }
        }

        return new SexClassification(totalWomen, totalMen, difference);
    }
}