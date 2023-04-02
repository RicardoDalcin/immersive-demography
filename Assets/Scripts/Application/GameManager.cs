using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ApplicationView
{
    scatterplot,
    peopleView
};

public class GameManager : Singleton<GameManager>
{
    public GameObject scatterplot;
    public GameObject peopleView;

    ApplicationView currentView;

    void Start()
    {
        currentView = ApplicationView.scatterplot;
        EnableView(currentView);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SwitchView();
        }
    }

    public void SwitchView()
    {
        if (currentView == ApplicationView.scatterplot)
        {
            currentView = ApplicationView.peopleView;
        }
        else if (currentView == ApplicationView.peopleView)
        {
            currentView = ApplicationView.scatterplot;
        }

        EnableView(currentView);
    }

    void EnableView(ApplicationView view)
    {
        if (view == ApplicationView.scatterplot)
        {
            scatterplot.SetActive(true);
            peopleView.SetActive(false);

            Behaviour[] behaviours = scatterplot.GetComponents<Behaviour>();

            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.enabled = true;
            }

            Behaviour[] peopleViewBehaviours = peopleView.GetComponents<Behaviour>();

            foreach (Behaviour behaviour in peopleViewBehaviours)
            {
                behaviour.enabled = false;
            }
        }
        else if (view == ApplicationView.peopleView)
        {
            scatterplot.SetActive(false);
            peopleView.SetActive(true);

            Behaviour[] behaviours = peopleView.GetComponents<Behaviour>();

            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.enabled = true;
                // if (behaviour.GetType().toString() == "Microsoft.MixedReality.Toolkit.UI.ObjectManipulator")
                // {
                //     behaviour.enabled = true;
                // }
            }

            Behaviour[] scatterplotBehaviours = scatterplot.GetComponents<Behaviour>();

            foreach (Behaviour behaviour in scatterplotBehaviours)
            {
                behaviour.enabled = false;
            }
        }
    }
}
