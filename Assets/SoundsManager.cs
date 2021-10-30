using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviourSingleton<SoundsManager>
{

    [Header("Sounds Player")]

    [SerializeField] string m_bajar= null;

    [SerializeField] string m_salto = null;

    [SerializeField] string m_colisionplataforma = null;

    [SerializeField] string m_disparo = null;

    [SerializeField] string m_daño = null;

    [SerializeField] string m_muerte = null;

    [Header("Sounds Enemys")]

    [SerializeField] string m_estaticoMuere = null;

    [SerializeField] string m_pistolaMuerte = null;

    [SerializeField] string m_laser = null;

    [SerializeField] string m_pared = null;

    public string Bajar { get => m_bajar; }
    public string Salto { get => m_salto; }
    public string Colisionplataforma { get => m_colisionplataforma; }
    public string Disparo { get => m_disparo; }
    public string Daño { get => m_daño; }
    public string Muerte { get => m_muerte; }
    public string EstaticoMuere { get => m_estaticoMuere; }
    public string PistolaMuerte { get => m_pistolaMuerte; }
    public string Laser { get => m_laser; }
    public string Pared { get => m_pared; }
}
