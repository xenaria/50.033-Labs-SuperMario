
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterCreator : MonoBehaviour
{

    public GameObject characterPrefab;
    public Vector3[] location;

    private List<GameObject> characterList = new List<GameObject>();


    public void SpawnCharacter()
    {
        characterList.Add(Instantiate(characterPrefab, location[Random.Range(0, location.Length)], Quaternion.identity));


    }

    public void KillOneCharacter()
    {
        if (characterList.Count > 0)
        {
            GameObject last = characterList[characterList.Count - 1];
            characterList.RemoveAt(characterList.Count - 1);
            Destroy(last);

        }
    }
}
