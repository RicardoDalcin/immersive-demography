using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class PeopleView : MonoBehaviour
{
    public enum DataView
    {
        Sex,
        Ethnicity,
        Admission,
        Nationality,
        General
    }

    public struct PeopleNode
    {
        public GameObject person;
        public Color color;
        public Vector3 position;

        public PeopleNode(GameObject person, Color color, Vector3 position)
        {
            this.person = person;
            this.color = color;
            this.position = position;
        }
    }

    public GameObject characterPrefab;
    public GameObject environmentDesk;
    public float characterYPosition = 0.0f;

    public List<PeopleNode> people;

    public const int FIRST_YEAR = 2017;
    public const int DEFAULT_YEAR = 2017;
    public const int DEFAULT_SEMESTER = 1;

    public const int DEFAULT_COURSE_ID = 1;

    public const DataView DEFAULT_VIEW_ID = DataView.Sex;

    public const float BASE_PERSON_SCALE = 0.05f;

    Boundaries boundaries;

    int loadedYear = DEFAULT_YEAR;
    int loadedSemester = DEFAULT_SEMESTER;
    int loadedCourseId = DEFAULT_COURSE_ID;
    DataView loadedView = DEFAULT_VIEW_ID;

    float lastFrameScale = 1.0f;
    float virtualScale = 1.0f;

    bool shouldReloadPeople = true;
    int yearToLoad = DEFAULT_YEAR;
    int semesterToLoad = DEFAULT_SEMESTER;
    int courseIdToLoad = DEFAULT_COURSE_ID;
    DataView viewToLoad = DEFAULT_VIEW_ID;

    void Start()
    {
        Collider selfCollider = gameObject.GetComponent<Collider>();

        Collider deskCollider = environmentDesk.GetComponent<Collider>();

        float minX = deskCollider.bounds.min.x;
        float maxX = deskCollider.bounds.max.x;
        float minZ = deskCollider.bounds.min.z;
        float maxZ = deskCollider.bounds.max.z;

        boundaries = new Boundaries(minX, maxX, minZ, maxZ);
    }

    void Update()
    {
        if (GameManager.Instance.IsViewEnabled(ApplicationView.PeopleView) == false)
        {
            return;
        }

        if (shouldReloadPeople)
        {
            shouldReloadPeople = false;

            loadedYear = yearToLoad;
            loadedSemester = semesterToLoad;
            loadedCourseId = courseIdToLoad;

            LoadPoints(yearToLoad, semesterToLoad, courseIdToLoad, viewToLoad);
        }

        gameObject.transform.rotation = Quaternion.identity;

        float changeInScale = gameObject.transform.localScale.x - lastFrameScale;

        lastFrameScale = gameObject.transform.localScale.x;

        if (changeInScale != 0.0f && gameObject.transform.localScale.x != 1.0f)
        {
            virtualScale += changeInScale;

            if (virtualScale < 0.1f)
                virtualScale = 0.1f;

            if (virtualScale > 1.0f)
                virtualScale = 1.0f;
        }

        UpdatePeople();

        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    void UpdatePeople()
    {
        foreach (PeopleNode peopleNode in people)
        {
            GameObject person = peopleNode.person;
            Vector3 originalPosition = peopleNode.position;

            Vector3 personPosition = person.transform.localPosition;

            float x = personPosition.x;
            float z = personPosition.z;

            float xRelativeToCenter = originalPosition.x * virtualScale;
            float zRelativeToCenter = originalPosition.z * virtualScale;

            float zPosition = 0.4f * (1 - virtualScale);

            person.transform.localPosition = new Vector3(
                xRelativeToCenter,
                personPosition.y,
                zRelativeToCenter + zPosition
            );

            person.transform.localScale = new Vector3(
                BASE_PERSON_SCALE * virtualScale,
                BASE_PERSON_SCALE * virtualScale,
                BASE_PERSON_SCALE * virtualScale
            );
        }
    }

    public void LoadPoints(
        int year,
        int semester,
        int courseId,
        DataView view,
        bool grouped = false
    )
    {
        Debug.Log("PeopleView: loading " + view + " points for " + year + "/" + semester);

        if (people != null)
            DumpPoints();

        CourseUFF course = DataManager.Instance.GetCourse(year, semester, courseId);

        if (course == null)
        {
            Debug.Log("Semester or course not found");
            return;
        }

        if (course.id == courseId)
        {
            int courseWomen = 0;
            int courseMen = 0;

            foreach (ClassificationUFF sex in course.sex)
            {
                if (sex.name == "F")
                {
                    courseWomen = sex.total;
                }

                if (sex.name == "M")
                {
                    courseMen = sex.total;
                }
            }

            people = new List<PeopleNode>();

            if (grouped)
            {
                float womenGroupRadius = courseWomen / 3.5f;
                float menGroupRadius = courseMen / 3.5f;

                Vector2 womenGroupCenterXZ = new Vector2(0, -womenGroupRadius - 1);
                Vector2 menGroupCenterXZ = new Vector2(0, menGroupRadius + 1);

                for (int i = 0; i < courseWomen; i++)
                {
                    GameObject character = Instantiate(characterPrefab);
                    character.transform.parent = gameObject.transform;
                    character.transform
                        .GetChild(1)
                        .gameObject.GetComponent<Renderer>()
                        .material.color = new Color(0.4f, 0.1f, 1.0f, 1.0f);

                    float random = Random.Range(0, womenGroupRadius);
                    float theta = Random.Range(0, 2 * Mathf.PI);

                    float x = Mathf.Sqrt(random * womenGroupRadius) * Mathf.Cos(theta);
                    float z = Mathf.Sqrt(random * womenGroupRadius) * Mathf.Sin(theta);

                    float xRelativeToCenter = x + womenGroupCenterXZ.x;
                    float zRelativeToCenter = z + womenGroupCenterXZ.y;

                    character.transform.position = new Vector3(
                        xRelativeToCenter,
                        characterYPosition,
                        zRelativeToCenter
                    );
                }

                for (int i = 0; i < courseMen; i++)
                {
                    GameObject character = Instantiate(characterPrefab);
                    character.transform.parent = gameObject.transform;
                    character.transform
                        .GetChild(1)
                        .gameObject.GetComponent<Renderer>()
                        .material.color = new Color(1.0f, 0.6f, 0.1f, 1.0f);

                    float random = Random.Range(0, menGroupRadius);
                    float theta = Random.Range(0, 2 * Mathf.PI);

                    float x = Mathf.Sqrt(random * menGroupRadius) * Mathf.Cos(theta);
                    float z = Mathf.Sqrt(random * menGroupRadius) * Mathf.Sin(theta);

                    float xRelativeToCenter = x + menGroupCenterXZ.x;
                    float zRelativeToCenter = z + menGroupCenterXZ.y;

                    character.transform.position = new Vector3(
                        xRelativeToCenter,
                        characterYPosition,
                        zRelativeToCenter
                    );
                }
            }
            else
            {
                for (int i = 0; i < courseWomen; i++)
                {
                    GameObject character = Instantiate(characterPrefab);
                    character.transform.SetParent(gameObject.transform, true);
                    character.transform
                        .GetChild(1)
                        .gameObject.GetComponent<Renderer>()
                        .material.color = new Color(0.4f, 0.1f, 1.0f, 1.0f);
                    character.transform.localScale = new Vector3(
                        BASE_PERSON_SCALE * virtualScale,
                        BASE_PERSON_SCALE * virtualScale,
                        BASE_PERSON_SCALE * virtualScale
                    );

                    Vector2 horizontalPosition = boundaries.GetRandomPoint();
                    character.transform.localPosition = new Vector3(
                        horizontalPosition.x,
                        0.02f,
                        horizontalPosition.y
                    );

                    people.Add(
                        new PeopleNode(
                            character,
                            new Color(0.4f, 0.1f, 1.0f, 1.0f),
                            new Vector3(horizontalPosition.x, 0.02f, horizontalPosition.y)
                        )
                    );
                }

                for (int i = 0; i < courseMen; i++)
                {
                    GameObject character = Instantiate(characterPrefab);
                    character.transform.parent = gameObject.transform;
                    character.transform
                        .GetChild(1)
                        .gameObject.GetComponent<Renderer>()
                        .material.color = new Color(1.0f, 0.6f, BASE_PERSON_SCALE, 1.0f);
                    character.transform.localScale = new Vector3(
                        BASE_PERSON_SCALE * virtualScale,
                        BASE_PERSON_SCALE * virtualScale,
                        BASE_PERSON_SCALE * virtualScale
                    );

                    Vector2 horizontalPosition = boundaries.GetRandomPoint();
                    character.transform.localPosition = new Vector3(
                        horizontalPosition.x,
                        0.02f,
                        horizontalPosition.y
                    );

                    people.Add(
                        new PeopleNode(
                            character,
                            new Color(1.0f, 0.6f, BASE_PERSON_SCALE, 1.0f),
                            new Vector3(horizontalPosition.x, 0.02f, horizontalPosition.y)
                        )
                    );
                }
            }
        }
    }

    public void DumpPoints()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
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

        if (GameManager.Instance.IsViewEnabled(ApplicationView.PeopleView) == false)
        {
            shouldReloadPeople = true;
            yearToLoad = year;
            semesterToLoad = semester;
            courseIdToLoad = loadedCourseId;
            viewToLoad = loadedView;

            return;
        }

        loadedYear = year;
        loadedSemester = semester;

        DumpPoints();
        LoadPoints(year, semester, loadedCourseId, loadedView);
    }

    public void OnCourseSliderUpdated(SliderEventData eventData)
    {
        var arbitraryValue = float.Parse($"{eventData.NewValue}");
        var courseId = (int)(arbitraryValue * 80) + 1;

        if (courseId == loadedCourseId)
        {
            return;
        }

        if (GameManager.Instance.IsViewEnabled(ApplicationView.PeopleView) == false)
        {
            shouldReloadPeople = true;
            yearToLoad = loadedYear;
            semesterToLoad = loadedSemester;
            courseIdToLoad = courseId;
            viewToLoad = loadedView;

            return;
        }

        loadedCourseId = courseId;

        DumpPoints();
        LoadPoints(loadedYear, loadedSemester, courseId, loadedView);
    }

    public void OnViewSliderUpdated(SliderEventData eventData)
    {
        var arbitraryValue = float.Parse($"{eventData.NewValue}");
        var viewId = (int)(arbitraryValue * 4);

        var view = (DataView)viewId;

        if (view == loadedView)
        {
            return;
        }

        if (GameManager.Instance.IsViewEnabled(ApplicationView.PeopleView) == false)
        {
            shouldReloadPeople = true;
            yearToLoad = loadedYear;
            semesterToLoad = loadedSemester;
            courseIdToLoad = loadedCourseId;
            viewToLoad = view;

            return;
        }

        loadedView = view;

        DumpPoints();
        LoadPoints(loadedYear, loadedSemester, loadedCourseId, view);
    }
}
