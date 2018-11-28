//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public enum tlType { EN_MS, MS_EN }

public class Translate : MonoBehaviour
{
    public tlType translation;
    public string enJsonFile;
    public string msJsonFile;
    public bool findExactWord;
    public Text title;
    public Text search;
    public Text result;
    public Text other;
    public Text other2;

    private string dataFilePath;
    private string answer;
    private Dictionary dict;
    private string[] entry1;
    private string[] entry2;

    private char[] delimeter = new char[] { ' ', '-', ':', ';', ',', '.' };

    private bool foundResult = false;
    private bool foundOther = false;

    public bool isDataLoaded = false;   //for speech uses
    public 
    // Use this for initialization
    void Start()
    {
        if (translation == tlType.MS_EN)
        {
            dataFilePath = Application.dataPath + "/StreamingAssets/" + msJsonFile;
            title.text = "Malay-English Translator";
        }
        else if (translation == tlType.EN_MS)
        {
            dataFilePath = Application.dataPath + "/StreamingAssets/" + enJsonFile;
            title.text = "English-Malay Translator";
        }

        LoadDictionary();
    }

    private void LoadDictionary()
    {
        if (!File.Exists(dataFilePath))
        {
            Debug.LogError("Data file not exist!");
        }
        else
        {
            using (StreamReader stream = new StreamReader(dataFilePath))
            {
                string json = stream.ReadToEnd();
                dict = JsonUtility.FromJson<Dictionary>(json);
            }
            Debug.Log("Word entry loaded: " + dict.words.Length);
            //--------------------------------
            entry1 = new string[dict.words.Length];
            entry2 = new string[dict.words.Length];

            for (int i = 0; i < dict.words.Length; i++)
            {
                if (translation == tlType.MS_EN)
                {
                    entry1[i] = dict.words[i].ms;
                    entry2[i] = dict.words[i].en;
                }
                else
                {
                    entry1[i] = dict.words[i].en;
                    entry2[i] = dict.words[i].ms;
                }
            }
        }
        isDataLoaded = true;
    }

    public void SearchWord(string text)
    {
        resetSearch();

        for (int i = 0; i < dict.words.Length; i++)
        {
            Compare(i, text.ToLower(), entry1[i].ToLower());
        }
    }

    void Compare(int index, string input, string database)
    {
        // Find exact same word
        if (input == database)
        {
            DisplayResult(index);
            foundResult = true;
            return;
        }
        else { DisplayResult(-1); }

        // Find word that part of it similar
        if (database.Contains(input) && !findExactWord)
        {
            string[] temp = database.Split(delimeter, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < temp.Length; i++)
            {
                if (input == temp[i])
                {
                    DisplayOther(index, input);
                    foundOther = true;
                    break;
                }
                else if (temp[i].EndsWith(input))
                {
                    DisplayOther(index, input);
                    foundOther = true;
                    break;
                }
                else if (temp[i].StartsWith(input))
                {
                    DisplayOther(index, input);
                    foundOther = true;
                    break;
                }
            }
        }
        else { DisplayOther(-1, input); }
    }

    void DisplayResult(int index)
    {
        if (index >= 0) { result.text = entry2[index]; }
        else if (!foundResult)
        {
            result.text = "no entry!";
        }
    }

    void DisplayOther(int index, string input)
    {
        if (index >= 0)
        {
            string similar = entry1[index].Replace(input, "<b>" + input + "</b>");
            answer += similar + ": <i><color=#B62121C8>" + entry2[index] + "</color></i>\n";
            //answer += similar + ": " + entry2[index] + "\n";
            other.text = answer;

            //force canvas update so we can get correct result from cachedTextGenerator
            Canvas.ForceUpdateCanvases();
            //get truncated index
            int truncateIndex = other.cachedTextGenerator.characterCountVisible;
            //remove text before truncated index
            other2.text = answer.Substring(truncateIndex);

        }
        if (answer == "" && !foundOther)
        {
            other.text = "<color=#B62121C8><b>no entry</b></color>";
        }
    }

    void resetSearch()
    {
        answer = "";
        result.text = "...";
        other.text = "";
        other2.text = "";
        foundResult = false;
        foundOther = false;
    }
}
