using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingController : MonoBehaviour
{
    [SerializeField] Animator firstTextAUTO;
    [SerializeField] Animator secondTextAUTO;
    [SerializeField] Animator thirdTextAUTO;
    [SerializeField] Animator fourthTextAUTO;

    [SerializeField] Animator demostrationElementsAUTO;
    [SerializeField] Animator tablaDebilidadesAUTO;
    [SerializeField] GameObject canvasFocus;

    [SerializeField] GameObject player;
    [SerializeField] PlantEnemy enemyFirst;

    private bool isAttacking;
    private bool secondCheckPointReached;
    // Start is called before the first frame update
    void Start()
    {
        isAttacking = false;
        secondCheckPointReached = false;

        firstTextAUTO.enabled = false;
        secondTextAUTO.enabled = false;
        thirdTextAUTO.enabled = false;
        fourthTextAUTO.enabled = false;

        demostrationElementsAUTO.enabled = false;
        tablaDebilidadesAUTO.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfAttacking();
        CheckIfSecondCheckpointReached();
        Debug.Log("player pos" + player.gameObject.transform.position.z);
    }

    IEnumerator Manager()
    {
        firstTextAUTO.enabled = true;

        //No puede moverse hasta que acabe la animacion del texto
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2.5f);    //lo que dura la animacion del texto hardcodded
        //dejar mover
        Time.timeScale = 1;

        //Cuando el enemigo te dispare pausar juego y poner texto de esquivar
        yield return WaitUntilTrue(CheckIfAttacking);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
        secondTextAUTO.enabled = true;
        //dejar mover despues de un par de segundos
        yield return new WaitForSecondsRealtime(1.5f);//dura animacion
        yield return WaitUntilTrue(IsMousePressed);
        Time.timeScale = 1;

        //Cuando vea el siguiente enemigo pausar y decir puede coger cobertura
        yield return WaitUntilTrue(CheckIfSecondCheckpointReached);
        Time.timeScale = 0;
        thirdTextAUTO.enabled = true;
        yield return new WaitForSecondsRealtime(1.5f);//animacion
        yield return WaitUntilTrue(IsMousePressed);
        Time.timeScale = 1;

        //Pausar para enseñar como enseñar elemento
        yield return WaitUntilTrue(CheckIfThirdCheckpointReached);
        Time.timeScale = 0;
        fourthTextAUTO.enabled = true;
        demostrationElementsAUTO.enabled = true;
        yield return new WaitForSecondsRealtime(3.5f);
        canvasFocus.SetActive(true);
        yield return WaitUntilTrue(IsMousePressed);
        canvasFocus.SetActive(false);
        fourthTextAUTO.enabled = false;
        fourthTextAUTO.gameObject.SetActive(false);
        tablaDebilidadesAUTO.enabled = true;
        yield return new WaitForSecondsRealtime(3f);//Poner un panel para mostrar bien la UI
        yield return WaitUntilTrue(IsMousePressed);
        Time.timeScale = 1;
        demostrationElementsAUTO.enabled = false;


    }

    bool CheckIfAttacking()
    {
        if (enemyFirst.plantAttacking)
        {
            isAttacking = true;
        }
        if (isAttacking)
            return true;
      return false;
    }

    bool CheckIfSecondCheckpointReached()
    {
        //numero a pasar -3.748482z

        if (player.gameObject.transform.position.z >= -3.748482f)
        {
            secondCheckPointReached = true;
            return secondCheckPointReached;
        }
        return false;
    }

    bool CheckIfThirdCheckpointReached()
    {
        if (player.gameObject.transform.position.z >= 0f)
        {
            return true;
        }
        return false;
    }

    bool IsMousePressed()
    {
        return Input.GetKeyDown(KeyCode.Mouse1);
    }

    IEnumerator WaitUntilTrue(System.Func<bool> checkMethod)
    {
        while (checkMethod() == false)
            yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            StartCoroutine(Manager());
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
