using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// define semester data type
[System.Serializable]
public class Semester
{
    public int Ano;
    public int Periodo;
    public List<Course> Cursos;
}

// define semesters data type
[System.Serializable]
public class Semesters
{
    public List<Semester> Semester;
}