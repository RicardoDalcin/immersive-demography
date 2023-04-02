// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

[AddComponentMenu("Scripts/MRTK/Examples/ShowSliderValue")]
public class ShowCourseValue : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMesh = null;

    List<SimpleCourseUFF> courseList = null;

    void Start()
    {
        ParseDataset("Assets/Resources/Data/courses.json");
    }

    public void ParseDataset(string datasetPath)
    {
        string text = File.ReadAllText(datasetPath);

        CoursesUFF courses = JsonUtility.FromJson<CoursesUFF>(text);

        courseList = courses.Course;
    }

    public void OnSliderUpdated(SliderEventData eventData)
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        if (textMesh != null)
        {
            var foobar = float.Parse($"{eventData.NewValue}");
            var courseId = (int)(foobar * 80) + 1;

            var course = courseList.Find(c => c.id == courseId);

            var name = course.name;

            if(course.name.Length > 20){
                name = course.name.Substring(0, 20) + "...";
            }

            textMesh.text = name;
        }
    }
}
