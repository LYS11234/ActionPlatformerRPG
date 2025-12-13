using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;

public class GoogleSpreadSheetManager : MonoBehaviour
{
    // GameManager에 파싱해두기에 굳이 싱글톤 사용할 필요 없음
    [Tooltip("구글 스프레드 시트 App Script URL 모음")]
    [SerializeField]
    private string[] googleSheetURL;
    [Tooltip("사용하지 않을 시트를 지정합니다. 공백없이, /로 구분합니다. 예: Sheet1/Sheet2")]
    [SerializeField]
    private string unavailableSheets;
    private string path => $"{Application.dataPath}/01_Scripts/Util/";
    
    private string[] unavailableSheetArray;
    
    [field:SerializeField]
    private Dictionary<string, ScriptableObject> cachedSOs = new(); // 캐시된 Scriptable Object들

    public T GetData<T>(string _tableName) where T : ScriptableObject
    {
        if (cachedSOs.ContainsKey(_tableName))
        {
            return cachedSOs[_tableName] as T;
        }
        Debug.LogError($"Data is unloaded or empty: {_tableName}");
        return null;
    }


    public async void FetchGoogleSheet()
    {
        cachedSOs.Clear();
        if (unavailableSheets.Length > 0)
        {
            unavailableSheetArray = unavailableSheets.Split('/');
        }
        foreach (string url in googleSheetURL)
        {
            if(string.IsNullOrEmpty(url))
            {
                continue;
            }
            await ProcessURL(url);
        }
    }

    private async Task ProcessURL(string url)
    {
        string _json = "";

        _json = await LoadDataGoogleSheet(url);

        if(string.IsNullOrEmpty(_json))
        {
            return;
        }
        try
        {
            JObject _parsedJson = JObject.Parse(_json);

            if (!_parsedJson.ContainsKey("fileName"))
            {
                Debug.LogError($"JSON 포맷 에러\n URL: {url}");
                
                return;
            }

            string _tableName = _parsedJson["fileName"].ToString();
            string _dataJson = _parsedJson["data"].ToString();
            Debug.LogError(_parsedJson.ToString());
            AddSODictionary(_tableName, _dataJson);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
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
                Debug.LogError($"Error:{e.Message}\n{e.StackTrace}");
                return null;
            }
        }
    }

    private bool IsExistUnavailableSheets(string sheetName)
    {
        if(string.IsNullOrEmpty(unavailableSheets))
        {
            return false;
        }
        return Array.Exists(unavailableSheetArray, x => x == sheetName);
    }

    public void AddSODictionary(string _typeName, string _data)
    {
        string _soClassName = $"{_typeName}SO";
        string _dataClassName = $"{_typeName}Data";

        Type _soType = Type.GetType(_soClassName);
        Type _dataType = Type.GetType(_dataClassName);

        if(_soType == null)
        {
            Debug.Log($"Undefined Class: {_soClassName}");
            return;
        }
        if (_dataType == null)
        {
            Debug.LogError($"Undefined Data: {_dataClassName}");
            return;
        }

        ScriptableObject _soInstance = ScriptableObject.CreateInstance(_soType);

        try
        {
            JObject _dataObject = JObject.Parse(_data);

            FieldInfo _targetField = null;
            foreach (var field in _soType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!field.FieldType.IsGenericType)
                {
                    continue;
                }
                if(field.FieldType.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                {
                    continue;
                }
                Type[] _args = field.FieldType.GetGenericArguments();
                if (_args[0] != typeof(string))
                {
                    continue;
                }
                if( _args[1] != _dataType)
                { 
                    continue;
                }
                _targetField = field;
                break;
            }
            if (_targetField == null)
            {
                Debug.LogError($"{_soClassName} doesn't have Dictionary<string, {_dataClassName}>()");
                return;
            }
            IDictionary _mainDict = (IDictionary)Activator.CreateInstance(_targetField.FieldType);

            foreach(var property in _dataObject.Properties())
            {
                string _tabName = property.Name;
                if(IsExistUnavailableSheets(_tabName))
                {
                    continue;
                }
                JToken _tabData = property.Value;

                Type _listType = typeof(List<>).MakeGenericType(_dataType);
                IList _parsedList = (IList)_tabData.ToObject(_listType);
                foreach (var item in _parsedList)
                {
                    FieldInfo _idField = _dataType.GetField("ID");
                    string _key = _tabName;
                    if(_idField != null)
                    {
                        _key += "_" + _idField.GetValue(item) as string;
                    }
                    if(_mainDict.Contains(_key))
                    {
                        continue;
                    }
                    _mainDict.Add(_key, item);
                }
            }

            _targetField.SetValue(_soInstance, _mainDict);

            if (cachedSOs.ContainsKey(_typeName))
            {
                cachedSOs[_typeName] = _soInstance;
            }
            else
            {
                cachedSOs.Add(_typeName, _soInstance);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}\n{e.StackTrace}");
        }
    }

#if UNITY_EDITOR
    private StringBuilder classCode;
    [ContextMenu("Create Scriptable Object and Data Classes")]
    private async void CreateClasses() //Google Spread Sheet에서 모든 url 순환 수행
    {
        string _json = "";
        for (int i = 0; i < googleSheetURL.Length; i++)
        {
            _json = await LoadDataGoogleSheet(googleSheetURL[i]);
            CreateScriptableClass(_json);
        }
        UnityEditor.AssetDatabase.Refresh();
    }


    private void CreateScriptableClass(string _json) //클래스 자동 생성
    {
        JObject _jsonObject = JObject.Parse(_json);
        if (!_jsonObject.ContainsKey("fileName"))
        {
            Debug.LogError($"JSON Format Error");

            return;
        }
        string _tableName = _jsonObject["fileName"].ToString();
        if (Type.GetType($"{_tableName}Data") != null)
        {
            return;
        }
        classCode = new();
        classCode.AppendLine("using System;\nusing System.Collections.Generic;\nusing UnityEngine;");
        classCode.AppendLine($"[System.Serializable]\npublic class {_tableName}SO : ScriptableObject");
        classCode.AppendLine("{");
        classCode.AppendLine($"\tpublic Dictionary<string, {_tableName}Data> {_tableName} = new();");
        classCode.AppendLine("}");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllText($"{path}{_tableName}SO.cs", classCode.ToString());
        // Create SO class file

        classCode = new();
        classCode.AppendLine("using System;\nusing System.Collections.Generic;\nusing UnityEngine;");
        classCode.AppendLine($"[System.Serializable]\npublic class {_tableName}Data");
        classCode.AppendLine("{");
        if (_jsonObject["data"] is JObject _dataRoot)
        {
            foreach (var _jObject in _dataRoot)
            {

                string _className = _jObject.Key;
                if (IsExistUnavailableSheets(_className))
                {
                    continue;
                }

                var _items = (JArray)_jObject.Value;
                var _firstItem = (JObject)_items[0];

                int _itemIndex = 0;
                int _propertyCount = _firstItem.Properties().Count();
                string[] _propertyTypes = new string[_propertyCount];

                foreach (JToken _item in _items)
                {
                    _itemIndex = 0;
                    SetVariables((JObject)_item, ref _itemIndex, ref _propertyTypes);
                }
                _itemIndex = 0;
                foreach (var _property in _firstItem.Properties())
                {
                    string _propertyName = _property.Name;
                    string _propertyType = _propertyTypes[_itemIndex++];
                    classCode.AppendLine($"\tpublic {_propertyType} {_propertyName};");
                }
                break;
            }
            
        }
        classCode.AppendLine("}");
        File.WriteAllText($"{path}{_tableName}Data.cs", classCode.ToString());
    }


    private void SetVariables(JObject _item, ref int _itemIndex, ref string[] _propertyTypes)
    {
        foreach (var _property in _item.Properties())
        {
            string _propertyType = GetCSharpType(_property.Value.Type);
            string _oldPropertyType = _propertyTypes[_itemIndex];

            if (_oldPropertyType == null)
            {
                _propertyTypes[_itemIndex] = _propertyType;
            }
            else if (_oldPropertyType == "int")
            {
                switch (_propertyType)
                {
                    case "int":
                        {
                            _propertyTypes[_itemIndex] = "int";
                            break;
                        }
                    case "float":
                        {
                            _propertyTypes[_itemIndex] = "float";
                            break;
                        }
                    case "bool":
                        {
                            _propertyTypes[_itemIndex] = "string";
                            break;
                        }
                    case "string":
                        {
                            _propertyTypes[_itemIndex] = "string";
                            break;
                        }
                }
            }
            else if (_oldPropertyType == "float")
            {
                switch (_propertyType)
                {
                    case "int":
                        {
                            _propertyTypes[_itemIndex] = "float";
                            break;
                        }
                    case "float":
                        {
                            _propertyTypes[_itemIndex] = "float";
                            break;
                        }
                    case "bool":
                        {
                            _propertyTypes[_itemIndex] = "string";
                            break;
                        }
                    case "string":
                        {
                            _propertyTypes[_itemIndex] = "string";
                            break;
                        }
                }
            }
            else if (_oldPropertyType == "bool")
            {
                switch (_propertyType)
                {
                    case "int":
                        {
                            _propertyTypes[_itemIndex] = "string";
                            break;
                        }
                    case "float":
                        {
                            _propertyTypes[_itemIndex] = "string";
                            break;
                        }
                    case "bool":
                        {
                            _propertyTypes[_itemIndex] = "bool";
                            break;
                        }
                    case "string":
                        {
                            _propertyTypes[_itemIndex] = "string";
                            break;
                        }
                }
            }
            _itemIndex++;
        }
    }
    string GetCSharpType(JTokenType _jsonType)
    {
        switch (_jsonType)
        {
            case JTokenType.Integer:
                return "int";
            case JTokenType.Float:
                return "float";
            case JTokenType.Boolean:
                return "bool";
            default:
                return "string";
        }
    }
#endif
}