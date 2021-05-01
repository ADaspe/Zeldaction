using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_GameManager : MonoBehaviour
{
    public ELC_CharacterManager CharacterManager;

    public LayerMask PlayerMask; //Si on utilise le même layer pour les 2 personnages
    public LayerMask SpiritMask; // Layer de l'esprit
    public LayerMask MiaMask; // Layer de Mia
    public LayerMask EnemiesMask; //Les ennemis
    public LayerMask GlobalObstaclesMask; //Les obstacles qu'aucun des 2 personnages ne peut traverser
    public LayerMask SolidObstaclesMask; //Les obstacles que Mia ne peut traverser
    public LayerMask SpiritualObstaclesMask; //Les obstacles que l'esprit peut traverser
    public LayerMask InteractionMask; //Les éléments avce lesquels le joueur pourra interagir
    public LayerMask ExplodingMushroomMask; // éléments qui seront affectés par l'explosion du champi
}
