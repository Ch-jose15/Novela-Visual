using UnityEngine;
using TMPro;
using System.Xml;
using System.Collections;
using UnityEngine.UI;
using System;

public class GestorDialogos : MonoBehaviour
{
    public TextMeshProUGUI txtNombre;    
    public TextMeshProUGUI txtDialogo;   
    public GameObject fondoEscena1;
    public GameObject fondoEscena2;
    public float velocidadEscritura = 0.05f;
    public GameObject personajeIzquierda, personajeDerecha;
    public GameObject panelhistorial;
    public TextMeshProUGUI txtLogPrefab;
    public Transform contenedorLog;

    private XmlDocument documentoXml;
    private XmlNodeList frases;
    private int indiceFrase = 0;
    private string idEscenaActual = "Escena1";
    private Coroutine corrutinaEscritura;
    private bool estaEscribiendo = false;
    private string textoCompletoActual = "";

    /*-------------------------------------------------------------------------*/

    void Start()
    {
        CargarXML();
        CargarBloqueDeTexto("Escena1");
    }

    /*-------------------------------------------------------------------------*/

    void CargarXML()
    {
        TextAsset xmlData = Resources.Load<TextAsset>("Dialogos");
        if (xmlData != null)
        {
            documentoXml = new XmlDocument();
            documentoXml.LoadXml(xmlData.text);
        }
    }

    /*-------------------------------------------------------------------------*/

    public void CargarBloqueDeTexto(string idEscena)
    {
        idEscenaActual = idEscena;
        frases = documentoXml.SelectNodes($"//Escena[@id='{idEscena}']/Frase");
        indiceFrase = 0;
        ActualizarFondosVisuales(idEscena);
        ComenzarEscritura();
    }

    /*-------------------------------------------------------------------------*/

    void ActualizarFondosVisuales(string id)
    {
        fondoEscena1.SetActive(id == "Escena1");
        fondoEscena2.SetActive(id == "Escena2");
    }

    /*-------------------------------------------------------------------------*/

    public void ClickBotonSiguiente()
    {
        if (estaEscribiendo)
        {
            StopCoroutine(corrutinaEscritura);
            txtDialogo.text = textoCompletoActual;
            estaEscribiendo = false;
        }
        else
        {
            if (indiceFrase < frases.Count - 1)
            {
                indiceFrase++;
                ComenzarEscritura();
            }
            else
            {
                if (idEscenaActual == "Escena1") CargarBloqueDeTexto("Escena2");
                else txtDialogo.text = "Fin de la prueba. Hasta que hagamos el dialogo :)"; 
            }
        }
    }

    /*-------------------------------------------------------------------------*/

    void ComenzarEscritura()
    {
        string nombre = frases[indiceFrase].Attributes["personaje"].Value;
        string codigoColor = frases[indiceFrase].Attributes["color"].Value;
        string lado = frases[indiceFrase].Attributes["lado"].Value;

        txtNombre.text = $"<color={codigoColor}>{nombre}</color>";
        GestionarPersonajes(lado);
        textoCompletoActual = frases[indiceFrase].InnerText;

        RegistrarEnHistorial(nombre, frases[indiceFrase].InnerText, codigoColor);

        if (corrutinaEscritura != null) StopCoroutine(corrutinaEscritura);
        corrutinaEscritura = StartCoroutine(EscribirTexto());
    }

    /*-------------------------------------------------------------------------*/

    IEnumerator EscribirTexto()
    {
        estaEscribiendo = true;
        txtDialogo.text = "";

        foreach(char letra in textoCompletoActual.ToCharArray())
        {
            txtDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        estaEscribiendo = false;
    }

    /*-------------------------------------------------------------------------*/

    void GestionarPersonajes(string posicion)
    {
        Color activo =Color.white;
        Color inactivo = new Color(0.3f, 0.3f, 0.3f, 1.0f);

        if (posicion == "izq")
        {
            personajeIzquierda.SetActive(true);
            personajeIzquierda.GetComponent<Image>().color = activo;
            personajeDerecha.GetComponent<Image>().color = inactivo;
        }
        else if (posicion == "der")
        {
            personajeDerecha.SetActive(true);
            personajeDerecha.GetComponent<Image>().color = activo;
            personajeIzquierda.GetComponent<Image>().color = inactivo;
        }
        else
        {
            personajeIzquierda.SetActive(false);
            personajeDerecha.SetActive(false);
        }
    }

    /*-------------------------------------------------------------------------*/

    public void AbrirCerrarLog()
    {
        panelhistorial.SetActive(!panelhistorial.activeSelf);
    }

    /*-------------------------------------------------------------------------*/

    void RegistrarEnHistorial(string nombre, string frase, string color)
    {
        TextMeshProUGUI nuevaLinea = Instantiate(txtLogPrefab, contenedorLog);
        nuevaLinea.text = $"<color={color}><b>{nombre}: </b></color>{frase}";
    }
}