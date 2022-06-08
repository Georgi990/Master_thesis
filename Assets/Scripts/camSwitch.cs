using UnityEngine;

public class camSwitch : MonoBehaviour
{
    public Camera camStatic;
    public Camera camFollow1;
    public Camera camTopView;
    public Camera camPanoram;
    public Camera camPredFP;

    private void Start()
    {
        camStatic.gameObject.SetActive(false);
        camFollow1.gameObject.SetActive(false);
        camTopView.gameObject.SetActive(true);
        camPanoram.gameObject.SetActive(false);
        camPredFP.gameObject.SetActive(false);

        Debug.Log("Press: 1 - Static camera, 2 - Follow predator, 3 - Top View, 4 - Panoram view, 5 - Predator first person view");
    }

    void Update()
    {
        CheckWhichCam();
    }

    void CheckWhichCam()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) //static cam
        {
            camStatic.gameObject.SetActive(true);
            camFollow1.gameObject.SetActive(false);
            camTopView.gameObject.SetActive(false);
            camPanoram.gameObject.SetActive(false);
            camPredFP.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camStatic.gameObject.SetActive(false); //follow cam1
            camFollow1.gameObject.SetActive(true);
            camTopView.gameObject.SetActive(false);
            camPanoram.gameObject.SetActive(false);
            camPredFP.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            camStatic.gameObject.SetActive(false); //topViewCam
            camFollow1.gameObject.SetActive(false);
            camTopView.gameObject.SetActive(true);
            camPanoram.gameObject.SetActive(false);
            camPredFP.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            camStatic.gameObject.SetActive(false); //panoramCam
            camFollow1.gameObject.SetActive(false);
            camTopView.gameObject.SetActive(false);
            camPanoram.gameObject.SetActive(true);
            camPredFP.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            camStatic.gameObject.SetActive(false); //predFPview
            camFollow1.gameObject.SetActive(false);
            camTopView.gameObject.SetActive(false);
            camPanoram.gameObject.SetActive(false);
            camPredFP.gameObject.SetActive(true);
        }
    }
}
