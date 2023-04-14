using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// define semester data type
[System.Serializable]
public class SemesterIndividual
{
    public int year;
    public int period;
    public List<CourseIndividual> courses;
}

// define semesters data type
[System.Serializable]
public class SemestersIndividual
{
    public List<SemesterIndividual> Semester;
}
