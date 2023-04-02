using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// define semester data type
[System.Serializable]
public class SemesterUFF
{
    public int year;
    public int period;
    public List<CourseUFF> courses;
}

// define semesters data type
[System.Serializable]
public class SemestersUFF
{
    public List<SemesterUFF> Semester;
}