using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Crafting
{
    public class KnownRecipes : MonoBehaviour
    {
        #region Variables
        [Tooltip("The starting recipes the player knows")]
        public List<Recipe> StartingRecipes;
        [HideInInspector] public List<Recipe> KnownRecipesList;
        public GameObject RecipePrefab;
        public GameObject RecipeContentView;
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
                GameObject obj = Instantiate(RecipePrefab);

                obj.GetComponent<CraftingListItem>().SetData(recipe);
                RecipiesObjectList.Add(obj);
            }
        }



    }
}