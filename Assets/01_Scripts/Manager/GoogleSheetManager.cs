using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GoogleSheetManager : MonoBehaviour
{

    [Tooltip("true면 구글 스프레드 시트를, false면 로컬 json 파일을 사용합니다.")]
    [SerializeField]
    private bool isAccessGoogleSheet;
    [Tooltip("구글 스프레드 시트 URL")]
    [SerializeField]
    private string googleSheetURL;
    [Tooltip("사용할 시트를 지정합니다. 공백없이, /로 구분합니다. 예: Sheet1/Sheet2")]
    [SerializeField]
    private string availableSheets;
    [Tooltip("파일 생성 경로를 지정합니다.")]
    [SerializeField]
    private string generateFolderPath;
    [Tooltip("외부에서 접근할 수 있는 Scriptable Object입니다.")]
    public ScriptableObject GoogleSheetSO { get; private set; }

    private string jsonPath => $"{Application.dataPath}{generateFolderPath}/GoogleSheetJson.json";
    private string classPath => $"{Application.dataPath}{generateFolderPath}/GoogleSheetClass.cs";
    private string soPath => $"Assets{generateFolderPath}/GoogleSheetSO.asset";

    private string[] availableSheetArray;
    private string json;
    private bool refreshTrigger;

    private static GoogleSheetManager _instance;
    public GoogleSheetManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
           _instance = this;
            return;
        }
        Destroy(this.gameObject);
    }


    public static T SO<T>() where T : ScriptableObject
    {
        if (_instance.GoogleSheetSO == null)
        {
            Debug.LogError($"GoogleSheetSO is Null");
            return null;
        }
        return _instance.GoogleSheetSO as T;
    }

#if UNITY_EDITOR

    [ContextMenu("FetchGoogleSheet")]
    async void FetchGoogleSheet()
    {
        availableSheetArray = availableSheets.Split('/');

        if (isAccessGoogleSheet)
        {
            Debug.Log("Loading from Google Spread Sheet");
            json = await LoadDataGoogleSheet(googleSheetURL);
            
        }
        else
        {
            Debug.Log($"Loading from local json");
            json = LoadDataLocalJson();
        }
        if(json == null)
        {
            return;
        }
        bool isJsonSaved = SaveFileOrSkip(jsonPath, json);
        string allClassCode = GenerateCSharpClass(json);
        bool isClassSaved = SaveFileOrSkip(classPath, allClassCode);

        if(isJsonSaved || isClassSaved)
        {
            refreshTrigger = true;
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            CreateGoogleSheetSO();
            Debug.Log("Fetch done");
        }
    }

    async Task<string> LoadDataGoogleSheet(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] byteDatas = await client.GetByteArrayAsync(url);
                return Encoding.UTF8.GetString(byteDatas);
            }
            catch (HttpRequestException e)
            {
                {
                    Debug.LogError($"Error: {e.HelpLink}\n{e.Message}\n{e.Source}");
                    return null;
                }
            }
        }
    }

    private string LoadDataLocalJson()
    {
        if (File.Exists(jsonPath))
        {
            return File.ReadAllText(jsonPath);
        }

        Debug.Log($"File not exist. \n{jsonPath}");
        return null;
    }

    private bool SaveFileOrSkip(string path, string content)
    {
        string directoryPath = Path.GetDirectoryName(path);
        if (Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        if (File.Exists(path) && File.ReadAllText(path).Equals(content))
        {
            return false;
        }
        File.WriteAllText(path, content);
        return true;

    }

    private bool IsExistAvailableSheets(string sheetName)
    {
        return Array.Exists(availableSheetArray, x => x == sheetName);
    }

    private string GenerateCSharpClass(string jsonInput)
    {
        JObject jsonObject = JObject.Parse(jsonInput);
        StringBuilder classCode = new();

        classCode.AppendLine("using System;\nusing System.Collections.Generic;\nusing UnityEngine;\n");
        classCode.AppendLine("public class GoogleSheetSO : ScriptableObject\n{");
        foreach (var sheet in jsonObject)
        {
            string className = sheet.Key;
            if (!IsExistAvailableSheets(className))
            {
                continue;
            }

            classCode.AppendLine($"\tpublic List<{className}> {className}List;");
        }
        classCode.AppendLine("}\n");

        foreach (var jObject in jsonObject)
        {
            string className = jObject.Key;
            if (!IsExistAvailableSheets(className))
            {
                continue;
            }

            var items = (JArray)jObject.Value;
            var firstItem = (JObject)items[0];
            classCode.AppendLine($"[Serializable]\npublic class {className}\n{{");

            int itemIndex = 0;
            int propertuCount = ((JObject)items[0]).Properties().Count();
            string[] propertyTypes = new string[propertuCount];

            foreach (JToken item in items)
            {
                itemIndex = 0;
                foreach (var property in ((JObject)item).Properties())
                {
                    string propertyType = GetCSharpType(property.Value.Type);
                    string oldPropertyType = propertyTypes[itemIndex];

                    switch (oldPropertyType)
                    {
                        case null:
                            {
                                propertyTypes[itemIndex] = propertyType;
                                break;
                            }
                        case "int":
                            {
                                if (propertyType == "bool")
                                {
                                    propertyType = "string";
                                    break;
                                }
                                propertyTypes[itemIndex] = propertyType;
                                break;
                            }
                        case "float":
                            {
                                if (propertyType == "int" || propertyType == "float")
                                {
                                    propertyTypes[itemIndex] = "float";
                                    break;
                                }
                                propertyTypes[itemIndex] = "string";
                                break;
                            }
                        case "bool":
                            {
                                if (propertyType != "bool")
                                {
                                    propertyTypes[itemIndex] = "string";
                                    break;
                                }
                                propertyTypes[itemIndex] = "bool";
                                break;
                            }
                            
                    }
                    itemIndex++;
                }
            }
            itemIndex = 0;
            foreach(var property in firstItem.Properties())
            {
                string propertyName = property.Name;
                string propertyType = propertyTypes[itemIndex++];
                classCode.AppendLine($"\tpublic {propertyType} {propertyName};");
            }

            classCode.AppendLine("}\n");
        }
        return classCode.ToString();
    }
   

    private string GetCSharpType(JTokenType jsonType)
    {
        switch (jsonType)
        {
            case JTokenType.Integer:
                {
                    return "int";
                }
            case JTokenType.Float:
                {
                    return "float";
                }
            case JTokenType.Boolean:
                {
                    return "bool";
                }
            default:
                {
                    return "string";
                }
        }
    }

    private bool CreateGoogleSheetSO()
    {
        if (Type.GetType("GoogleSheetSO") == null)
        {
            return false;
        }

        GoogleSheetSO = ScriptableObject.CreateInstance("GoogleSheetSO");

        JObject jsonObject = JObject.Parse(json);
        try
        {
            foreach (var jObject in jsonObject)
            {
                string className = jObject.Key;
                if (!IsExistAvailableSheets(className))
                {
                    continue;
                }

                Type classType = Type.GetType(className);
                Type listType = typeof(List<>).MakeGenericType(classType);
                IList listInst = (IList)Activator.CreateInstance(listType);
                var items = (JArray)jObject.Value;

                foreach (var item in items)
                {
                    object classInst = Activator.CreateInstance(classType);

                    foreach (var property in ((JObject)item).Properties())
                    {
                        FieldInfo fieldInfo = classType.GetField(property.Name);
                        object value = Convert.ChangeType(property.Value.ToString(), fieldInfo.FieldType);
                        fieldInfo.SetValue(classInst, value);
                    }
                    listInst.Add(classInst);
                }
                GoogleSheetSO.GetType().GetField($"{className}List").SetValue(GoogleSheetSO, listInst);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"CreateGoogleSheetSO Error: {ex.Message}");
        }
        UnityEditor.AssetDatabase.CreateAsset(GoogleSheetSO, soPath);
        UnityEditor.AssetDatabase.SaveAssets();
        return true;
    }

    private void OnValidate()
    {
        if(refreshTrigger)
        {
            bool isComplited = CreateGoogleSheetSO();
            if(!isComplited)
            {
                return;
            }
            refreshTrigger = false;
            Debug.Log("Fetch done");
        }
    }

#endif
}

