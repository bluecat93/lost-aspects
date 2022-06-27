using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;


namespace Crafting
{
    [System.Serializable]
    public class Recipe
    {
        public List<InventoryManager.ItemInsideInventory> Ingridients;
        public InventoryManager.ItemInsideInventory CraftedItem;
    }
    public class KnownRecipes : MonoBehaviour
    {
        #region Variables
        [Tooltip("The starting recipes the player knows")]
        public List<Recipe> StartingRecipes;
        [HideInInspector] public List<Recipe> KnownRecipesList;
        public GameObject RecipePrefab;
        public GameObject RecipeContentView;
        public Vector3 ScaleOfRecipe;
        public Vector3 RecipePosition;
        private List<GameObject> RecipiesObjectList;
        #endregion

        void Start()
        {
            RecipiesObjectList = new List<GameObject>();

            if (StartingRecipes == null)
            {
                KnownRecipesList = new List<Recipe>();
            }
            else
            {
                KnownRecipesList = new List<Recipe>();
                AddRecipies(StartingRecipes);
            }
        }
        public void AddRecipe(Recipe recipe)
        {
            KnownRecipesList.Add(recipe);
        }

        public void AddRecipies(List<Recipe> recipes)
        {
            foreach (Recipe recipe in recipes)
            {
                AddRecipe(recipe);
            }
        }

        public bool RemoveRecipe(Recipe recipe)
        {
            return KnownRecipesList.Remove(recipe);
        }
        public void RemoveAllRecipes()
        {
            KnownRecipesList = new List<Recipe>();
        }

        public void UpdateRecipes()
        {
            // remove the crafting recipies first to avoid duplicates
            foreach (GameObject obj in RecipiesObjectList)
            {
                Destroy(obj);
            }

            foreach (Recipe recipe in KnownRecipesList)
            {
                GameObject obj = Instantiate(RecipePrefab) as GameObject;

                obj.transform.SetParent(RecipeContentView.transform);
                obj.GetComponent<CraftingListItem>().SetData(recipe);

                obj.transform.localScale = ScaleOfRecipe;
                obj.transform.localPosition = Vector3.zero + RecipePosition;

                RecipiesObjectList.Add(obj);
            }
        }
    }
}