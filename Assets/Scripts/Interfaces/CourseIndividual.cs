using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CoursePerson
{
    public string sex;
    public string ethnicity;
    public string admission;
    public string nationality;
}

[System.Serializable]
public class CourseIndividual
{
    public int id;
    public string name;
    public List<CoursePerson> people;
}

[System.Serializable]
public class SimpleCourseIndividual
{
    public int id;
    public string name;
}

[System.Serializable]
public class CoursesIndividual
{
    public List<SimpleCourseIndividual> Course;
}
