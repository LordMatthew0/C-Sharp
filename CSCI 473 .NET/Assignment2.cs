/* Corbin Lutsch & Matthew Lord
 *     Z1837389  & Z1848456
 *  CSCI - 473
 *  Due: 02/14/19
 *  Assignment 2 - Practice Good Form
 * 
 */
 


using System;
using System.IO; //for stream reader
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mandc_Assign2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Menu menu = new Menu();

            //generate list boxes
            UpdateLists();

            //generate drop down menus
            for(int i = 0; i < 4; i++)
            {
                cbRace.Items.Add((Player.Race)i);
            }
            for (int i = 0; i < 9; i++)
            {
                cbClass.Items.Add((Player.Role)i);
            }
            cbServer.Items.Add("Beta4Azeroth");
            cbServer.Items.Add("TKWasASetback");
            cbServer.Items.Add("ZappyBoi");

            cbType.Items.Add("Casual");
            cbType.Items.Add("Questing");
            cbType.Items.Add("Mythic+");
            cbType.Items.Add("Raiding");
            cbType.Items.Add("PVP");
        }

       /***************************************************************
       private void UpdateLists()

       Use: Clears the list boxes and then re-initializes them with values
       in the players sorted set and guilds sorted set

       Parameters: none

       Returns: nothing
       ***************************************************************/
        private void UpdateLists()
        {
            //clear the list boxes
            lsBoxGuilds.Items.Clear();
            lsBoxPlayers.Items.Clear();

            foreach (Player p in Global.SortedPlayers) //add each player, role, and level to the list box
            {
                lsBoxPlayers.Items.Add(String.Format("{0, -15}\t{1, -10} {2, -10} \n", p.Name, p.Role2, p.Level));
            }

            foreach (Guild g in Global.SortedGuilds) //add each guild to the list box
            {
                lsBoxGuilds.Items.Add(String.Format("{0, -23}\t{1, -10} \n", g.Gname, g.Server));
            }
        }


        /***************************************************************
        private void Button_Print_Guild(object obj, EventArgs args)

        Use: Prints each player that belongs to the selected guild

        Parameters: 1. object obj - the button object
                    2. EventArgs args - the Event's arguments

        Returns: nothing
        ***************************************************************/
        private void Button_Print_Guild(object obj, EventArgs args)
        {
            rTextBox.Clear();

            Button printGuild = obj as Button;

            if (lsBoxGuilds.SelectedIndex != -1)
            {
                if (printGuild != null)
                {
                    //find the Guild selected
                    string[] tokens = lsBoxGuilds.SelectedItem.ToString().Split('\t');
 
                    Guild g = null;

                    foreach (Guild gu in Global.SortedGuilds)
                    {
                        if (gu.Gname == tokens[0].Trim() && gu.Server == tokens[1].Trim())
                           g = gu; //save the guild
                    }
                    
                    if (g != null)
                    {
                        string s = g.Gname + " " + g.Server;

                        rTextBox.Text = String.Format("Guild Listing for {0}\t\t{1}", g.Gname, g.Server);
                        rTextBox.Text += "\n-------------------------------------------------------------\n";

                        //print eac player in the guild
                        foreach (Player p in Global.SortedPlayers) //add each player, role, level, and guild to the output box
                        {
                            if (p.GuildId == g.Gid)
                            {
                                rTextBox.Text += (String.Format("Name: {0, -15} Race: {1, -15} Level: {2, -10} Guild: {3, -10} \n", p.Name, p.Race2, p.Level, s));
                            }

                        }
                    }
                    else
                    {
                        rTextBox.Text = "ERROR: Guild not found"; 
                    }
                    
                }
            }
            else
            {
                rTextBox.Text = "Please select a guild whose roster you would like to see";
            }
            
        }


        /***************************************************************
        private void Button_Disband_Guild(object obj, EventArgs args)

        Use: Finds each player in the guild to be disbanded and sets their
        guild id back to zero. Then removes the guild from the sorted guilds set.

        Parameters: 1. object obj - the button object
                    2. EventArgs args - the Event's arguments

        Returns: nothing
        ***************************************************************/
        private void Button_Disband_Guild(object obj, EventArgs e)
        {
            Button disbandGuild = obj as Button;
            uint counter = 0;
            rTextBox.Clear();
            if (lsBoxGuilds.SelectedIndex != -1)
            {
                if (disbandGuild != null)
                {
                    //get the guild name
                    string[] tokens = lsBoxGuilds.SelectedItem.ToString().Split('\t');

                    Guild remove = null;
                    
                    foreach(Guild g in Global.SortedGuilds)
                    {
                        if (g.Gname == tokens[0].Trim() && g.Server == tokens[1].Trim())
                        {
                            remove = g; //save the guild object
                        }
                    }

                    if (remove != null)
                    {
                        foreach (Player p in Global.SortedPlayers) //add each player, role, and level to the list box
                        {
                            if (p.GuildId == remove.Gid)
                            {
                                counter++;
                                p.GuildId = 0; //set the player's guild id to zero (not in guild)
                            }
                        }
                        //remove guild from list box
                        lsBoxGuilds.Items.RemoveAt(lsBoxGuilds.SelectedIndex);

                        //remove guild from sorted set
                        Global.SortedGuilds.Remove(remove);

                        rTextBox.Text = String.Format("{0} players have been disbanded from {1}\t\t{2}", counter, remove.Gname, remove.Server);
                    }
                    else
                    {
                        rTextBox.Text = "ERROR: Guild not found";
                    }
                    

                }
            }
            else
            {
                rTextBox.Text = "Please select a guild you would like to disband";
            }
        }


        /***************************************************************
        private void Button_Join_Guild(object obj, EventArgs args)

        Use: Finds the selected guild and selected player and then assigns
        that guild id to that players guild id.

        Parameters: 1. object obj - the button object
                    2. EventArgs args - the Event's arguments

        Returns: nothing
        ***************************************************************/
        private void Button_Join_Guild(object obj, EventArgs args)
        {
            rTextBox.Clear();

            Button joinGuild = obj as Button;

            if (lsBoxGuilds.SelectedIndex != -1 && lsBoxPlayers.SelectedIndex != -1)
            {
                if (joinGuild != null)
                {
                  //get the player name and the guild name 
                    string[] playerTokens = lsBoxPlayers.SelectedItem.ToString().Split('\t');
                    string[] guildTokens = lsBoxGuilds.SelectedItem.ToString().Split('\t');

                    Player p = null;
                    Guild g = null;

                    foreach(Player pl in Global.SortedPlayers)
                    {
                        if (pl.Name == playerTokens[0].Trim())
                            p = pl; //save the player object  
                    }

                    foreach(Guild gu in Global.SortedGuilds)
                    {
                        if (gu.Gname == guildTokens[0].Trim() && gu.Server == guildTokens[1].Trim())
                            g = gu; //save the guild object
                    }

                    if (p.GuildId == g.Gid) //player is already in that guild
                    {
                        rTextBox.Text = String.Format("{0} is already in {1} {2}", p.Name, g.Gname, g.Server);
                    }
                    else
                    {
                        p.GuildId = g.Gid; //set the player's guild to the new guild id
                        rTextBox.Text = String.Format("{0} has joined {1} {2}", p.Name, g.Gname, g.Server);
                    }
 
                }
            }
            else
            {
                rTextBox.Text = "You must select both a player and a guild from the lists above.";
            }
        }


        /***************************************************************
        private void Button_Leave_Guild(object obj, EventArgs args)

        Use: Finds the selected guild and selected player and then 
        assigns 0 to that player if they were in the selected guild

        Parameters: 1. object obj - the button object
                    2. EventArgs args - the Event's arguments

        Returns: nothing
        ***************************************************************/
        private void Button_Leave_Guild(object obj, EventArgs args)
        {
            rTextBox.Clear();

            Button leaveGuild = obj as Button;

            if (lsBoxGuilds.SelectedIndex != -1 && lsBoxPlayers.SelectedIndex != -1)
            {
                if (leaveGuild != null)
                {
                    //obtain the selected player and guild to leave
                    string[] playerTokens = lsBoxPlayers.SelectedItem.ToString().Split('\t');
                    string[] guildTokens = lsBoxGuilds.SelectedItem.ToString().Split('\t');

                    Player p = null;
                    Guild g = null;
                    foreach (Player pl in Global.SortedPlayers)
                    {
                        if (pl.Name == playerTokens[0].Trim())
                            p = pl; //save the player object  
                    }
                    foreach (Guild gu in Global.SortedGuilds)
                    {
                        if (gu.Gname == guildTokens[0].Trim() && gu.Server == guildTokens[1].Trim())
                            g = gu; //save the guild object
                    }

                    if (p.GuildId == 0) //player is not in a guild 
                    {
                        rTextBox.Text = String.Format("{0} cannot leave a guild because that user does not belong to one", p.Name);
                    }
                    else if (p.GuildId == g.Gid) //correct player and guild match
                    {
                        p.GuildId = 0;
                        rTextBox.Text = String.Format("{0} has left {1} {2}", p.Name, g.Gname, g.Server);
                    }
                    else //wrong guild selection
                    {
                        rTextBox.Text = String.Format("{0} is not a member of guild: {1} \t{2}", p.Name, g.Gname, g.Server);
                    }
                }
            }
            else
            {
                rTextBox.Text = "You must select both a player and their guild from the lists above.";
            }
        }

        /***************************************************************
        private void Button_Apply_Search(object obj, EventArgs args)

        Use: Filters both players and guilds in the list boxes by characters
        entered in the text boxes. If 0 - many characters match in sequence 
        starting from the beginning of the player name/ guild name then 
        those players/guilds will be returned. Otherwise an error
        message will be displayed in the output box.

        Parameters: 1. object obj - the button object
                    2. EventArgs args - the Event's arguments

        Returns: nothing
        ***************************************************************/

        private void Button_Apply_Search(object obj, EventArgs args)
        {
            rTextBox.Clear();
            int count = 0;
            bool found = true;

            Button applySearch = obj as Button;

            if (applySearch != null)
            {
                string name = txtSearchPlayer.Text;
                string server = txtSearchServer.Text;

                if (name == "" && server == "")
                {
                    UpdateLists();
                }

                if (name != "")
                {
                    count = 0;
                    //search by player name only
                    found = true;
                    foreach (Player p in Global.SortedPlayers)
                    {
                        //avoid segmentation faulting on the guild
                        if (name.Length > p.Name.Length)
                            continue;

                        for (int i = 0; i < name.Length; i++)
                        {
                            if (name[i] == p.Name[i])
                                count++;
                        }

                        if (count == name.Length) //found match 
                        {
                            if (found) //clear the first time we find a match
                                lsBoxPlayers.Items.Clear();

                            found = false;
                            lsBoxPlayers.Items.Add(String.Format("{0, -15}\t{1, -10} {2, -10} \n", p.Name, p.Role2, p.Level));
                        }

                        count = 0;
                    }

                    if (found) // none are found actually
                    {
                        lsBoxPlayers.Items.Clear();

                        foreach (Player p in Global.SortedPlayers) //add each player, role, and level to the list box
                        {
                            lsBoxPlayers.Items.Add(String.Format("{0, -15}\t{1, -10} {2, -10} \n", p.Name, p.Role2, p.Level));
                        }

                        rTextBox.Text = "Nothing was a match for your filtering criteria";
                    }
                }

                if (server != "")
                {
                    found = true;
                    foreach(Guild g in Global.SortedGuilds)
                    {  
                        //avoid segmentation faulting on the guild
                        if (server.Length > g.Server.Length-2)
                            continue;

                        for(int i = 0; i < server.Length; i++)
                        {
                            if (server[i] == g.Server[i+1])
                                count++;                           
                        }

                        if (count == server.Length) //found match 
                        {
                            if (found) //clear the first time
                                lsBoxGuilds.Items.Clear();

                            found = false;
                            lsBoxGuilds.Items.Add(String.Format("{0, -23}\t{1, -10} \n", g.Gname, g.Server));
                        }

                        count = 0;
                    }

                    if (found) // none are found actually
                    {
                        lsBoxGuilds.Items.Clear();

                        foreach (Guild g in Global.SortedGuilds)
                        {
                            lsBoxGuilds.Items.Add(String.Format("{0, -23}\t{1, -10} \n", g.Gname, g.Server));
                        }

                        rTextBox.Text = "Nothing was a match for your filtering criteria";
                    }
                }
             
           
            }
        }

        /***************************************************************
        private void cbClass_SelectedIndexChanged(object obj, EventArgs e)

        Use: Updates the selectable options for the Role once a Class
        option has been made. 

        Parameters: 1. object obj - the button object
                    2. EventArgs e - the Event's arguments

        Returns: nothing
        ***************************************************************/

        private void cbClass_SelectedIndexChanged(object obj, EventArgs e)
        {
            
            cbRole.Items.Clear(); //clear the drop down box 
            if (cbClass.SelectedIndex != -1)
            {
                switch(cbClass.SelectedIndex) //populate it with the correct corresponding selections
                {
                    case 0:
                        cbRole.Items.Add("Tank");
                        cbRole.Items.Add("DPS");
                        break;
                    case 1:
                        cbRole.Items.Add("DPS");
                        break;
                    case 2:
                        cbRole.Items.Add("Tank");
                        cbRole.Items.Add("Healer");
                        cbRole.Items.Add("DPS");
                        break;
                    case 3:
                        cbRole.Items.Add("Healer");
                        cbRole.Items.Add("DPS");
                        break;
                    case 4:
                        cbRole.Items.Add("DPS");
                        break;
                    case 5:
                        cbRole.Items.Add("DPS");
                        break;
                    case 6:
                        cbRole.Items.Add("Tank");
                        cbRole.Items.Add("Healer");
                        cbRole.Items.Add("DPS");
                        break;
                    case 7:
                        cbRole.Items.Add("DPS");
                        break;
                    case 8:
                        cbRole.Items.Add("Healer");
                        cbRole.Items.Add("DPS");
                        break;
                    default:
                        break;
                }
                cbRole.SelectedIndex = 0; //automatically select the first index in the list
            }
        }

        /***************************************************************
        private void Button_Add_Player(object obj, EventArgs args)

        Use: validates all fields have been selected and that there is not
        a duplicaite player name, then creates a player object with the
        entered values and generates a unique id for that player.
        Also repopulates player list box. 

        Parameters: 1. object obj - the button object
                    2. EventArgs args - the Event's arguments

        Returns: nothing
        ***************************************************************/

        private void Button_Add_Player(object  obj, EventArgs args)
        {
            Button addPlayer = obj as Button;
            bool playerNameExists = false;

            if (addPlayer != null)
            {
                foreach(Player pl in Global.SortedPlayers) //make sure the player doesnt already exist
                {
                    if (pl.Name == txtPlayerName.Text)
                        playerNameExists = true;
                }

                if (!playerNameExists)
                {
                    if (txtPlayerName.Text != "" && cbRace.SelectedIndex != -1 && cbRole.SelectedIndex != -1 && cbClass.SelectedIndex != -1)
                    {
                        //generate a unique id number
                        Random random = new Random();
                        int uid = random.Next(0, 99999999);
                        bool exists = false;
                        //make sure it is not already taken
                        while (!exists)
                        {
                            foreach (Player p in Global.SortedPlayers)
                            {
                                if (p.Id == uid)
                                    exists = true;
                            }

                            if (exists)
                            {
                                uid = random.Next(0, 99999999);
                                exists = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                        //create the player
                        Player myPlayer = new Player(Convert.ToUInt32(uid), txtPlayerName.Text, Convert.ToUInt32(cbRace.SelectedIndex), Convert.ToUInt32(cbClass.SelectedIndex), 0, 0, 0);
                        //add the player to the set
                        Global.SortedPlayers.Add(myPlayer);
                        //reprint the list
                        lsBoxPlayers.Items.Clear();
                        foreach (Player p2 in Global.SortedPlayers) //add each player, role, and level to the list box
                        {
                            lsBoxPlayers.Items.Add(String.Format("{0, -15}\t{1, -10} {2, -10} \n", p2.Name, p2.Role2, p2.Level));
                        }

                    }
                    else
                    {
                        rTextBox.Text = "You must enter a player name, race, role, and class.";
                    }
                }
                else
                {
                    rTextBox.Text = "That player name already exists.";
                }
                
            }
        }

        /***************************************************************
        private void Button_Add_Guild(object obj, EventArgs args)

        Use: Validates all fields are selected and that a guild name
        is not a duplicaite. Then creates a guild object and unique
        guild id. Also repopulates the guild list box. 

        Parameters: 1. object obj - the button object
                    2. EventArgs args - the Event's arguments

        Returns: nothing
        ***************************************************************/

        private void Button_Add_Guild(object obj, EventArgs args)
        {
            Button addGuild = obj as Button;
            bool guildNameExists = false;

            if (addGuild != null)
            {
                string serv = "[" + cbServer.Text + "]";

                foreach (Guild gu in Global.SortedGuilds)
                {
                    //check if the guild name on that server already exists 
                    if (gu.Gname == txtGuildName.Text && gu.Server == serv)
                        guildNameExists = true;
                }

                if (!guildNameExists)
                {
                    if (txtGuildName.Text != "" && cbServer.SelectedIndex != -1 && cbType.SelectedIndex != -1)
                    {
                        //generate a unique id number
                        Random random = new Random();
                        int uid = random.Next(0, 999999);
                        bool exists = false;
                        //make sure it is not already taken
                        while (!exists)
                        {
                            foreach (Guild g in Global.SortedGuilds)
                            {
                                if (g.Gid == uid)
                                    exists = true;
                            }

                            if (exists)
                            {
                                uid = random.Next(0, 999999);
                                exists = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                        //create the guild
                        string temp = "[" + cbServer.Text + "]";
                        Guild myGuild = new Guild(Convert.ToUInt32(uid), txtGuildName.Text, temp);
                        
                        //add the guild to the set
                        Global.SortedGuilds.Add(myGuild);
                        //reprint the list
                        lsBoxGuilds.Items.Clear();
                        foreach (Guild g2 in Global.SortedGuilds)
                        { 
                            lsBoxGuilds.Items.Add(String.Format("{0, -23}\t{1, -10} \n", g2.Gname, g2.Server));
                        }
                        rTextBox.Text = String.Format("{0}-{1} has been created!", myGuild.Gname, myGuild.Server);

                    }
                    else
                    {
                        rTextBox.Text = "You must enter a guild name, server, and type.";
                    }
                }
                else
                {
                    rTextBox.Text = "That guild name already exists on that server.";
                }

            }
        }

        /***************************************************************
        private void Box_Players_Index_Changed(object obj, EventArgs args)

        Use: Passive reaction for displaying the player and their associated
        guild in the output box when a user selected that player on screen. 

        Parameters: 1. object obj - the button object
                    2. EventArgs args - the Event's arguments

        Returns: nothing
        ***************************************************************/

        private void Box_Players_Index_Changed(object obj, EventArgs args)
        {
            if (lsBoxPlayers.SelectedIndex != -1)
            {
                //get the selected item from the list box 
                string[] tokens = lsBoxPlayers.SelectedItem.ToString().Split('\t');
               
                foreach (Player p in Global.SortedPlayers)
                {
                    if (p.Name == tokens[0].Trim()) //if we find the player name in our sorted set 
                    {
                        rTextBox.Text = p.ToString(); //display its information
                        break;
                    }
                        
                }
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

        Use: Default constructor reads in all three input files and assigns
        them to their corresponding Dictionary containers contained in a
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
                       Convert.ToUInt32(tokens[5]), Convert.ToUInt32(tokens[6]));

                    //add to set
                    Global.SortedPlayers.Add(myPlayer);

                    line = inFile.ReadLine();
                }
            }

            //read in the guilds text file
            using (StreamReader inFile = new StreamReader("..\\..\\guilds.txt"))
            {
                string[] tokens2;
                string addBrackets = "[";
                line = inFile.ReadLine();

                while (line != null)
                {
                    string rebuild = null; //to hold the rebuilded string
                    tokens = line.Split();//seperate the guild ID from the rest of the string

                    //now piece back the guild name since guilds can be more than one word 
                    //seperated by spaces 
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        if (i != tokens.Length - 1)
                            rebuild += tokens[i] + " ";
                        else
                            rebuild += tokens[i];
                    }
                    tokens2 = rebuild.Split('-'); //split the guild name from server 
                    addBrackets += tokens2[1] + "]"; //add the server and closing bracket
                    //create new Guild object
                    Guild myGuild = new Guild(Convert.ToUInt32(tokens[0]), tokens2[0], addBrackets);
                    Global.SortedGuilds.Add(myGuild);

                    line = inFile.ReadLine();
                    addBrackets = "["; //reset the bracket
                }
            }
        }

    }


    public class Guild : IComparable
    {
        readonly uint gid;
        readonly string gname;
        readonly string server;

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

        Use: Default constructor initializes attributes for the Player class
     
        Parameters: 
        uint g - guild id
        string n - guild name
        string s - server name
      
        Returns: nothing
        ***************************************************************/
        public Guild(uint g, string n, string s)
        {
            this.gid = g;
            this.gname = n;
            this.server = s;
        }

        /***************************************************************
        public () //default constructor #2

        Use: Default constructor initializes attributes for the Player class
     
        Parameters: 
      
        Returns: nothing
        ***************************************************************/
        public Guild()
        {
            this.gid = 0;
            this.gname = "";
            this.server = "";
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
            return String.Format("{0, -25} {1, -20} \n", gname, server);
        }


    }
    public class Player : IComparable
    {
        public enum Race { Orc, Troll, Tauren, Forsaken };
        public enum Role { Warrior, Mage, Druid, Priest, Warlock, Rogue, Paladin, Hunter, Shaman };
        readonly uint id;
        readonly string name;
        readonly Race race;
        readonly Role role;
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
            get { return role;  }
        }
        public uint Level
        {
            get { return level; }
            set {  level = value; }
        }

        public uint Exp
        {
            get { return exp; }
            set { exp = value;  }

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
        uint world - the player's logged in world
        uint level- the player's level
        uint exp - the player's exp
        uint guildId - the -player's guild id

        Returns: nothing
        ***************************************************************/
        public Player(uint id, string name, uint race, uint role, uint level, uint exp, uint guildID)
        {
            this.id = id;
            this.name = name;
            this.race = (Race)race;  
            this.role = (Role)role;            
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
            return String.Format("Name: {0, -15} Race: {1, -15} Level: {2, -15} Guild: {3, -15} {4, -15}\n", name, race, level, gname, gserver);
        }

    }



}
