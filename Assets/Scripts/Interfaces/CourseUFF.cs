using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CourseUFF
{
    public int id;
    public string name;
    public List<ClassificationUFF> sex;
    public List<ClassificationUFF> admission;
    public List<ClassificationUFF> ethnicity;
    public List<ClassificationUFF> nationality;
}

[System.Serializable]
public class SimpleCourseUFF
{
    public int id;
    public string name;
}

[System.Serializable]
public class CoursesUFF
{
    public List<SimpleCourseUFF> Course;
}