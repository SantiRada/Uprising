using System.Collections.Generic;
using UnityEngine;

public class PlanetarySystem : MonoBehaviour {

    [Header("Minerals")]
    public int[] countMineral = { 4, 3, 2, 2, 1 };
    [Space]
    public Mineral[] commonMineral;         // 4
    public Mineral[] uncommonMineral;       // 3
    public Mineral[] rareMineral;           // 2
    public Mineral[] epicMineral;           // 2
    public Mineral[] legendaryMineral;      // 1
    [Space]
    public List<Mineral> dataMinerals = new List<Mineral>();

    [Header("Planets")]
    public int countPlanets = 4;
    [Range(0, 100)] public int chancePrevToReceiveMineral = 80;
    [Space]
    public PlanetScriptable[] commonPlanets;
    public PlanetScriptable[] uncommonPlanets;
    public PlanetScriptable[] rarePlanets;
    public PlanetScriptable[] epicPlanets;
    public PlanetScriptable[] legendaryPlanets;
    [Space]
    public List<PlanetScriptable> dataPlanets = new List<PlanetScriptable>();
    private LoadMapFactions _mapFactions;

    private void Awake()
    {
        dataMinerals = CreateMinerals();
    }
    private void Start()
    {
        dataPlanets = CreatePlanets();

        for (int i = 0; i < dataPlanets.Count; i++)
        {
            if (dataPlanets[i].receiveMineral == null)
            {
                dataPlanets[i].receiveMineral = FindMineralWithRarityOutsideOtherPlanets(Rareza.Comun, dataPlanets);
            }

            if (dataPlanets[i].givesMineral == null)
            {
                dataPlanets[i].givesMineral = FindMineralWithRarityOutsideOtherPlanets(Rareza.Comun, dataPlanets);
            }
        }

        _mapFactions = FindAnyObjectByType<LoadMapFactions>();

        _mapFactions._dataMinerals = dataMinerals;
        _mapFactions._dataPlanets = dataPlanets;
    }
    ////////////////////////////////////////////////
    private List<Mineral> CreateMinerals()
    {
        List<Mineral> mineralToCreate = new List<Mineral>();

        Mineral[] listToReview;

        for (int i = 0; i < countMineral.Length; i++)
        {
            for (int j = 0; j < countMineral[i]; j++)
            {
                switch (i)
                {
                    case 0: listToReview = commonMineral; break;
                    case 1: listToReview = uncommonMineral; break;
                    case 2: listToReview = rareMineral; break;
                    case 3: listToReview = epicMineral; break;
                    case 4: listToReview = legendaryMineral; break;
                    default: listToReview = commonMineral; break;
                }

                int rnd = 0;
                int iter = 0;
                do
                {
                    rnd = Random.Range(0, listToReview.Length);
                    iter++;

                } while (mineralToCreate.Contains(listToReview[rnd]) && iter < 20);

                if (iter>= 20) Debug.LogWarning("Falló el WHILE en la creación de los Minerales");

                mineralToCreate.Add(listToReview[rnd]);
            }
        }

        return mineralToCreate;
    }
    ////////////////////////////////////////////////
    private int[] CalculateCountBetween(Rareza rarely)
    {
        int[] count = { 0, 0 };
        int min = 0;
        int max = 0;

        if (rarely == Rareza.Comun)
        {
            min = -1;
            count[1] = countMineral[0];
        }
        else
        {
            int rarelyNumber = (int)rarely;

            min = (rarelyNumber - 1);   // 0
            max = rarelyNumber;         // 1
        }

        if (min == -1) count[0] = 0;
        else if(min == 0) count[0] = countMineral[0];
        else if(min > 0)
        {
            for(int i = 0; i < min; i++)
            {
                count[0] += countMineral[i];
            }
        }

        if (rarely != Rareza.Comun)
        {
            for (int i = 0; i <= max; i++)
            {
                count[1] += countMineral[i];
            }
        }

        return count;
    }
    private Mineral FindMineralPreviouslyCreated(Rareza rarely)
    {
        int[] count = CalculateCountBetween(rarely);

        // Debug.LogWarning("Desde " + count[0] + " hasta " + count[1]);

        // Crear listado de minerales exclusivamente de la rareza dada
        List<Mineral> mineralPreview = new List<Mineral>();
        for(int i = 0; i < (count[1] - count[0]); i++)
        {
            mineralPreview.Add(dataMinerals[(count[0] + i)]);
        }

        return mineralPreview[(Random.Range(0, mineralPreview.Count))];

    }
    private Mineral FindMineralWithRarityOutsideOtherPlanets(Rareza rarely, List<PlanetScriptable> localPlanets)
    {
        List<Mineral> mineralPreview = new List<Mineral>();
        for (int i = 0; i < localPlanets.Count; i++)
        {
            mineralPreview.Add(localPlanets[i].receiveMineral);
            mineralPreview.Add(localPlanets[i].givesMineral);
        }

        List<Mineral> copyDataMineral = new List<Mineral>();

        int[] count = CalculateCountBetween(rarely);

        for (int i = 0; i < (count[1] - count[0]); i++)
        {
            copyDataMineral.Add(dataMinerals[(count[0] + i)]);
        }

        for (int i = 0; i < mineralPreview.Count; i++)
        {
            if (copyDataMineral.Contains(mineralPreview[i])) copyDataMineral.Remove(mineralPreview[i]);
        }

        if (copyDataMineral.Count <= 0) return null;

        int rnd = Random.Range(0, copyDataMineral.Count);

        return copyDataMineral[rnd];
    }
    public Rareza DeterminateRarelyGiveMineral(int i)
    {
        // Definir probabilidades base 
        Dictionary<Rareza, float> probabilidades = new Dictionary<Rareza, float>
        {
            { Rareza.PocoComun, 0.60f },   // Base: 60%
            { Rareza.Raro, 0.25f },        // Base: 25%
            { Rareza.Epico, 0.10f },       // Base: 10%
            { Rareza.Legendario, 0.05f }   // Base: 5%
        };

        // Ajustar probabilidades según i (más i, más chance de rarezas altas)
        float factor = (float)i / countPlanets; // Valor entre 0.4 y 1

        probabilidades[Rareza.PocoComun] = Mathf.Lerp(0.80f, 0.30f, factor);
        probabilidades[Rareza.Raro] = Mathf.Lerp(0.15f, 0.40f, factor);
        probabilidades[Rareza.Epico] = Mathf.Lerp(0.04f, 0.20f, factor);
        probabilidades[Rareza.Legendario] = Mathf.Lerp(0.01f, 0.10f, factor);

        // Seleccionar rareza basado en probabilidades
        float randomValue = Random.value;
        float acumulador = 0f;

        foreach (var par in probabilidades)
        {
            acumulador += par.Value;
            if (randomValue <= acumulador)
                return par.Key;
        }

        return Rareza.PocoComun; // Fallback (no debería llegar aquí)
    }
    ////////////////////////////////////////////////
    private List<PlanetScriptable> CreatePlanets()
    {
        List<PlanetScriptable> planets = new List<PlanetScriptable>();

        // Se crea el primer planeta por fuera del bucle para asegurar que siempre haya un planeta que:
        // Sea Común
        // Reciba mineral Común
        // Dé un mineral Poco Comun
        #region CreateFirstPlanet
        // Crear el planeta segun el mineral aleatorio
        PlanetScriptable newPlanet = commonPlanets[Random.Range(0, commonPlanets.Length)];

        // Selecciona un mineral COMUN aleatorio entre los previamente creados
        Mineral recivedMineral = FindMineralPreviouslyCreated(Rareza.Comun);
        // Settear que mineral RECIBE según el primer valor randomizado (Común)
        newPlanet.receiveMineral = recivedMineral;

        // Selecciona un mineral COMUN aleatorio entre los previamente creados
        Mineral givedMineral = FindMineralPreviouslyCreated(Rareza.PocoComun);
        // Settear que mineral DA según un segundo valor randomizado (Poco Común)
        newPlanet.givesMineral = givedMineral;

        planets.Add(newPlanet);
        #endregion

        List<PlanetScriptable> planetsToReview = new List<PlanetScriptable>();

        for(int i = 0; i < (countPlanets - 1); i++)
        {
            planetsToReview.Clear();

            switch (Random.Range(0, 5))
            {
                case 0: planetsToReview.AddRange(commonPlanets); break;
                case 1: planetsToReview.AddRange(uncommonPlanets); break;
                case 2: planetsToReview.AddRange(rarePlanets); break;
                case 3: planetsToReview.AddRange(epicPlanets); break;
                case 4: planetsToReview.AddRange(legendaryPlanets); break;
                default: planetsToReview.AddRange(commonPlanets); break;
            }

            for(int j = 0; j < planets.Count; j++)
            {
                if (planetsToReview.Contains(planets[j]))
                {
                    planetsToReview.Remove(planets[j]);
                }
            }

            // Crear el planeta segun el listado randomizado
            PlanetScriptable otherPlanet = planetsToReview[Random.Range(0, planetsToReview.Count)];

            #region Calcular el Mineral a Recibir

            int probabilityToReceivedMineral = Random.Range(0, 100);

            // Si se trata del planeta 2 o se dió el nivel de probabilidad para que se repita el mineral entonces se accede
            if (i == 0 || probabilityToReceivedMineral < chancePrevToReceiveMineral)
            {
                // Settear que mineral RECIBE según el mineral dado por el planeta anterior
                otherPlanet.receiveMineral = planets[planets.Count - 1].givesMineral;
            }
            else
            {
                int rnd = Random.Range(2, 5);

                Rareza rarely = Rareza.Raro;

                switch (rnd)
                {
                    case 2: rarely = Rareza.Raro; break;
                    case 3: rarely = Rareza.Epico; break;
                    case 4: rarely = Rareza.Legendario; break;
                }

                Mineral receiveMineralToNewPlanet = FindMineralWithRarityOutsideOtherPlanets(rarely, planets);

                if(receiveMineralToNewPlanet == null)
                {
                    receiveMineralToNewPlanet = FindMineralWithRarityOutsideOtherPlanets(Rareza.PocoComun, planets);
                }

                otherPlanet.receiveMineral = receiveMineralToNewPlanet;
            }
            #endregion

            Mineral giveMineral = null;
            Rareza rarity = Rareza.Comun;
            int iteration = 0;
            do
            {
                rarity = DeterminateRarelyGiveMineral(i);
                giveMineral = FindMineralWithRarityOutsideOtherPlanets(rarity, planets);

                if (giveMineral == null) giveMineral = FindMineralWithRarityOutsideOtherPlanets(Rareza.PocoComun, planets);

                iteration++;
            } while (giveMineral == otherPlanet.receiveMineral && iteration < 20);

            if (iteration >= 20) Debug.LogWarning("Falló el WHILE en la creación de GiveMineral");

            otherPlanet.givesMineral = giveMineral;

            planets.Add(otherPlanet);
        }

        return planets;
    }
    private void ShowPlanets()
    {
        for (int i = 0; i < dataPlanets.Count; i++)
        {
            Debug.Log("Planeta Número " + (i + 1) + ": " + LanguageSystem.GetValue("rogue", dataPlanets[i].namePlanet));
            Debug.Log("Estructura: " + dataPlanets[i].rarePlanet);
            Debug.Log("Recibe: " + dataPlanets[i].receiveMineral.name + " (" + dataPlanets[i].receiveMineral.rareza + ")");
            Debug.Log("Da: " + dataPlanets[i].givesMineral.name + " (" + dataPlanets[i].givesMineral.rareza + ")");
            if(i < (dataPlanets.Count - 1)) Debug.Log("------------------------------");
        }
    }
}