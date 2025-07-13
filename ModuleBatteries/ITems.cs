using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using Nautilus.Crafting;
using UnityEngine;

namespace ModuleBatteries.Items
{
    public static class ReactorRodItems
    {
        public static TechType MiniRod;
        public static TechType DepletedMiniRod;
        

        public static void Register()
        {
            // 🔋 Mini Reactor Rod
            var miniInfo = PrefabInfo.WithTechType(
                "MiniReactorRod",
                "Mini Reactor Rod",
                "A compact nuclear fuel rod for small devices."
                    ).WithIcon(RamuneLib.Utils.ImageUtils.GetSprite("MiniReactorRod")); // Reuse icon for now

            MiniRod = miniInfo.TechType;

            var miniRod = new CustomPrefab(miniInfo);
            var miniTemplate = new CloneTemplate(miniInfo, TechType.ReactorRod);

            miniRod.SetGameObject(miniTemplate);
            miniRod.SetRecipe(new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new CraftData.Ingredient(TechType.Lead, 1),
                    new CraftData.Ingredient(TechType.UraniniteCrystal, 1),
                    new CraftData.Ingredient(TechType.Glass, 1),
                }
            })
            .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Resources","Electronics");
            
            miniRod.SetUnlock(TechType.ReactorRod); // unlocked with big rod
            miniRod.SetPdaGroupCategory(TechGroup.Resources, TechCategory.AdvancedMaterials);

            miniRod.Register();

            // ☢️ Depleted Mini Reactor Rod
            var depletedInfo = PrefabInfo.WithTechType(
                "DepletedMiniReactorRod",
                "Depleted Mini Rod",
                "A used-up mini nuclear rod. No longer useful for power."
            ).WithIcon(RamuneLib.Utils.ImageUtils.GetSprite("DepletedMRR"));

            DepletedMiniRod = depletedInfo.TechType;

            var depletedRod = new CustomPrefab(depletedInfo);
            var depletedTemplate = new CloneTemplate(depletedInfo, TechType.ReactorRod);

            depletedRod.SetGameObject(depletedTemplate);
            depletedRod.SetUnlock(TechType.ReactorRod);
            depletedRod.SetPdaGroupCategory(TechGroup.Resources, TechCategory.BasicMaterials);

            depletedRod.Register();
        }
    }
}
