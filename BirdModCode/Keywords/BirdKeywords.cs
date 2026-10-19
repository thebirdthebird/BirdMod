using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BirdMod.BirdModCode.Keywords;

public class BirdKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] 
    public static CardKeyword TalonKeyword;
}