using System.Collections.Generic;
using UnityEngine;

public static class WeightedPokemonVariationSelector
{
    public static PokemonVariation GetRandomItem(List<PokemonVariation> variations)
    {
        float totalWeight = 0f;
        
        foreach (var v in variations)
        {
            totalWeight += v.variationProbability;
        }
        
        float randomWeight = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var weightedItem in variations)
        {
            cumulativeWeight += weightedItem.variationProbability;
            if (randomWeight <= cumulativeWeight)
            {
                return weightedItem;
            }
        }

        return null;
    }
}

public static class WeightedPokemonSpawnSelector
{
    public static Pokemon_SO GetRandomItem(List<WeightedPokemonSpawn> variations)
    {
        float totalWeight = 0f;
        
        foreach (var v in variations)
        {
            totalWeight += v.weight;
        }
        
        float randomWeight = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var weightedItem in variations)
        {
            cumulativeWeight += weightedItem.weight;
            if (randomWeight <= cumulativeWeight)
            {
                return weightedItem.pokémon;
            }
        }

        return null;
    }
}