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

    List<ScatterplotDataPoint> scatterplotPoints = null;

    int loadedYear = DEFAULT_YEAR;
    int loadedSemester = DEFAULT_SEMESTER;

    bool shouldReloadScatterplot = false;
    int yearToLoad = DEFAULT_YEAR;
    int semesterToLoad = DEFAULT_SEMESTER;

    void Start()
    {
        LoadPoints(DEFAULT_YEAR, DEFAULT_SEMESTER);
    }

    void Update()
    {
        if (
            GameManager.Instance.IsViewEnabled(ApplicationView.Scatterplot) == false
            || shouldReloadScatterplot == false
        )
        {
            return;
        }

        shouldReloadScatterplot = false;

        loadedYear = yearToLoad;
        loadedSemester = semesterToLoad;

        DumpPoints();
        LoadPoints(yearToLoad, semesterToLoad);
    }

    public void LoadPoints(int year, int semester)
    {
        Debug.Log("Scatterplot: loading points for year " + year + " and semester " + semester);

        if (scatterplotPoints != null)
            DumpPoints();

        scatterplotPoints = new List<ScatterplotDataPoint>();

        SemesterUFF requestedSemester = DataManager.Instance.GetSemester(year, semester);

        if (requestedSemester == null)
        {
            Debug.Log("Semester not found");
            return;
        }

        int maxWomen = 0;
        int maxMen = 0;
        float maxDiff = 0.0f;

        foreach (CourseUFF course in requestedSemester.courses)
        {
            SexClassification sexClassification = DataManager.Instance.GetSexClassification(course);

            if (sexClassification.totalWomen > maxWomen)
            {
                maxWomen = sexClassification.totalWomen;
            }
            if (sexClassification.totalMen > maxMen)
            {
                maxMen = sexClassification.totalMen;
            }
            if (sexClassification.difference > maxDiff)
            {
                maxDiff = sexClassification.difference;
            }
        }

        foreach (CourseUFF course in requestedSemester.courses)
        {
            float normalizationFactor = 0.5f;

            SexClassification sexClassification = DataManager.Instance.GetSexClassification(course);

            float x =
                normalizationFactor
                * System.Convert.ToSingle(sexClassification.totalWomen)
                / maxWomen;

            float y =
                normalizationFactor
                * System.Convert.ToSingle(sexClassification.difference)
                / maxDiff;

            float z =
                normalizationFactor * System.Convert.ToSingle(sexClassification.totalMen) / maxMen;

            ScatterplotDataPoint newDataPoint = Instantiate(
                    pointPrefab,
                    new Vector3(x, y, z),
                    Quaternion.identity
                )
                .GetComponent<ScatterplotDataPoint>();

            newDataPoint.transform.position += this.transform.position;
            newDataPoint.transform.parent = this.transform;
            newDataPoint.gameObject.name = course.name;

            newDataPoint.dataClass = course.id.ToString();

            newDataPoint.textLabel.text = course.name + " (" + newDataPoint.dataClass + ")";

            Color newColor = new Color();

            newColor.a = 1f;

            newColor.r = x / normalizationFactor;
            newColor.g = y / normalizationFactor;
            newColor.b = z / normalizationFactor;

            newDataPoint.GetComponent<Renderer>().material.color = newColor;
            newDataPoint.pointColor = newColor;

            scatterplotPoints.Add(newDataPoint);
        }
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

        if (year == loadedYear && semester == loadedSemester)
        {
            return;
        }

        if (GameManager.Instance.IsViewEnabled(ApplicationView.Scatterplot) == false)
        {
            shouldReloadScatterplot = true;
            yearToLoad = year;
            semesterToLoad = semester;

            return;
        }

        loadedYear = year;
        loadedSemester = semester;

        DumpPoints();
        LoadPoints(year, semester);
    }
}
