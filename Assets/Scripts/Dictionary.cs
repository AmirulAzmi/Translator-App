using UnityEngine;

[System.Serializable]
public class Word
{
    //public string rt, ms, en, type;
    public string ms, en;
}

[System.Serializable]
public class Dictionary{
    public Word[] words;
}
