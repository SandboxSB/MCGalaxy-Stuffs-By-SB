using System;
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Commands;

namespace MCGalaxy
{
    public class CmdParkourrules : Command
    {
        public override string name { get { return "CustomRules"; } }

        public override string shortcut { get { return "CustomRulesShortCut"; } }

        public override string type { get { return "other"; } }

        public override bool museumUsable { get { return false; } }

        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message)
        {
            List<string> rules = new List<string>();
            if (!File.Exists("text/customrules.txt"))
            {
                File.WriteAllText("text/customrules.txt", "No rules entered yet!");
            }
            using (StreamReader reader = File.OpenText("text/customrules.txt"))
            {
                while (!reader.EndOfStream)
                rules.Add(reader.ReadLine());
            }
            
            p.Message("%fCustom Rules:");
            foreach (string rule in rules) {
                p.Message(rule);
            }

        }

        public override void Help(Player p)
        {
            p.Message("%T/CustomRules");
            p.Message("%HSends you the rules of the Custom Rules.");
        }
    }
}
