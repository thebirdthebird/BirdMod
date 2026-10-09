using BirdMod.BirdModCode.Cards;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace BirdMod.BirdModCode.Patches;

// THANK YOU TO DARKGLADE, WHO IM BASING THIS CODE OFF OF, PLEASE SEE THE ORIGINAL:
// https://github.com/Darkglade1/Ruina2/blob/main/Ruina2Code/Patches/CardHolderPatch.cs

[HarmonyPatch(typeof (NCardHolder), "SmallScale", MethodType.Getter)]
public static class CardHolderPatchSmallScale
{
    private static bool Prefix(NCardHolder __instance, ref Vector2 __result)
    {
        var c = __instance.CardModel;
        if (c is not BirdModCard card)
            return true;
        if (card == null) return true;
        if (card.HELP)
        {
            __result = Vector2.One * 0.3f;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof (NCardHolder), "HoverScale", MethodType.Getter)]
public static class CardHolderPatchHoverScale
{
    private static bool Prefix(NCardHolder __instance, ref Vector2 __result)
    {
        var c = __instance.CardModel;
        if (c is not BirdModCard card)
            return true;
        if (card == null) return true;
        if (card.HELP)
        {
            __result = Vector2.One * 0.6f;
            return false;
        }
        return true;
    }
}