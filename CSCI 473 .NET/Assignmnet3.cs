/* Corbin Lutsch & Matthew Lord
 *     Z1837389  & Z1848456
 *  CSCI - 473
 *  Due: 02/28/19
 *  Assignment 3 - A LINQ to the Past
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace mandc_Assign3
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            //read the input files into Global collections
            Menu menu = new Menu();

            //populate selection boxes
            Populate_Boxes();
        }

        /***************************************************************
         public void Populate_Boxes()

        Use: Populates all of the combo boxes with values

        Parameters: none

        Returns: nothing
        ***************************************************************/
        public void Populate_Boxes()
        {
            cbClass.Items.Add("Warrior");
            cbClass.Items.Add("Mage");
            cbClass.Items.Add("Druid");
            cbClass.Items.Add("Priest");
            cbClass.Items.Add("Warlock");
            cbClass.Items.Add("Rogue");
            cbClass.Items.Add("Paladin");
            cbClass.Items.Add("Hunter");
            cbClass.Items.Add("Shaman");

            cbServer1.Items.Add("Beta4Azeroth");
            cbServer1.Items.Add("TKWasASetback");
            cbServer1.Items.Add("ZappyBoi");

            cbServer2.Items.Add("Beta4Azeroth");
            cbServer2.Items.Add("TKWasASetback");
            cbServer2.Items.Add("ZappyBoi");

            cbServer3.Items.Add("Beta4Azeroth");
            cbServer3.Items.Add("TKWasASetback");
            cbServer3.Items.Add("ZappyBoi");

            cbRole.Items.Add("Tank");
            cbRole.Items.Add("Healer");
            cbRole.Items.Add("Damage");

            cbType.Items.Add("Casual");
            cbType.Items.Add("Questing");
            cbType.Items.Add("Mythic+");
            cbType.Items.Add("Raiding");
            cbType.Items.Add("PVP");
        }

        /***************************************************************
        private void Button_Class_Types(object obj, EventArgs args)

        Use: Finds all players of a class type from a single server 
        and outputs to the rich text box $$$

        Parameters: 1. object obj - the button
                    2. EventArgs args - the events arguments

        Returns: nothing
        ***************************************************************/
        private void Button_Class_Types(object obj, EventArgs args)
        {
            Button results = obj as Button;

            if (results != null)
            {
                if (cbClass.SelectedIndex != -1 && cbServer1.SelectedIndex != -1)
                {
                   
                    rTextBox.Text = "All " + cbClass.SelectedItem.ToString() + " from " + cbServer1.SelectedItem.ToString();
                    rTextBox.Text += "\n--------------------------------------------------------------------------------\n";

                    //query to find all the players with selected Class and server
                    var findPlayers = from P in Global.SortedPlayers
                                      from G in Global.SortedGuilds
                                      where (int)P.Role2 == cbClass.SelectedIndex
                                      where G.Server == cbServer1.SelectedItem.ToString()
                                      where G.Gid == P.GuildId
                                      orderby P.Level ascending
                                      select P;
                  
                    foreach (Player player in findPlayers) //loop through every player 
                    {
                        rTextBox.Text += player.ToString(); //add it to the textbox
                    }

                    if (!findPlayers.Any()) //we did not find any matches
                    {
                        rTextBox.Text += "No results found.\n";
                    }

                    rTextBox.Text += "\nEND RESULTS\n--------------------------------------------------------------------------------\n";
                }
                else
                {
                    rTextBox.Text = "Please select a Class and a Server to complete the query.";
                }
            }
  
        }


        /***************************************************************
        private void Button_Role_Percent(object obj, EventArgs args)

        Use: calculates and displays the percentage of each role in a server

        Parameters: 1. object obj - the button
                    2. EventArgs args - the events arguments

        Returns: nothing
        ***************************************************************/
        private void Button_Role_Percent(object obj, EventArgs args)
        {
            Button results = obj as Button;

            if (results != null)
            {
                if (cbServer2.SelectedIndex != -1)
                {
                    rTextBox.Text = "Percentage of Each Race from " + cbServer2.SelectedItem.ToString();
                    rTextBox.Text += "\n--------------------------------------------------------------------------------\n";

                    //select all of the players with the selected server name
                    var findTotalPlayers = from G in Global.SortedGuilds
                                           from P in Global.SortedPlayers
                                           where G.Server == cbServer2.SelectedItem.ToString()
                                           where P.GuildId == G.Gid
                                           select P;

                    //find all of the players that are orcs
                    var findOrcs = from O in findTotalPlayers
                                   where O.Race2 == 0
                                   select O;

                    //find all of the players that are trololols
                    var findTroll = from Tr in findTotalPlayers
                                   where (int)Tr.Race2 == 1
                                   select Tr;

                    //find all of the players that are taurens
                    var findTauren = from Ta in findTotalPlayers
                                   where (int)Ta.Race2 == 2
                                   select Ta;

                    //find all of the players that are FORSAKEN U
                    var findForsaken = from F in findTotalPlayers
                                   where (int)F.Race2 == 3
                                   select F;

                    rTextBox.Text += String.Format("{0, -15}\t{1, -10: 00.00%}\n", "Orc:", (float)findOrcs.Count() / findTotalPlayers.Count());
                    rTextBox.Text += String.Format("{0, -15}\t{1, -10: 00.00%}\n", "Troll:", (float)findTroll.Count() / findTotalPlayers.Count());
                    rTextBox.Text += String.Format("{0, -15}\t{1, -10: 00.00%}\n", "Tauren:", (float)findTauren.Count() / findTotalPlayers.Count());
                    rTextBox.Text += String.Format("{0, -15}\t{1, -10: 00.00%}\n", "Forsaken:", (float)findForsaken.Count() / findTotalPlayers.Count());
                    
                    rTextBox.Text += "\nEND RESULTS\n--------------------------------------------------------------------------------\n";
                }
                else
                {
                    rTextBox.Text = "You must select a server to find the percentage of each race.";
                }
            }


        }


        /***************************************************************
        private void Min_Value_Changed(object obj, EventArgs args)

        Use: increases max value when min value goes above it

        Parameters: 1. object obj - the button
                    2. EventArgs args - the events arguments

        Returns: nothing
        ***************************************************************/
        private void Min_Value_Changed(object obj, EventArgs args)
        {
            if (min.Value > max.Value)
            {
                max.Value = min.Value + 1;
            }
        }


        /***************************************************************
        private void Max_Value_Changed(object obj, EventArgs args)

        Use: decreases min value when max value goes below it

        Parameters: 1. object obj - the button
                    2. EventArgs args - the events arguments

        Returns: nothing
        ***************************************************************/
        private void Max_Value_Changed(object obj, EventArgs args)
        {
            if (max.Value < min.Value)
            {
                min.Value = max.Value - 1;
            }
        }

        /***************************************************************
        private void Button_Roles_Server_Range(object obj, EventArgs args)

        Use: Selects all players of a specific role on a server within a range
        of levels

        Parameters: 1. object obj - the button
                    2. EventArgs args - the events arguments

        Returns: nothing
        ***************************************************************/

        private void Button_Roles_Server_Range(object obj, EventArgs args)
        {
            Button results = obj as Button;

            if (results != null)
            {
                if (cbRole.SelectedIndex != -1 && cbServer3.SelectedIndex != -1)
                {

                    rTextBox.Text = "All " + cbRole.SelectedItem.ToString() + " from [" + cbServer3.SelectedItem.ToString() + "] Levels ";
                    rTextBox.Text += min.Value.ToString() + " to " + max.Value.ToString();
                    rTextBox.Text += "\n--------------------------------------------------------------------------------\n";

                    //query to find all the players with selected Role from selected server in level range
                    var findPlayers = from P in Global.SortedPlayers
                                      from G in Global.SortedGuilds
                                      where (int)P.Type2 == cbRole.SelectedIndex && P.Level >= min.Value && P.Level <= max.Value
                                      where G.Server == cbServer3.SelectedItem.ToString()
                                      where P.GuildId == G.Gid
                                      orderby P.Level ascending select P;

                    foreach(Player p in findPlayers)
                    {
                        rTextBox.Text += p.ToString(); //add it to the textbox
                    }
                    
                    if (!findPlayers.Any())
                    {
                        rTextBox.Text += "No results found.\n";
                    }

                    rTextBox.Text += "\nEND RESULTS\n--------------------------------------------------------------------------------\n";
                }
                else
                {
                    rTextBox.Text = "Please select a Role and a Server to complete the query.";
                }
            }
        }

        /***************************************************************
        private void Button_Guild_Types(object obj, EventArgs args)

        Use: prints each server name follows by the guilds that are of
        the selected type

        Parameters: 1. object obj - the button
                    2. EventArgs args - the events arguments

        Returns: nothing
        ***************************************************************/

        private void Button_Guild_Types(object obj, EventArgs args)
        {
            Button results = obj as Button;
            bool printServerName = true;

            if (results != null)
            {
                if (cbType.SelectedIndex != -1)
                {
                    rTextBox.Text = "All " + cbType.SelectedItem.ToString() + "-Type of Guilds";
                    rTextBox.Text += "\n--------------------------------------------------------------------------------\n";

                    // query to select all of the guilds with the server type and group them by server name
                    var findGuilds = from G in Global.SortedGuilds
                                     where (int)G.Type2 == cbType.SelectedIndex
                                     group G by G.Server;

                    foreach (var guildGroup in findGuilds) 
                    {
                        foreach (Guild g in guildGroup)
                        {
                            if (printServerName) //print the server name once 
                            {
                                rTextBox.Text += g.Server + "\n";
                                printServerName = false;
                            }
                            rTextBox.Text += "\t<" + g.Gname + ">\n";
                        }
                        printServerName = true;
                    }

                    rTextBox.Text += "\nEND RESULTS\n--------------------------------------------------------------------------------\n";
                }
                else
                {
                    rTextBox.Text = "Please select a Guild type.";
                }

            }

        }

        /***************************************************************
        private void Button_Not_In_Role(object obj, EventArgs args)

        Use: displays each player that can fill the selected role 
        that are currently not in that role already

        Parameters: 1. object obj - the button
                    2. EventArgs args - the events arguments

        Returns: nothing
        ***************************************************************/

        private void Button_Not_In_Role(object obj, EventArgs args)
        {
            Button results = obj as Button;

            if (results != null)
            {
                if (radioTank.Checked)
                {
                    rTextBox.Text = "All Eligible Players Not Fulfilling the Tank Role";
                    rTextBox.Text += "\n--------------------------------------------------------------------------------\n";

                    // query to select all eligible players that have the option to be a tank (Warrior 0, Druid 2, Paladin 6)
                    var findPlayers = from P in Global.SortedPlayers
                                      where P.Role2 == 0 || (int)P.Role2 == 2 || (int)P.Role2 == 6
                                      where P.Type2 != 0
                                      orderby P.Level ascending select P;

                    foreach (Player p in findPlayers)
                    {
                            rTextBox.Text += p.ToString();
                    }
                    rTextBox.Text += "\nEND RESULTS\n--------------------------------------------------------------------------------\n";
                }
                else if (radioHealer.Checked)
                {
                    rTextBox.Text = "All Eligible Players Not Fulfilling the Healer Role";
                    rTextBox.Text += "\n--------------------------------------------------------------------------------\n";

                    // query to select all eligible players that have the option to be a healer (Druid 2, Priest 3, Paladin 6, Shaman 8)
                    var findPlayers = from P in Global.SortedPlayers
                                      where (int)P.Role2 == 2 || (int)P.Role2 == 3 || (int)P.Role2 == 6 || (int)P.Role2 == 8
                                      where (int)P.Type2 != 1
                                      orderby P.Level ascending select P;

                    foreach (Player p in findPlayers)
                    {
                            rTextBox.Text += p.ToString();
                    }
                    rTextBox.Text += "\nEND RESULTS\n--------------------------------------------------------------------------------\n";

                }
                else if (radioDamage.Checked) 
                {
                    rTextBox.Text = "All Eligible Players Not Fulfilling the Damage Role";
                    rTextBox.Text += "\n--------------------------------------------------------------------------------\n";

                    //ALL types can be damage so query just orders them by their level and takes out current DAMAGE TYPES ^.^
                    var findPlayers = from P in Global.SortedPlayers
                                      where (int)P.Type2 != 2
                                      orderby P.Level ascending select P;

                    foreach (Player p in findPlayers)
                    {
                            rTextBox.Text += p.ToString();
                    }
                    rTextBox.Text += "\nEND RESULTS\n--------------------------------------------------------------------------------\n";
                }
                else
                {
                    rTextBox.Text = "Please select Tank, Healer, or Damage.";
                }

                
            }


        }

        /***************************************************************
        private void Button_Max_Level(object obj, EventArgs args)

        Use: prints the percentage of max level players in each guild

        Parameters: 1. object obj - the button
                    2. EventArgs args - the events arguments

        Returns: nothing
        ***************************************************************/

        private void Button_Max_Level(object obj, EventArgs args)
        {
            Button results = obj as Button;

            string guildName = null;

            if (results != null)
            {
                rTextBox.Text = "Percentage of Max Level Players in All Guilds";
                rTextBox.Text += "\n--------------------------------------------------------------------------------\n";

                //query to group all players in a guild
                var groupGuilds = from P in Global.SortedPlayers group P by P.GuildId;

                foreach(var groupOfPlayers in groupGuilds) //loop through each guild
                {
                    foreach(Player p in groupOfPlayers)
                    {
                        var getGuildName = from G in Global.SortedGuilds where G.Gid == p.GuildId select G;
                        foreach(Guild g in getGuildName)
                        {
                            guildName = "<" + g.Gname + ">";
                            rTextBox.Text += String.Format("{0, -30}", guildName);
                            break;
                        }
                        break;
                    }
                    //select all of the max level players in group of players
                    var totalMaxLevel = from P in groupOfPlayers
                                        where P.Level == 60
                                        select P;

                    rTextBox.Text += String.Format("{0, -5: 0.00%}\n\n", (float)totalMaxLevel.Count() / groupOfPlayers.Count());
                    
                }

                rTextBox.Text += "\nEND RESULTS\n--------------------------------------------------------------------------------\n";
            }
        }

    }




    public static class Global
    {
        public static SortedSet<Player> SortedPlayers = new SortedSet<Player>();
        public static SortedSet<Guild> SortedGuilds = new SortedSet<Guild>();
    }


    public class Menu
    {
        string line;
        string[] tokens;

        /***************************************************************
        public Menu()

        Use: Default constructor reads in all input files and assigns
        them to their corresponding containers contained in a
        seperate Global class for all classes to access.
     
        Parameters: none
			
        Returns: nothing
        ***************************************************************/
        public Menu()
        {
            //read the players input file
            using (StreamReader inFile = new StreamReader("..\\..\\players.txt"))
            {
                line = inFile.ReadLine();

                while (line != null)
                {
                    tokens = line.Split(); //split the input up 

                    //create a new player using default constructor 
                    Player myPlayer = new Player(Convert.ToUInt32(tokens[0]), tokens[1], Convert.ToUInt32(tokens[2]), Convert.ToUInt32(tokens[3]), Convert.ToUInt32(tokens[4]),
                       Convert.ToUInt32(tokens[5]), Convert.ToUInt32(tokens[6]), Convert.ToUInt32(tokens[7]));

                    //add to set
                    Global.SortedPlayers.Add(myPlayer);

                    line = inFile.ReadLine();
                }
            }

            //read in the guilds text file
            using (StreamReader inFile = new StreamReader("..\\..\\guilds.txt"))
            {
                string[] tokens2;
                line = inFile.ReadLine();

                while (line != null)
                {
                    string rebuild = null; //to hold the rebuilded string
                    tokens = line.Split();//seperate the guild ID from the rest of the string

                    //now piece back the guild name since guilds can be more than one word 
                    //seperated by spaces 
                    for (int i = 2; i < tokens.Length; i++)
                    {
                        if (i != tokens.Length - 1)
                            rebuild += tokens[i] + " ";
                        else
                            rebuild += tokens[i];
                    }
                    tokens2 = rebuild.Split('-'); //split the guild name from server 

                    //create new Guild object
                    Guild myGuild = new Guild(Convert.ToUInt32(tokens[0]), Convert.ToUInt32(tokens[1]), tokens2[0], tokens2[1]);
                    Global.SortedGuilds.Add(myGuild);

                    line = inFile.ReadLine();
     
                }
            }
        }

    }

    public class Guild : IComparable
    {
        public enum Type { Causal, Questing, Mythic, Raiding, PVP };
        readonly Type type;
        readonly uint gid;
        readonly string gname;
        readonly string server;

        public Type Type2
        {
            get { return type; }
        }

        public uint Gid //this is my public property
        {
            get { return gid; }
        }

        public string Gname //this is my public property
        {
            get { return gname; }
        }

        public string Server //this is my public property
        {
            get { return server; }
        }

        /***************************************************************
        public int CompareTo(object obj)

        Use: Compares Guild objects so they can be sorted in Sorted Sets
     
        Parameters: 1. object obj - the object on the "right side" of the operand
        to be compared with

        Returns: an integer if obj is null, exception if it is not a Guild object,
        or recursively calls the CompareTo to for sorting.
        ***************************************************************/
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Guild rightObj = obj as Guild;

            int result = gname.CompareTo(rightObj.gname);

            if (rightObj != null)
            {
                if (result == 0)
                    return 1;
                else
                    return result;
            }
            else
            {
                throw new ArgumentException("[Guild]:CompareTo argument is not a Guild");
            }
        }

        /***************************************************************
        public (uint g, string n, string s) //default constructor #1

        Use: Default constructor initializes attributes for the Guild class
     
        Parameters: 
        uint g - guild id
        uint type - the type of the guild
        string n - guild name
        string s - server name
      
        Returns: nothing
        ***************************************************************/
        public Guild(uint g, uint type, string n, string s)
        {
            this.gid = g;
            this.type = (Type)type;
            this.gname = n;
            this.server = s;
        }

        /***************************************************************
        public () //default constructor #2

        Use: Default constructor initializes attributes for the Guild class
     
        Parameters: 
      
        Returns: nothing
        ***************************************************************/
        public Guild()
        {
            this.gid = 0;
            this.type = 0;
            this.gname = "";
            this.server = "";
        }

        /***************************************************************
        public override string ToString()

        Use: Overrides the ToString() method for the Guild class to allow
        efficient printing

        Parameters: none

        Returns: nothing
        ***************************************************************/
        public override string ToString()
        {
            string addBrackets = "<" + server + ">";

            return String.Format("{0, -25} {1, -20} \n", gname, addBrackets);
        }


    }

    public class Player : IComparable
    {
        public enum Race { Orc, Troll, Tauren, Forsaken };
        public enum Role { Warrior, Mage, Druid, Priest, Warlock, Rogue, Paladin, Hunter, Shaman };
        public enum Type { Tank, Healer, Damage };
        readonly uint id;
        readonly string name;
        readonly Race race;
        readonly Role role;
        readonly Type type;
        uint level;
        uint exp;
        uint guildID;


        public uint Id //this is my public property
        {
            get { return id; }
        }

        public string Name //this is my public property
        {
            get { return name; }
        }

        public Race Race2 //this is my public property
        {
            get { return race; }
        }

        public Role Role2
        {
            get { return role; }
        }

        public Type Type2
        {
            get { return type; }
        }

        public uint Level
        {
            get { return level; }
            set { level = value; }
        }

        public uint Exp
        {
            get { return exp; }
            set { exp = value; }

        }

        public uint GuildId
        {
            get { return guildID; }
            set { guildID = value; }
        }


        /***************************************************************
        public Player(uint id, string name, uint race, uint level,
        uint exp, uint guildID, params uint[] gear) //default constructor #1

        Use: Default constructor initializes attributes for the Player class
     
        Parameters: 
        uint id - the item id
        string name - the string name of the player
        uint race - the player's race 
        uint role - the player's role
        uint type - the type of role 
        uint world - the player's logged in world
        uint level- the player's level
        uint exp - the player's exp
        uint guildId - the -player's guild id

        Returns: nothing
        ***************************************************************/
        public Player(uint id, string name, uint race, uint role, uint type, uint level, uint exp, uint guildID)
        {
            this.id = id;
            this.name = name;
            this.race = (Race)race;
            this.role = (Role)role;
            this.type = (Type)type;
            this.level = level;
            this.exp = exp;
            this.guildID = guildID;
        }

        /***************************************************************
        public Player() //default constructor #2

        Use: Default constructor initializes attributes for the Player class
     
        Parameters: none

        Returns: nothing
        ***************************************************************/
        public Player()
        {
            this.id = 0;
            this.name = "";
            this.race = 0;
            this.role = 0;
            this.type = 0;
            this.level = 0;
            this.exp = 0;
            this.guildID = 0;

        }

        /***************************************************************
        public int CompareTo(object obj)

        Use: Compares Player objects so they can be sorted in Sorted Sets
     
        Parameters: 1. object obj - the object on the "right side" of the operand
        to be compared with

        Returns: an integer if obj is null, exception if it is not a Player object,
        or recursively calls the CompareTo to for sorting.
        ***************************************************************/
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Player rightObj = obj as Player;
            if (rightObj != null)
                return name.CompareTo(rightObj.name);
            else
                throw new ArgumentException("[Player]:CompareTo argument is not a Player");
        }

        /***************************************************************
        public override string ToString()

        Use: Overrides the ToString() method for the Player class to allow
        efficient printing

        Parameters: none

        Returns: nothing
        ***************************************************************/
        public override string ToString()
        {
            string gname = "";
            string gserver = "";

            foreach (Guild g in Global.SortedGuilds)
            {
                if (g.Gid == GuildId)
                {
                    gname = g.Gname;
                    gserver = g.Server;
                }
            }
            return String.Format("Name: {0, -15} ({1, -8} - {2, -7})  Race: {3, -8} Level: {4, -2}  <{5}> \n", name, role, type, race, level, gname);
        }

    }
}
