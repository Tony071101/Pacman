using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
[System.Serializable]
public static class SaveSystem
{
    public static void SavePacman(GameManager pacman){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/pacman.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        PacmanDataClass data = new PacmanDataClass(pacman);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PacmanDataClass LoadPacman(){
        string path = Application.persistentDataPath + "/pacman.dat";
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PacmanDataClass data = formatter.Deserialize(stream) as PacmanDataClass;
            stream.Close();
            return data;
        }else{
            Debug.LogError("Save file not found at " + path);
            return null;
        }
    }
}
