using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingController : MonoBehaviour
{
    [SerializeField] Animator firstTextAUTO;
    [SerializeField] Animator secondTextAUTO;
    //[SerializeField] Animator thirdTextAUTO;
    [SerializeField] Animator fourthTextAUTO;
    [SerializeField] Animator fifthTextAUTO;
    [SerializeField] Animator sixthTextAUTO;
    [SerializeField] Animator seventhTextAUTO;

    [SerializeField] Animator demostrationElementsAUTO;
    [SerializeField] Animator tablaDebilidadesAUTO;
    [SerializeField] GameObject canvasFocus;
    [SerializeField] GameObject paredInvisibleRaton;
    //[SerializeField] GameObject paredInvisible2;

    [SerializeField] GameObject player;
    [SerializeField] GameObject ratEnemy;   //Solo necesito freeze para que no se mueva hasta x momento
    [SerializeField] PlantEnemy enemyFirst;

    private bool isAttacking;
    private bool secondCheckPointReached;

    [SerializeField] ElementsManager manager;
    // Start is called before the first frame update
    void Start()
    {
        isAttacking = false;
        secondCheckPointReached = false;

        firstTextAUTO.enabled = false;
        secondTextAUTO.enabled = false;
        //thirdTextAUTO.enabled = false;
        fourthTextAUTO.enabled = false;
        fifthTextAUTO.enabled = false;
        sixthTextAUTO.enabled = false;
        seventhTextAUTO.enabled = false;

        demostrationElementsAUTO.enabled = false;
        tablaDebilidadesAUTO.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfAttacking();
        CheckIfSecondCheckpointReached();
        //if (ratEnemy != null && ratEnemy.gameObject.activeInHierarchy)
        //    ;//CheckIfFirstEnemyDead();
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

        //Cuando vea el siguiente enemigo pausar y decir funcion elementos
        yield return WaitUntilTrue(CheckIfSecondCheckpointReached);
        Time.timeScale = 0;
        paredInvisibleRaton.SetActive(false);
        //ratEnemy.gameObject.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePosition;  //Desfreeze al raton

        fourthTextAUTO.enabled = true;
        yield return new WaitForSecondsRealtime(4.5f);
        fourthTextAUTO.enabled = false;
        fourthTextAUTO.gameObject.SetActive(false);
        fifthTextAUTO.enabled = true;
        manager.tutorialDone = true;
        yield return WaitUntilTrue(IsButton2Pressed);
        Time.timeScale = 1;
        fifthTextAUTO.enabled = false;
        fifthTextAUTO.gameObject.SetActive(false);
        sixthTextAUTO.enabled = true;

        //Cuando muera raton
        yield return WaitUntilTrue(CheckIfRatEnemyDead);

        yield return WaitUntilTrue(CheckIfThirdCheckpointReached);
        //paredInvisible2.SetActive(false);
        Time.timeScale = 0;
        seventhTextAUTO.enabled = true;
        tablaDebilidadesAUTO.enabled = true;
        sixthTextAUTO.enabled = false;
        sixthTextAUTO.gameObject.SetActive(false);
        yield return WaitUntilTrue(IsMousePressed);  //Tarda en aparecer el panel
        canvasFocus.SetActive(true);
        demostrationElementsAUTO.enabled = true;
        seventhTextAUTO.enabled = false;
        seventhTextAUTO.gameObject.SetActive(false);
        tablaDebilidadesAUTO.enabled = false;
        tablaDebilidadesAUTO.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(4f);  //Tarda en aparecer el panel
        yield return WaitUntilTrue(IsMousePressed);
        demostrationElementsAUTO.enabled = false;
        demostrationElementsAUTO.gameObject.SetActive(false);
        canvasFocus.SetActive(false);
        Time.timeScale = 1;

        PlayerPrefs.SetInt("TutorialHasPlayed", 1);
       
    }

    void CheckIfFirstEnemyDead()
    {
        //Esta funcion desactivara el freeze del raton 
        if (enemyFirst == null)//si es null significa que ha muerto o que no esta asignado en el inspector
        {
            ratEnemy.gameObject.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePosition;
        }
    }

    bool CheckIfRatEnemyDead()
    {
        return ratEnemy.GetComponent<LifeSystem>().currLife <= 0;
    }

    bool CheckIfAttacking()
    {
        if (enemyFirst != null && enemyFirst.plantAttacking)
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
        if (player.gameObject.transform.position.z >= 3f)
        {
            return true;
        }
        return false;
    }

    bool IsMousePressed()
    {
        return Input.GetKeyDown(KeyCode.Mouse1);
    }

    bool IsButton2Pressed()
    {
        return Input.GetKeyDown(KeyCode.Alpha2);
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
