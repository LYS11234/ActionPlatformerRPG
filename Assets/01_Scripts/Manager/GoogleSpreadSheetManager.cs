using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class GoogleSpreadSheetManager : MonoBehaviour
{
    // GameManager에 파싱해두기에 굳이 싱글톤 사용할 필요 없음
    [Tooltip("구글 스프레드 시트 URL 모음")]
    [SerializeField]
    private string[] googleSheetURL;
    [Tooltip("사용하지 않을 시트를 지정합니다. 공백없이, /로 구분합니다. 예: Sheet1/Sheet2")]
    [SerializeField]
    private string unavailableSheets;
    [Tooltip("외부에서 접근할 수 있는 Scriptable Object입니다.")]
    private string[] unavailableSheetArray;

    [field:SerializeField]
    private Dictionary<string, ScriptableObject> cachedSOs = new Dictionary<string, ScriptableObject>();

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
        unavailableSheetArray = unavailableSheets.Split('/');
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
            AddListOnList(_tableName, _dataJson);
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
                Debug.LogError($"Error: {e.HelpLink}\n{e.Message}\n{e.Source}");
                return null;
            }
        }
    }

    private bool IsExistUnavailableSheets(string sheetName)
    {
        return Array.Exists(unavailableSheetArray, x => x == sheetName);
    }

    public void AddListOnList(string _typeName, string _data)
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
                if(field.FieldType.GetGenericTypeDefinition() != typeof(List<>))
                {
                    continue;
                }
                Type _innerType = field.FieldType.GetGenericArguments()[0];
                if (!_innerType.IsGenericType)
                {
                    continue;
                }
                if(_innerType.GetGenericTypeDefinition() != typeof(List<>))
                {
                    continue;
                }
                _targetField = field;
                break;
            }
            if (_targetField == null)
            {
                Debug.LogError($"{_soClassName} doesn't have List<List<{_typeName}>>");
                return;
            }
            IList _mainList = (IList)Activator.CreateInstance(_targetField.FieldType);

            foreach(var property in _dataObject.Properties())
            {
                string _tabName = property.Name;
                if(IsExistUnavailableSheets(_tabName))
                {
                    continue;
                }
                JToken _tabData = property.Value;

                Type _innerListType = typeof(List<>).MakeGenericType(_dataType);
                object _parsedList = _tabData.ToObject(_innerListType);

                _mainList.Add(_parsedList);
            }

            _targetField.SetValue(_soInstance, _mainList);

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
            Debug.LogError($"Error: {e.Message}");
        }
    }
}
