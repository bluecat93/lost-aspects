using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Finals
{
    #region Tags
    public const string PLAYER = "Player";
    public const string ENEMY = "Enemy";
    public const string EQUIPABLE = "Equipable";
    #endregion

    #region Animations
    public const string ATTACK_ENEMY = "Attacking";
    public const string ATTACK_PLAYER = "Attack";
    public const string STATE = "State";
    public const string ATTACK_RANGE_BOOLEAN = "InAttackRange";
    public const string ATTACK_NUMBER_ENEMY = "AttackNumber";
    public const string ATTACK_NUMBER_PLAYER = "Attack Number";
    public const string VELOCITY_X = "VelocityX";
    public const string VELOCITY_Z = "VelocityZ";
    public const string IS_GROUNDED = "isGrounded";
    public const string IS_JUMPING = "isJumping";
    public const string IS_FALLING = "isFalling";
    public const string CROUCHING = "Crouching";
    public const string ROLL = "Roll";
    #endregion

    #region Scenes
    public const string MULTIPLAYER_LOBBY = "Lobby";
    public const string MAIN_MENU = "MainMenu";
    #endregion

    #region Game Objects
    public const string LOCAL_GAME_PLAYER = "LocalGamePlayer";
    public const string ITEM_INDEX = "ItemIndex";

    #endregion

    #region Controls
    public const string HORIZONTAL_MOVEMENT = "Horizontal";
    public const string VERTICAL_MOVEMENT = "Vertical";
    public const string ATTACK = "Swing";
    public const string EQUIP = "Equip";
    public const string CAMERA_UNLOCKED = "Camera Unlocked";
    public const string CROUCH = "Crouch";
    public const string DODGE = "Dodge";
    public const string USE = "Use";
    public const string OPEN_MENU = "Open Menu";
    public const string INVENTORY = "Inventory";
    public const string USE_ITEM = "Use Item From Inventory";
    #endregion

    #region Inventory
    public const float ITEM_PICKUP_TIME = 0.01f;
    #endregion
}
