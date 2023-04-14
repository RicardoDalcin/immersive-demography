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

    bool shouldGroup = false;

    List<PeopleDataType> dataTypes;

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

            LoadPoints(yearToLoad, semesterToLoad, courseIdToLoad, viewToLoad, shouldGroup);
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

    public void LoadPoints(int year, int semester, int courseId, DataView view, bool grouped = true)
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
            dataTypes = new List<PeopleDataType>();

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

            if (grouped)
            {
                int index = 0;
                float groupRadius = 0.0f;
                Vector2 groupCenterXZ = new Vector2(0, 0);

                foreach (PeopleDataType data in dataTypes)
                {
                    float lastGroupRadius = groupRadius;
                    Vector2 lastGroupCenterXZ = groupCenterXZ;

                    groupRadius = data.total / 100.0f;

                    if (index == 0)
                        groupCenterXZ = new Vector2(
                            boundaries.GetMinX() + groupRadius + 0.1f,
                            (boundaries.GetMinZ() + boundaries.GetMaxZ()) / 2.0f
                        );
                    else
                        groupCenterXZ = new Vector2(
                            lastGroupCenterXZ.x + lastGroupRadius + groupRadius + 0.03f,
                            lastGroupCenterXZ.y
                        );

                    // Generate the circle points
                    List<Vector2> circlePoints = GenerateCirclePoints(groupRadius, data.total);

                    for (int i = 0; i < data.total; i++)
                    {
                        GameObject character = Instantiate(characterPrefab);
                        character.transform.SetParent(gameObject.transform, true);
                        character.transform
                            .GetChild(1)
                            .gameObject.GetComponent<Renderer>()
                            .material.color = data.color;
                        character.transform.localScale = new Vector3(
                            BASE_PERSON_SCALE * virtualScale,
                            BASE_PERSON_SCALE * virtualScale,
                            BASE_PERSON_SCALE * virtualScale
                        );

                        // Get the point from the circle point list
                        Vector2 point = circlePoints[i];

                        float xRelativeToCenter = point.x + groupCenterXZ.x;
                        float zRelativeToCenter = point.y + groupCenterXZ.y;

                        if (xRelativeToCenter > boundaries.GetMaxX())
                            xRelativeToCenter = boundaries.GetMaxX() - 0.1f;
                        else if (xRelativeToCenter < boundaries.GetMinX())
                            xRelativeToCenter = boundaries.GetMinX() + 0.1f;

                        if (zRelativeToCenter > boundaries.GetMaxZ())
                            zRelativeToCenter = boundaries.GetMaxZ() - 0.1f;
                        else if (zRelativeToCenter < boundaries.GetMinZ())
                            zRelativeToCenter = boundaries.GetMinZ() + 0.1f;

                        character.transform.localPosition = new Vector3(
                            xRelativeToCenter,
                            0.02f,
                            zRelativeToCenter
                        );

                        float randomRotation = Random.Range(0, 360);

                        character.transform.Rotate(new Vector3(0, randomRotation, 0));

                        people.Add(
                            new PeopleNode(
                                character,
                                data.color,
                                new Vector3(xRelativeToCenter, 0.02f, zRelativeToCenter)
                            )
                        );
                    }

                    index += 1;
                }
            }
            else
            {
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

                        float randomRotation = Random.Range(0, 360);

                        character.transform.Rotate(new Vector3(0, randomRotation, 0));

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
    }

    public void DumpPoints()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private List<Vector2> GenerateCirclePoints(float radius, int count)
    {
        List<Vector2> points = new List<Vector2>();
        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2; // golden ratio constant

        for (int i = 0; i < count; i++)
        {
            float angle = 2 * Mathf.PI * i / goldenRatio;
            float distance = Mathf.Sqrt(i + 0.5f) / Mathf.Sqrt(count);

            float x = radius * distance * Mathf.Cos(angle);
            float z = radius * distance * Mathf.Sin(angle);

            points.Add(new Vector2(x, z));
        }

        return points;
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
        LoadPoints(year, semester, loadedCourseId, loadedView, shouldGroup);
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
        LoadPoints(loadedYear, loadedSemester, courseId, loadedView, shouldGroup);
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
        LoadPoints(loadedYear, loadedSemester, loadedCourseId, view, shouldGroup);
    }

    public void OnGroupSwitchUpdate()
    {
        shouldGroup = !shouldGroup;

        DumpPoints();
        LoadPoints(loadedYear, loadedSemester, loadedCourseId, loadedView, shouldGroup);
    }

    public string GetLabel() 
    {
        var result = "";

        foreach (PeopleDataType dataType in dataTypes){
            result += "<color=#" + ColorUtility.ToHtmlStringRGB(dataType.color) + ">" + dataType.label + "</color>\n";
        }

        return result;
    }
}
