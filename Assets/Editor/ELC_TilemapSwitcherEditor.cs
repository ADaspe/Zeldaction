using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ELC_TilemapSwitch))] //On fait un editor pour rajouter des �l�ments sur l'inspector d'un script : ici le script c'est ELC_TilemapSwitch
public class ELC_TilemapSwitcherEditor : Editor //H�rite pas de monobehaviour : ce script ne peut pas �tre plac� sur un objet, il va run en dehors de la scene
{

    public override void OnInspectorGUI() //Pour modifier les �l�ments affich�s sur l'inspector
    {
        base.OnInspectorGUI(); //C'est le code de base de l'inspecteur de base de Unity, si on veut faire un inspector de 0 (vide) il faut l'enlever mais l� on va juste rajouter des �l�ments par dessus au lieu de tout refaire

        ELC_TilemapSwitch TLSwitcher = (ELC_TilemapSwitch)target; //Pour cr�er des r�f�rences on utilise target qui correspond � l'objet sur lequel on est actuellement (ici ce sera tout le temps un objet o� y'a un ELC_TilemapSwitch dans tous les cas logiquement)

        if (GUILayout.Button("Switch Tilemaps")) //GUILayout permet d'avoir acc�s � tous les diff�rents �l�ments qu'on peut rajouter dans l'inspector (ici donc un bouton)
        {
            //Ce que fait la fonction quand on appuie sur le bouton
            TLSwitcher.EngageSwitchProcess();
            Debug.Log("Switch de tilemap !"); 
        }

        if(GUILayout.Button("Revert"))
        {
            TLSwitcher.RevertSwitch();
            Debug.Log("revert");
        }
    }

}
