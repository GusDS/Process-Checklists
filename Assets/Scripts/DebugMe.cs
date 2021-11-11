using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMe : MonoBehaviour
{
    private Text debugText;
    private struct ToWatch
    {
        public string texto;
        public string valor;
    }
    private List<ToWatch> watches = new List<ToWatch>();
    private int posicion;
    private string watchtexto;

    private void Start()
    {
        debugText = GetComponent<Text>();
    }
    public void Show(string texto, string valor)
    {
        ToWatch item = new ToWatch();
        item.texto = texto;
        item.valor = valor;

        posicion = watches.FindIndex(x => x.texto == texto);

        if (posicion == -1)
        {
            watches.Add(item);
        }
        else
        {
            watches.RemoveAt(posicion);
            watches.Insert(posicion, item);
        }
    }

    void Update()
    {
        watchtexto = string.Empty;
        for (int i = 0; i < watches.Count; i++)
        {
            watchtexto += watches[i].texto + ":\t" + watches[i].valor + "\n";
        }
        // GameObject.Find("DebutText").GetComponent<Text>().text = watchtexto;
        debugText.text = watchtexto;
    }
}
