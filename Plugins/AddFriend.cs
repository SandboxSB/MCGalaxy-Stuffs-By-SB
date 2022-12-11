using System;
using MCGalaxy;
using MCGalaxy.Commands;
using MCGalaxy.DB;

namespace Marry 
{
	public sealed class MarryPlugin : Plugin 
	{
		public override string name { get { return "&cSB's &7Add Friend &6Plugin"; } }
		public override string MCGalaxy_Version { get { return "1.9.3.1"; } }
		
		public const string EXTRA_KEY = "__Add_Name";
		public static PlayerExtList marriages;
		static OnlineStatPrinter onlineLine;
		static OfflineStatPrinter offlineLine;

		public override void Load(bool startup) {
			Command.Register(new CmdAccept());
			Command.Register(new CmdDeny());
			Command.Register(new CmdDivorce());
			Command.Register(new CmdMarry());

			marriages = PlayerExtList.Load("extra/friends.txt");		
			onlineLine  = (p, who) => FormatMarriedTo(p, who.name);
			offlineLine = (p, who) => FormatMarriedTo(p, who.Name);
			OnlineStat.Stats.Add(onlineLine);
			OfflineStat.Stats.Add(offlineLine);
		}
		
		public override void Unload(bool shutdown) {
			Command.Unregister(Command.Find("Accept1"));
			Command.Unregister(Command.Find("Deny1"));
			Command.Unregister(Command.Find("DelFriend"));
			Command.Unregister(Command.Find("AddFriend"));
			
			OnlineStat.Stats.Remove(onlineLine);
			OfflineStat.Stats.Remove(offlineLine);
		}
		
		
		static void FormatMarriedTo(Player p, string who) {
			string data = marriages.FindData(who);
			if (data == null) return;
			p.Message("  Friends with {0}", p.FormatNick(data));
		}
				
		public static Player CheckProposal(Player p) {
			string name = p.Extras.GetString(MarryPlugin.EXTRA_KEY);
			if (name == null) {
				p.Message("You do not have a pending friends request."); return null;
			}
			
			Player src = PlayerInfo.FindExact(name);
			if (src == null) {
				p.Message("The person you want to be friends is not online."); return null;
			}
			
			if (MarryPlugin.marriages.FindData(name) != null) {
				p.Message("{0} is already have friends.", p.FormatNick(name));
				p.Extras.Remove(MarryPlugin.EXTRA_KEY); return null;
			}
			
			if (MarryPlugin.marriages.FindData(p.name) != null) {
                p.Message("You already have friends.");
				return null;
			}
			return src;
		}
	}
	
	public sealed class CmdAccept : Command 
	{
		public override string name { get { return "Accept1"; } }
		public override string type { get { return "fun"; } }
		
		public override void Use(Player p, string message) {
			Player proposer = MarryPlugin.CheckProposal(p);
			if (proposer == null) return;
			
			const string msg = "-λNICK &aaccepted {0}&S to become friendd, and they are now best friends!";
			Chat.MessageFrom(p, string.Format(msg, p.FormatNick(proposer)));
			
			p.Message("&bYou &aaccepted &b{0}&b to become friends", p.FormatNick(proposer));
			proposer.SetMoney(proposer.money - 200);
			
			MarryPlugin.marriages.Update(p.name, proposer.name);
			MarryPlugin.marriages.Update(proposer.name, p.name);
			MarryPlugin.marriages.Save();
			p.Extras.Remove(MarryPlugin.EXTRA_KEY);
		}
		
		public override void Help(Player p) {
			p.Message("&T/Accept &H- Accepts friends request.");
		}
	}

	public class CmdDeny : Command 
	{
		public override string name { get { return "Deny1"; } }
		public override string type { get { return "fun"; } }
		
		public override void Use(Player p, string message) {
			Player proposer = MarryPlugin.CheckProposal(p);
			if (proposer == null) return;
			
			const string msg = "-λNICK &Sdenied {0}&S to become friends :'( &f";
			Chat.MessageFrom(p, string.Format(msg, p.FormatNick(proposer)));
			
			p.Message("&bYou &cdenied &b{0}&b to become friends", p.FormatNick(proposer));
			p.Extras.Remove(MarryPlugin.EXTRA_KEY);
		}
		
		public override void Help(Player p) {
			p.Message("&T/Deny1 &H- Denies a pending marriage proposal.");
		}
	}

	public sealed class CmdDivorce : Command 
	{
		public override string name { get { return "DelFriend"; } }
		public override string type { get { return "fun"; } }
		
		public override void Use(Player p, string message) {
			string marriedTo = MarryPlugin.marriages.FindData(p.name);
			if (marriedTo == null) { p.Message("You don't have any friends ."); return; }
			
			if (p.money < 50) {
				p.Message("You need at least 50 &3{0} &Sto left your friend", Server.Config.Currency); 
				return;
			}
			p.SetMoney(p.money - 50);
			
			MarryPlugin.marriages.Remove(p.name);
			MarryPlugin.marriages.Remove(marriedTo);
			MarryPlugin.marriages.Save();
			
			const string msg = "-λNICK&S and {0}&S are no longer friends-";
			Chat.MessageFrom(p, string.Format(msg, p.FormatNick(marriedTo)));
			
			Player partner = PlayerInfo.FindExact(marriedTo);
			if (partner != null)
				partner.Message("{0} &bare not your friend anymore.", partner.FormatNick(p));
		}
		
		public override void Help(Player p) {
			p.Message("&T/DelFriend");
			p.Message("&HLeaves the player you are friends with.");
			p.Message("  &HCosts 50 &3" + Server.Config.Currency);
		}
	}

	public sealed class CmdMarry : Command 
	{
		public override string name { get { return "AddFriend"; } }
		public override string type { get { return "fun"; } }
		
		public override void Use(Player p, string message) {
			string entry = MarryPlugin.marriages.FindData(p.name);
			if (entry != null) {
				p.Message("&WYou are already have friends"); return;
			}
			
			if (p.money < 1) {
				p.Message("You need at least 1 &3{0} &Sto be friends with someone.", Server.Config.Currency); 
				return;
			}
			
			Player partner = PlayerInfo.FindMatches(p, message);
			if (partner == null) return;
			if (partner == p) { p.Message("&WYou cannot be friends with yourself."); return; }
			
			entry = MarryPlugin.marriages.FindData(partner.name);
			if (entry != null) {
				p.Message("{0} &Salready have friends", p.FormatNick(partner)); 
				return;
			}
			
			const string msg = "λNICK&S is asking {0}&S to become friends";
			Chat.MessageFrom(p, "-λNICK&S Become friends.");
			Chat.MessageFrom(p, string.Format(msg, p.FormatNick(partner)));
			
			partner.Extras[MarryPlugin.EXTRA_KEY] = p.name;
			partner.Message("&bTo accept become friends type &a/Accept1");
			partner.Message("&bOr to deny it, type &c/Deny1");
		}
		
		public override void Help(Player p) {
			p.Message("&T/AddFriend [player]");
			p.Message("&HAdd friends.");
			p.Message("  &HCosts 200 &3" + Server.Config.Currency);
		}
	}	
}
