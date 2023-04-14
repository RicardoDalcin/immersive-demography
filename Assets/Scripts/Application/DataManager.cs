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

public struct EthnicityClassification
{
    public int totalBlack;
    public int totalBrown;
    public int totalYellow;
    public int totalWhite;
    public int totalUndeclared;

    public EthnicityClassification(
        int totalBlack,
        int totalBrown,
        int totalYellow,
        int totalWhite,
        int totalUndeclared
    )
    {
        this.totalBlack = totalBlack;
        this.totalBrown = totalBrown;
        this.totalYellow = totalYellow;
        this.totalWhite = totalWhite;
        this.totalUndeclared = totalUndeclared;
    }
}

public struct AdmissionClassification
{
    public int totalSisu;
    public int totalCourseChange;
    public int totalExternalTransference;
    public int totalPublicReentry;

    public AdmissionClassification(
        int totalSisu,
        int totalCourseChange,
        int totalExternalTransference,
        int totalPublicReentry
    )
    {
        this.totalSisu = totalSisu;
        this.totalCourseChange = totalCourseChange;
        this.totalExternalTransference = totalExternalTransference;
        this.totalPublicReentry = totalPublicReentry;
    }
}

public struct NationalityClassification
{
    public int totalBrazilian;
    public int totalForeigner;

    public NationalityClassification(int totalBrazilian, int totalForeigner)
    {
        this.totalBrazilian = totalBrazilian;
        this.totalForeigner = totalForeigner;
    }
}

public class DataManager : Singleton<DataManager>
{
    const string DATASET_PATH = "Assets/Resources/Data/data-by-semesters.json";
    const string INDIVIDUAL_DATASET_PATH = "Assets/Resources/Data/individual.json";

    List<SemesterUFF> semestersList = null;
    List<SemesterIndividual> semestersIndividualList = null;

    void Start()
    {
        ParseDataset(DATASET_PATH);
        ParseIndividualDataset(INDIVIDUAL_DATASET_PATH);
    }

    void ParseDataset(string datasetPath)
    {
        string text = File.ReadAllText(datasetPath);

        SemestersUFF semesters = JsonUtility.FromJson<SemestersUFF>(text);

        semestersList = semesters.Semester;
    }

    void ParseIndividualDataset(string datasetPath)
    {
        string text = File.ReadAllText(datasetPath);

        SemestersIndividual semesters = JsonUtility.FromJson<SemestersIndividual>(text);

        semestersIndividualList = semesters.Semester;
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

    public SemesterIndividual GetSemesterIndividual(int year, int semester)
    {
        foreach (SemesterIndividual semesterItem in semestersIndividualList)
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

    public CourseIndividual GetCourseIndividual(int year, int semester, int courseId)
    {
        SemesterIndividual requestedSemester = GetSemesterIndividual(year, semester);

        if (requestedSemester == null)
        {
            return null;
        }

        foreach (CourseIndividual courseItem in requestedSemester.courses)
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

    public EthnicityClassification GetEthnicityClassification(CourseUFF course)
    {
        int totalBlack = 0;
        int totalBrown = 0;
        int totalYellow = 0;
        int totalWhite = 0;
        int totalUndeclared = 0;

        foreach (ClassificationUFF ethnicity in course.ethnicity)
        {
            switch (ethnicity.name)
            {
                case "NEGRA":
                    totalBlack = ethnicity.total;
                    break;
                case "PARDA":
                    totalBrown = ethnicity.total;
                    break;
                case "AMARELA":
                    totalYellow = ethnicity.total;
                    break;
                case "BRANCA":
                    totalWhite = ethnicity.total;
                    break;
                case "NAO DECLARADO":
                    totalUndeclared = ethnicity.total;
                    break;
            }
        }

        return new EthnicityClassification(
            totalBlack,
            totalBrown,
            totalYellow,
            totalWhite,
            totalUndeclared
        );
    }

    public AdmissionClassification GetAdmissionClassification(CourseUFF course)
    {
        int totalSisu = 0;
        int totalCourseChange = 0;
        int totalExternalTransference = 0;
        int totalPublicReentry = 0;

        foreach (ClassificationUFF admission in course.admission)
        {
            switch (admission.name)
            {
                case "SISU 1ª EDICAO":
                case "SISU 2ª EDICAO":
                    totalSisu += admission.total;
                    break;
                case "MUDANCA DE CURSO":
                    totalCourseChange = admission.total;
                    break;
                case "TRANSFERENCIA INTERINSTITUCIONAL":
                    totalExternalTransference = admission.total;
                    break;
                case "REINGRESSO POR CONCURSO PUBLICO":
                    totalPublicReentry = admission.total;
                    break;
            }
        }

        return new AdmissionClassification(
            totalSisu,
            totalCourseChange,
            totalExternalTransference,
            totalPublicReentry
        );
    }

    public NationalityClassification GetNationalityClassification(CourseUFF course)
    {
        int totalBrazilian = 0;
        int totalForeigner = 0;

        foreach (ClassificationUFF nationality in course.nationality)
        {
            if (nationality.name == "BRASILEIRA")
            {
                totalBrazilian = nationality.total;
            }
            else
            {
                totalForeigner += nationality.total;
            }
        }

        return new NationalityClassification(totalBrazilian, totalForeigner);
    }
}
