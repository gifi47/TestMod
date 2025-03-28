using com.mindblocks.i18n;
using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;

namespace TestMod
{
    [HarmonyPatch(typeof(BaseTranslation), "createSheets")]
    public static class BaseTranslationPatch
    {
        // Prefix method (runs before the original method)
        static void Prefix(LanguageCode lang, ref Dictionary<SheetTitle, Dictionary<string, string>> entries)
        {
            Melon<Test>.Logger.Msg("Enter createSheets with language " + lang);
            // Modify 'entries' or log data before execution
        }

        // Postfix method (runs after the original method)
        static void Postfix(LanguageCode lang, ref Dictionary<SheetTitle, Dictionary<string, string>> entries)
        {
            Melon<Test>.Logger.Msg("Exit createSheets with language " + lang);
            if (lang == LanguageCode.RU)
            {
                entries[SheetTitle.ItemNames].Add("CustomBow", "Эльфийский Зеркальный Лук");
                entries[SheetTitle.ItemNames].Add("MirrorOfElves", "Зеркало Эльфов");
                entries[SheetTitle.ItemNames].Add("Amogus", "[FF0000]АМОГУС[-]");
                entries[SheetTitle.ItemNames].Add("ExperiencePotion", "Зелье Опыта");
                entries[SheetTitle.ItemNames].Add("RainbowRod", "[FF0000]Р[-][FF7F00]а[-][FFFF00]д[-][00FF00]у[-][00FFFF]ж[-][0000FF]н[-][8B00FF]ы[-][FF0000]й[-] [FF7F00]Ж[-][FFFF00]е[-][00FF00]з[-][00FFFF]л[-]");
                entries[SheetTitle.ItemNames].Add("RainbowOre", "Радужный Минерал");
                entries[SheetTitle.ItemNames].Add("SusSword", "Нож Предателя");

                entries[SheetTitle.Items].Add("CustomBow", "Этот лук соджержит в себе частицу запретной магии [FF0000]Эльфийских Зеркал[-].\n\nПри выстреле из этого лука выбранная стрела копируется через небольшой промежуток времени три раза.");
                entries[SheetTitle.Items].Add("MirrorOfElves", "Странный артефакт прошлого из далёких земель. По слухам, люди смотревшие на своё отражение в этом зеркале слишком долго, теряли рассудок.");
                entries[SheetTitle.Items].Add("Amogus", "Among Us (с англ. — «Среди нас») — многопользовательская компьютерная игра, разработанная американской игровой студией Innersloth и выпущенная для устройств под управлением iOS и Android 15 июня 2018 года, а затем для компьютеров Windows 16 ноября 2018 года.");
                entries[SheetTitle.Items].Add("ExperiencePotion", "Осторожно, концентрированный [FFFF00]ОПЫТ[-]!");
                entries[SheetTitle.Items].Add("RainbowRod", "Этот посох обладает силой всех магических стихий!");
                entries[SheetTitle.Items].Add("RainbowOre", "Необычный минерал, от него так и веет магией.");
                entries[SheetTitle.Items].Add("SusSword", "[FF0000]You're SUS![-]");

                entries[SheetTitle.ItemEffects].Add("Experience", "[FFFF00]+{0} Опыт[-]");
            }
            else
            {
                entries[SheetTitle.ItemNames].Add("CustomBow", "Elven Mirror Bow");
                entries[SheetTitle.ItemNames].Add("MirrorOfElves", "Elven Mirror");
                entries[SheetTitle.ItemNames].Add("Amogus", "[FF0000]AMOGUS[-]");
                entries[SheetTitle.ItemNames].Add("ExperiencePotion", "Experience Potion");
                entries[SheetTitle.ItemNames].Add("RainbowRod", "[FF0000] R[-][FF7F00]a[-][FFFF00]i[-][00FF00]n[-][00FFFF]b[-][0000FF]o[-][8B00FF]w[-] [FF0000]R[-][FF7F00]o[-][FFFF00]d[-]");
                entries[SheetTitle.ItemNames].Add("RainbowOre", "Rainbow Mineral");
                entries[SheetTitle.ItemNames].Add("SusSword", "Imposter's Knife");

                entries[SheetTitle.Items].Add("CustomBow", "This bow contains a fragment of the forbidden magic of the [FF0000]Elven Mirrors[-].\n\nWhen shooting, the selected arrow is copied three times in a short intervals.");
                entries[SheetTitle.Items].Add("MirrorOfElves", "A strange ancient artifact from distant lands. Rumor has it that those who stared at their reflection in this mirror for too long lost their sanity.");
                entries[SheetTitle.Items].Add("Amogus", "Among Us is a multiplayer video game developed by the American studio Innersloth, released for iOS and Android on June 15, 2018, and for Windows on November 16, 2018.");
                entries[SheetTitle.Items].Add("ExperiencePotion", "Caution, concentrated [FFFF00]EXPERIENCE[-]!");
                entries[SheetTitle.Items].Add("RainbowRod", "This staff wields the power of all magical elements!");
                entries[SheetTitle.Items].Add("RainbowOre", "A peculiar mineral, radiating pure magic.");
                entries[SheetTitle.Items].Add("SusSword", "[FF0000]You're SUS![-]");

                entries[SheetTitle.ItemEffects].Add("Experience", "[FFFF00]+{0} Exp[-]");
            }
            // Add your custom logic after the method runs
        }
    }
}
