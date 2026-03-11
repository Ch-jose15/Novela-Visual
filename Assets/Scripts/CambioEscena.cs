using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public void EmpezarJuego()
    {
        SceneManager.LoadScene("Juego");
    }

    /*---------------------------------------------------------------*/

    public void SalirDelJuego()
    {
        Application.Quit();
    }
}
