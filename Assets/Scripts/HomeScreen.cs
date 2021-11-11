using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySqlX;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Relational;
/*
using MySql.Data.Common;
using MySql.Data.MySqlClient.Authentication;
using MySql.Data.MySqlClient.Interceptors;
using MySql.Data.MySqlClient.Memcached;
using MySql.Data.MySqlClient.Replication;
using MySql.Data.MySqlClient.X;
using MySql.Data.MySqlClient.X.XDevAPI;
using MySql.Data.MySqlClient.X.XDevAPI.Common;
using MySql.Data.Types;
using MySqlX.Protocol;
using MySqlX.Serialization;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;
*/

namespace ProcessChecklists
{
    public class HomeScreen : MonoBehaviour
    {
        public DebugMe debugMe;
        // private MySqlConnection connection;
        private Schema connection;
        public Button executeButton;
        public Button profileButton;
        // Prueba Paneles
        public Text mainAreaText;
        public GameObject homeContent;
        public GameObject taskPrefab;
        private List<String[]> checklist = new List<String[]>();
        private int panelCount = 0;
        private Rect rectContent;
        // Profile Panel
        public GameObject profilePanel;
        public float profilePanelSpeed = 10000;
        private RectTransform profilePanelRect;
        private bool profilePanelActive;
        private bool profilePanelOpening;
        private bool profilePanelClosing;
        private float profilePanelW;

        void Start()
        {
            executeButton.onClick.AddListener(PruebaPaneles);
            profileButton.onClick.AddListener(ToggleProfilePanel);
            // executeButton.onClick.AddListener(getChecklistAsync);
            // Login();
            // Profile Panel
            profilePanelRect = profilePanel.GetComponent<RectTransform>();
            profilePanelActive = true;
            profilePanelOpening = false;
            profilePanelClosing = false;
            // ToggleProfilePanel();
            profilePanelW = profilePanelRect.rect.width;
            // profilePanel.transform.Translate(Vector3.right * -panelWidth);
        }

        private void Update()
        {
            //debugMe.Show("profilePanel.transform.position.x", profilePanel.transform.position.x.ToString());
            //debugMe.Show("profilePanel.transform.position.y", profilePanel.transform.position.y.ToString());
            //debugMe.Show("profilePanel.transform.position.z", profilePanel.transform.position.z.ToString());

            if (profilePanelOpening)
            {
                profilePanel.transform.Translate(Vector3.right * profilePanelSpeed * Time.deltaTime);
                if (profilePanel.transform.position.x >= 0)
                {
                    // profilePanel.transform.Translate(Vector3.zero);
                    profilePanel.transform.position = Vector3.zero;
                    profilePanelOpening = false;
                    profilePanelActive = true;
                }
            }
            if (profilePanelClosing)
            {
                profilePanel.transform.Translate(Vector3.left * profilePanelSpeed * Time.deltaTime);
                if (profilePanel.transform.position.x <= -profilePanelW)
                {
                    // profilePanel.transform.Translate(Vector3.right * -profilePanelW);
                    profilePanel.transform.position = new Vector3(-profilePanelW, 0, 0);
                    profilePanelClosing = false;
                    profilePanelActive = false;
                }
            }
            // profilePanelW = profilePanelRect.rect.width;
            // profilePanel.transform.Translate(Vector3.right * -panelWidth);
        }

        void SetupConnection()
        {
/*
            if (connection == null) {
                string connectionString = "host=127.0.0.1; port=3306; protocol=socket; database=pcli; uid=gdisalvo; pwd=Palpen00";
                try {
                    connection = new MySqlConnection(connectionString);
                    connection.Open();
                }
                catch (MySqlException ex) {
                    Debug.LogError("MySQL Error: " + ex.ToString());
                }
            }
*/
            if (connection == null) {
                try {
                    connection = MySQLX.GetSession("server=localhost; port=3306; user=gdisalvo; password=Palpen00").GetSchema("pcli");
                }
                catch (MySqlException ex) {
                    Debug.LogError("MySQL Error: " + ex.ToString());
                }
            }

        }

        private void CloseConnection()
        {
            // if (connection != null) { connection.Close(); }
        }

        void PruebaPaneles()
        {
            panelCount = 0;
            // rectContent = homeContent.GetComponent<Rect>();
            for (int i = 0; i < 100; i++)
            {
                GameObject tmpPrefab = Instantiate(taskPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                tmpPrefab.transform.SetParent(homeContent.transform); // GameObject.Find("HomeContent").transform;
                // No se necesita posicionar a mano nuevos paneles ni resize de content padre.
                // Agregando en Viewport > HomeContent componentes Content Size Fitter (ambos Preferred Size) y Vertical Layout Group hace resize automático
                // tmpPrefab.transform.localPosition = new Vector3(0, panelCount * -37, 0);
                foreach (var textField in tmpPrefab.GetComponentsInChildren<Text>())
                {
                    if (textField.name == "TaskText") textField.text = "Tarea de Prueba " + i;
                    else textField.text = (i * 100).ToString();
                }
                panelCount++;
            }
            rectContent = homeContent.GetComponent<RectTransform>().rect;
            /*
            homeContent.GetComponent<RectTransform>().rect.Set(0, 0, 0, panelCount * 37); //  .Set(0, 0, 0, panelCount * 37);
            rectContent = homeContent.GetComponent<RectTransform>().rect;
            */
            Debug.Log("Rect:" + rectContent.ToString());
            Debug.Log("Altura contenido bloque Text:" + panelCount * 37);
        }

        void ToggleProfilePanel()
        {
            // profilePanelW = profilePanelRect.rect.width;
            // profilePanel.SetActive(!profilePanel.activeSelf);
            if (!profilePanelOpening & !profilePanelClosing) {
                if (profilePanelActive) {
                    profilePanelClosing = true;
                    // profilePanel.transform.Translate(Vector3.right * -profilePanelW);
                    // profilePanelActive = false;
                }
                else {
                    profilePanelOpening = true;
                    // profilePanel.transform.Translate(Vector3.right * profilePanelW);
                    // profilePanelActive = true;
                }
            }
        }

        async void getChecklistAsync()
        {
            // Si no está logueado, ir a Login 1ro. ToDo
            if (connection == null) {
                SetupConnection();
            } else {
                var t_checklists = connection.GetTable("T_Checklists");
                Task<RowResult> getChecklistTask = t_checklists.Select("nu_task_sort", "st_task") // var select = employees.Select("nu_task_sort", "st_task")
                  .Where("nu_process = :id")
                  .OrderBy("nu_task_sort")
                  .Bind("id", "1")
                  .ExecuteAsync(); // RowResult or DocResult ?
                // Do something else while the getEmployeesTask is executing in the background

                // at this point we are ready to get our results back. If it is not done, this will block until done
                RowResult response = await getChecklistTask;
                PostChecklists(response);
                /*  pcliDB.resultsText = mainAreaText;
                    checklist.Clear();
                    pcliDB.postExecution = PostChecklists;
                    pcliDB.ExecuteQuery("T_Checklists", null, "nu_process", "1"); //("T_Processes", "st_name, ss_target_roles"); "T_Checklists", "st_task, st_creation_by"
                */
            }
        }

        void PostChecklists(RowResult response)
        {
            if (checklist.Count == 0) {
                panelCount = 0;
                foreach (var row in response.FetchAll())
                {
                    checklist.Add(new String[] { row["nu_task_sort"].ToString(), row["st_task"].ToString() });

                    GameObject tmpPrefab = Instantiate(taskPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    tmpPrefab.transform.parent = homeContent.transform;
                    tmpPrefab.transform.localPosition = new Vector3(0, panelCount * -37, 0);

                    foreach (var textField in tmpPrefab.GetComponentsInChildren<Text>())
                    {
                        if (textField.name == "TaskText") textField.text = row["st_task"].ToString();
                        else textField.text = row["nu_task_sort"].ToString();
                    }
                    panelCount++;
                }
            }
        }

    }

    /* ----------------------------------------------------------------------------------------------------
            Con DynamoDB teníamos la definición "Task" con la estructura de la tabla, y podíamos crear una lista con rows de ese tipo documento (con pares kv). Era rebuscado para armar la lista pero funcionaba.
            Hay que ver en MySQL qué tipos para guardar estructuras de Documentos tenemos.

            // private DynamoDBLib pcliDB = new DynamoDBLib();
            // private List<Task> checklist = new List<Task>();

                void PostChecklists(QueryResponse response)
                {
                    if (checklist.Count == 0) {
                        panelCount = 0;
                        foreach (var item in response.Items) {
                            Task task = new Task();
                            foreach (var kvp in item) {
                                switch (kvp.Key) {
                                    case "nu_process": task.nu_process = Int32.Parse(kvp.Value.N.ToString()); break;
                                    case "nu_task_sort": task.nu_task_sort = Int32.Parse(kvp.Value.N.ToString()); break;
                                    case "st_task": task.st_task = kvp.Value.S.ToString(); break;
                                    case "st_creation_by": task.st_creation_by = kvp.Value.S.ToString(); break;
                                    case "st_creation_date": task.st_creation_date = kvp.Value.S.ToString(); break;
                                }
                            }
                            checklist.Add(task);
                        }
                    }
                    foreach (var task in checklist) {
                        GameObject tmpPrefab = Instantiate(taskPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        tmpPrefab.transform.parent = GameObject.Find("HomeText").transform;
                        tmpPrefab.transform.localPosition = new Vector3(0, panelCount * -37, 0);
                        foreach (var textField in tmpPrefab.GetComponentsInChildren<Text>()) {
                            if (textField.name == "TaskText") textField.text = task.st_task;
                            else textField.text = task.nu_task_sort.ToString();
                        }
                        panelCount++;
                    }
                }
        */
}