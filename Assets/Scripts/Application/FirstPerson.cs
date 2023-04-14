using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class FirstPerson : MonoBehaviour
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

    public struct PeopleDataType
    {
        public int total;
        public string label;
        public Color color;

        public PeopleDataType(int total, string label, Color color)
        {
            this.total = total;
            this.label = label;
            this.color = color;
        }
    }

    public GameObject characterPrefab;
    public GameObject mainCamera;
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
    Vector2 displacement = new Vector2(0.0f, 0.0f);

    bool shouldReloadPeople = true;
    int yearToLoad = DEFAULT_YEAR;
    int semesterToLoad = DEFAULT_SEMESTER;
    int courseIdToLoad = DEFAULT_COURSE_ID;
    DataView viewToLoad = DEFAULT_VIEW_ID;

    void Start()
    {
        boundaries = new Boundaries(-20, 20, -20, 20);
    }

    void Update()
    {
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
            // convert scale up to forward movement in the direction of the camera
            // scale down is the opposite
            float forwardMovement = changeInScale * 10.0f;

            Vector3 forward = mainCamera.transform.forward;

            displacement += new Vector2(forward.x, forward.z) * forwardMovement;
        }

        UpdatePeople();

        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    void UpdatePeople()
    {
        Debug.Log("Camera forward: " + mainCamera.transform.forward);
        Debug.Log("Displacement: " + displacement);
        foreach (PeopleNode peopleNode in people)
        {
            GameObject person = peopleNode.person;
            Vector3 originalPosition = peopleNode.position;

            Vector3 personPosition = person.transform.localPosition;

            float x = personPosition.x;
            float z = personPosition.z;

            person.transform.localPosition = new Vector3(
                originalPosition.x - displacement.x,
                originalPosition.y,
                originalPosition.z - displacement.y
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
            List<PeopleDataType> dataTypes = new List<PeopleDataType>();

            if (view == DataView.Sex)
            {
                SexClassification classification = DataManager.Instance.GetSexClassification(
                    course
                );

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalWomen,
                        "Feminino",
                        new Color(0.4f, 0.1f, 1.0f, 1.0f)
                    )
                );

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalMen,
                        "Masculino",
                        new Color(1.0f, 0.6f, 0.5f, 1.0f)
                    )
                );
            }
            else if (view == DataView.Ethnicity)
            {
                EthnicityClassification classification =
                    DataManager.Instance.GetEthnicityClassification(course);

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalBlack,
                        "Negra",
                        new Color(0.7f, 0.4f, 0.3f, 1.0f)
                    )
                );

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalBrown,
                        "Parda",
                        new Color(0.8f, 0.2f, 0.2f, 1.0f)
                    )
                );

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalWhite,
                        "Branca",
                        new Color(0.2f, 0.9f, 0.5f, 1.0f)
                    )
                );

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalUndeclared,
                        "Não declarada",
                        new Color(0.4f, 0.4f, 0.9f, 1.0f)
                    )
                );
            }
            else if (view == DataView.Admission)
            {
                AdmissionClassification classification =
                    DataManager.Instance.GetAdmissionClassification(course);

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalSisu,
                        "SISU",
                        new Color(0.4f, 0.1f, 0.8f, 1.0f)
                    )
                );

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalCourseChange,
                        "Mudança de Curso",
                        new Color(0.8f, 0.8f, 0.2f, 1.0f)
                    )
                );

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalExternalTransference,
                        "Transferência Interinstitucional",
                        new Color(0.2f, 0.9f, 0.1f, 1.0f)
                    )
                );

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalPublicReentry,
                        "Reingresso Concurso Público",
                        new Color(0.1f, 0.8f, 0.4f, 1.0f)
                    )
                );
            }
            else if (view == DataView.Nationality)
            {
                NationalityClassification classification =
                    DataManager.Instance.GetNationalityClassification(course);

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalBrazilian,
                        "Brasileira",
                        new Color(0.8f, 0.4f, 0.7f, 1.0f)
                    )
                );

                dataTypes.Add(
                    new PeopleDataType(
                        classification.totalForeigner,
                        "Estrangeira",
                        new Color(0.3f, 0.9f, 0.3f, 1.0f)
                    )
                );
            }

            people = new List<PeopleNode>();

            foreach (PeopleDataType data in dataTypes)
            {
                for (int i = 0; i < data.total; i++)
                {
                    GameObject character = Instantiate(characterPrefab);
                    character.transform.SetParent(gameObject.transform, true);
                    character.transform
                        .GetChild(1)
                        .gameObject.GetComponent<Renderer>()
                        .material.color = data.color;

                    Vector2 horizontalPosition = boundaries.GetRandomPoint();

                    character.transform.localPosition = new Vector3(
                        horizontalPosition.x,
                        characterYPosition,
                        horizontalPosition.y
                    );

                    float randomRotation = Random.Range(0, 360);

                    character.transform.Rotate(new Vector3(0, randomRotation, 0), Space.Self);
                    people.Add(
                        new PeopleNode(
                            character,
                            data.color,
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

        loadedView = view;

        DumpPoints();
        LoadPoints(loadedYear, loadedSemester, loadedCourseId, view);
    }
}
