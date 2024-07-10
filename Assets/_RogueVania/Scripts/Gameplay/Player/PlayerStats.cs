using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieGames.Utilities;

namespace TrixieGames.Gameplay.Player
{
    public class PlayerStats
    {
        public bool m_isAlive;

        public float m_speed = 5.0f; //effects movement speed
        public float m_strength = 5.0f; //effects attack strength
        public float m_agility = 5.0f; //effects attack speed
        public float m_defense = 5.0f; //effects hit penalty
        public float m_vigor = 5.0f; //effects endurance use penalty

        public int m_health = 3;
        public int m_endurance = 5;

        public STATUS_EFFECTS m_status;
        public PLAYER_JOB m_job;
    }
}
