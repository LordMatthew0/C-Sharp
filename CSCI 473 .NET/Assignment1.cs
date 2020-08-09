/* Corbin Lutsch & Matthew Lord
 *     Z1837389  & Z1848456
 *  CSCI - 473
 *  Due: 01/31/19
 *  Assignment 1 - Getting Comfortable 
 * 
 */

using System;
using System.IO; //for stream reader
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandc_Assign1
{
  
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Welcome to the World of ConflictCraft: Testing Environment!\n");
            Menu myMenu = new Menu();
            myMenu.PrintMenu();
            myMenu.GetOptions(); 
        }
    }

    public static class Global 
    {
        public static Dictionary<uint, Item> itemDictionary = new Dictionary<uint, Item>();
        public static Dictionary<uint, Player> playerDictionary = new Dictionary<uint, Player>();
        public static Dictionary<uint, string> guildDictionary = new Dictionary<uint, string>();
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
            //read the item input file
            using (StreamReader inFile = new StreamReader("..\\..\\items.txt"))
            {
                line = inFile.ReadLine();

                while (line != null)
                {
                    //split the string read in
                    tokens = line.Split('\t');

                    //create an item object
                    Item myItem = new Item(Convert.ToUInt32(tokens[0]), tokens[1], Convert.ToUInt32(tokens[2]),
                       Convert.ToUInt32(tokens[3]), Convert.ToUInt32(tokens[4]), Convert.ToUInt32(tokens[5]),
                       Convert.ToUInt32(tokens[6]), tokens[7]);

                    Global.itemDictionary.Add(Convert.ToUInt32(tokens[0]), myItem);

                    line = inFile.ReadLine();
                }
            }

            //read the players input file
            using (StreamReader inFile = new StreamReader("..\\..\\players.txt"))
            {
                line = inFile.ReadLine();

                while (line != null)
                {
                    tokens = line.Split(); //again, splitting the string up 

                    //tokens must be converted to the proper value for each attribute in the Player class
                    Player myPlayer = new Player(Convert.ToUInt32(tokens[0]), tokens[1], Convert.ToUInt32(tokens[2]), Convert.ToUInt32(tokens[3]), Convert.ToUInt32(tokens[4]),
                       Convert.ToUInt32(tokens[5]), Convert.ToUInt32(tokens[6]), Convert.ToUInt32(tokens[7]), Convert.ToUInt32(tokens[8]), Convert.ToUInt32(tokens[9]),
                       Convert.ToUInt32(tokens[10]), Convert.ToUInt32(tokens[11]), Convert.ToUInt32(tokens[12]), Convert.ToUInt32(tokens[13]),
                       Convert.ToUInt32(tokens[14]), Convert.ToUInt32(tokens[15]), Convert.ToUInt32(tokens[16]), Convert.ToUInt32(tokens[17]),
                       Convert.ToUInt32(tokens[18]), Convert.ToUInt32(tokens[19]));

                    Global.playerDictionary.Add(Convert.ToUInt32(tokens[0]), myPlayer);

                    line = inFile.ReadLine();
                }
            }

            //read in the guilds text file
            using (StreamReader inFile = new StreamReader("..\\..\\guilds.txt"))
            {
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

                    Global.guildDictionary.Add(Convert.ToUInt32(tokens[0]), rebuild);
                    line = inFile.ReadLine();
                }
            }
        }

        /***************************************************************
        public void PrintMenu()

        Use: Prints the testing menu
     
        Parameters: none
			
        Returns: nothing
        ***************************************************************/
        public void PrintMenu()
        {
            Console.WriteLine("\nWelcome to World of ConflictCraft: Testing Environment. Please select an option from the list below: ");
            Console.WriteLine("\t1.) Print All Players");
            Console.WriteLine("\t2.) Print All Guilds");
            Console.WriteLine("\t3.) Print All Gear");
            Console.WriteLine("\t4.) Print Gear List for Player");
            Console.WriteLine("\t5.) Leave Guild");
            Console.WriteLine("\t6.) Join Guild");
            Console.WriteLine("\t7.) Equip Gear");
            Console.WriteLine("\t8.) Unequip Gear");
            Console.WriteLine("\t9.) Award Experience");
            Console.WriteLine("\t10.) Quit");
        }

        /***************************************************************
        public GetOptions()

        Use: Gets input from the user and uses a switch statement to decide
        what to do with the input. 
     
        Parameters: none
			
        Returns: nothing
        ***************************************************************/
        public void GetOptions()
        {  
            line = Console.ReadLine();

            while (line != "10" && line != "q" && line != "Q" && line != "quit" && line != "Quit" && line != "exit" && line != "Exit")
            {
                switch (line)
                {
                    case "1": //Print All Players

                        foreach (KeyValuePair<uint, Player> obj in Global.playerDictionary)
                        {                        
                            Console.Write(obj.Value); //call to the Players override ToString() method

                            GetGuild(obj.Value);//prints the guild associated with the player
                        }
                        break;

                    case "2": //Print All Guilds

                        foreach (KeyValuePair<uint, string> obj in Global.guildDictionary)
                        {
                            Console.WriteLine(obj.Value); 
                        }
                        break;

                    case "3": //Print All Gear

                        foreach (KeyValuePair<uint, Item> obj in Global.itemDictionary)
                        {
                            Console.Write(obj.Value); //calls Items override ToString() method
                        }
                        break;

                    case "4": //Print gear for a specific player 

                        Console.Write("Enter the player name: ");
                        line = Console.ReadLine();

                        //LINQ query to find the associated player object
                        var findKey = from K in Global.playerDictionary where K.Value.Name == line select K.Key;

                        //Check that the player exists
                        if (!findKey.Any())
                            Console.WriteLine("'{0}' is not a valid player name.", line);

                        foreach (uint key in findKey)
                        {
                            Global.playerDictionary.TryGetValue(key, out Player foundPlayer); //get the player object associated with the name
                            Console.Write(foundPlayer); //print the player's information                           
                            GetGuild(foundPlayer); //including the guild they're in

                            for (int i = 0; i < foundPlayer.gear.Length; i++)
                            { 
                                Global.itemDictionary.TryGetValue(foundPlayer.gear[i], out Item foundItem); //find each Item object that the player is wearing 
                                if (foundPlayer.gear[i] == 0) //if the player is not weilding anything
                                {
                                    if (i == 11) //rings are indexed as spot 10 in enum
                                        Console.WriteLine("Ring: empty");
                                    else if (i == 12)//trinkets are indexed as spot 11 in enum
                                        Console.WriteLine("Trinket: empty");
                                    else if (i == 13) //trinkets are indexed as spot 11 in enum
                                        Console.WriteLine("Trinket: empty");
                                    else//otherwise we have the right type cast for our enumerator 
                                        Console.WriteLine("{0}: empty", (Item.ItemType)i);
                                }
                                else
                                {
                                    Console.Write(foundItem); //print the item's information
                                }
                            }
                        }
                        break;

                    case "5": //Leave Guild

                        Console.Write("Enter the player name: ");
                        line = Console.ReadLine();

                        //LINQ query to find the associated player object
                        var findKey2 = from K in Global.playerDictionary where K.Value.Name == line select K.Key;

                        //Check that the player exists
                        if (!findKey2.Any())
                            Console.WriteLine("'{0}' is not a valid player name.", line);

                        foreach (uint key in findKey2)
                        {
                            Global.playerDictionary.TryGetValue(key, out Player foundPlayer2); //get the player object

                            foundPlayer2.GuildId = 0; //set the guildId to 0 which means "no guild" in this context
                            Console.WriteLine("{0} has left their Guild.", foundPlayer2.Name);
                        }

                        break;

                    case "6": //Join Guild

                        Console.Write("Enter the player name: ");
                        line = Console.ReadLine();

                        //LINQ query to find the associated Player object
                        var findKey3 = from K in Global.playerDictionary where K.Value.Name == line select K.Key;

                        //check that the player exists
                        if (!findKey3.Any())
                            Console.WriteLine("'{0}' is not a valid player name.", line);

                        foreach (uint key in findKey3)
                        {
                            Global.playerDictionary.TryGetValue(key, out Player foundPlayer3); //get the player object

                            Console.Write("Enter the Guild they will join: ");
                            line = Console.ReadLine();

                            //LINQ query to find the guild key associated with the name of the guild to join
                            var findGuildKey = from K in Global.guildDictionary where K.Value == line select K.Key;

                            //check that the guild to join exists
                            if (!findGuildKey.Any())
                                Console.WriteLine("'{0}' is not a valid guild name.", line);

                            foreach (uint key2 in findGuildKey)
                            {
                                foundPlayer3.GuildId = key2; //set the new guildId for that player
                                Console.WriteLine("{0} has joined {1}!", foundPlayer3.Name, line);
                            }

                        }
                        break;

                    case "7": //Equip Item

                        Console.Write("Enter the player name: ");
                        line = Console.ReadLine();

                        //LINQ query to find the associated Player object
                        var findKey4 = from K in Global.playerDictionary where K.Value.Name == line select K.Key;

                        //check that the player exists
                        if (!findKey4.Any())
                            Console.WriteLine("'{0}' is not a valid player name.", line);

                        foreach (uint key in findKey4)
                        { //gets the Player object for the person entered
                            Global.playerDictionary.TryGetValue(key, out Player foundPlayer4);

                            Console.Write("Enter the item name they will equip: ");
                            line = Console.ReadLine();

                            //run query to find the corresponding Item object
                            var findItem = from K in Global.itemDictionary where K.Value.Name == line select K.Key;

                            //check that the item to equip exists
                            if (!findItem.Any())
                                Console.WriteLine("'{0}' is not a valid item name.", line);

                            foreach (uint key2 in findItem)
                            {                             
                                try
                                {
                                    foundPlayer4.EquipGear(key2); //equip the item
                                }
                                catch (ItemLevelException e)
                                {
                                    Console.WriteLine(e);
                                }         
                            }
                        }
                        break;

                    case "8": //Unequip gear

                        Console.Write("Enter the player name: ");
                        line = Console.ReadLine();

                        //LINQ query to find the associated guild key for the player name
                        var findKey5 = from K in Global.playerDictionary where K.Value.Name == line select K.Key;

                        //check that the player exists
                        if (!findKey5.Any())
                            Console.WriteLine("'{0}' is not a valid player name.", line);

                        foreach (uint key in findKey5)
                        {
                            Global.playerDictionary.TryGetValue(key, out Player foundPlayer5); //get the player object

                            Console.WriteLine("Enter the item slot number they will unequip: ");
                            Console.WriteLine("0 = Helmet");
                            Console.WriteLine("1 = Neck");
                            Console.WriteLine("2 = Shoulders");
                            Console.WriteLine("3 = Back");
                            Console.WriteLine("4 = Chest");
                            Console.WriteLine("5 = Wrist");
                            Console.WriteLine("6 = Gloves");
                            Console.WriteLine("7 = Belt");
                            Console.WriteLine("8 = Pants");
                            Console.WriteLine("9 = Boots");
                            Console.WriteLine("10 = Ring");
                            Console.WriteLine("11 = Trinket");

                            line = Console.ReadLine();

                            try
                            {
                                foundPlayer5.UnequipGear(Convert.ToInt32(line)); //unequip the item 
                            }
                            catch (InventoryFullException e)
                            {
                                Console.WriteLine(e);
                            }
                           
                        }
                        break;

                    case "9": //Award experience

                        Console.Write("Enter the player name: ");
                        line = Console.ReadLine();

                        //LINQ query to find the associated guild key for the player name
                        var findKey6 = from K in Global.playerDictionary where K.Value.Name == line select K.Key;

                        //check that the player exists
                        if (!findKey6.Any())
                            Console.WriteLine("'{0}' is not a valid player name.", line);

                        foreach (uint key in findKey6)
                        {
                            Global.playerDictionary.TryGetValue(key, out Player foundPlayer6);//get the player object

                            Console.Write("Enter the amount of experience to award: ");
                            line = Console.ReadLine();
                            foundPlayer6.Exp += Convert.ToUInt32(line); //add the experience 
                        }
                        break;

                    case "T": //Sort the Players and Items "hidden option"

                        SortedSet<Player> SortedPlayers = new SortedSet<Player>();
                        SortedSet<Item> SortedItems = new SortedSet<Item>();

                        foreach(KeyValuePair<uint, Player> p in Global.playerDictionary)
                        {
                            SortedPlayers.Add(p.Value); //add each player object to the sorted set
                        }

                        foreach (KeyValuePair<uint, Item> i in Global.itemDictionary)
                        {
                            SortedItems.Add(i.Value); //add each item object to the sorted set
                        }
                        foreach(Item i in SortedItems)
                        {
                            Console.Write(i); //uses the items ToString method
                        }
                        foreach(Player p in SortedPlayers)
                        {
                            Console.Write(p); //uses the players ToString method
                            GetGuild(p);
                        }
                        break;

                 default:
                        Console.WriteLine("Invalid choice, try again.");
                        break; 
                } //end of switch statement

                PrintMenu(); //reprint the menu
                line = Console.ReadLine(); //get user input
            }//end of while loop
        }

        /***************************************************************
        public void GetGuild(Player obj)

        Use: Takes in a player object and finds the guild name associated
        with that player object, then prints the guild they are in,
        if they aren't in a guild it prints a new line.
     
        Parameters: 1. Player obj - the object of the Player to find the guild for
			
        Returns: nothing
        ***************************************************************/
        public void GetGuild(Player obj)
        {
            //find the corresponding guild name
            Global.guildDictionary.TryGetValue(obj.GuildId, out string gname);

            if (obj.GuildId != 0) //if the player is in a guild
            {
                Console.Write("Guild: ");
                Console.WriteLine(gname);
            }
            else
                Console.Write("\n");
        }


    }

    public class Item : IComparable
    {
        public enum ItemType
        {
            Helmet, Neck, Shoulders, Back, Chest,
            Wrist, Gloves, Belt, Pants, Boots,
            Ring, Trinket
        };

       private static uint MAX_LEVEL = 60;
       static uint MAX_ILLVL = 360;
       private static uint MAX_PRIMARY = 200;
       private static uint MAX_STAMINA = 275;


        private readonly uint id;
        private string name;
        private ItemType type;
        private uint ilvl;
        private uint primary;
        private uint stamina;
        private uint requirement;
        private string flavor;

        public uint Id //this is my public property
        {
            get { return id;  }
        }

        public string Name //this is my public property
        {
            get { return name; }
            set { name = value; }
        }

        public ItemType Type //this is my public property
        {
            get { return type; }

            set
            {
                if ((int)value >= 0 && (int)value <= 11)
                    type = value;
                else
                    type = 0;
            }
        }

        public uint Ilvl //this is my public property
        {
            get { return ilvl; }

            set
            {
                if (value >= 0 && value <= MAX_ILLVL)
                    ilvl = value;
                else
                    ilvl = 0;
            }
        }

        public uint Primary //this is my public property
        {
            get { return primary; }
            set
            {
                if (value >= 0 && value <= MAX_PRIMARY)
                    primary = value;
                else
                    primary = 0;
            }
        }

        public uint Stamina //this is my public property
        {
            get { return stamina; }
            set
            {
                if (value >= 0 && value <= MAX_STAMINA)
                    stamina = value;
                else
                    stamina = 0;
            }
        }

        public uint Requirement //this is my public property
        {
            get { return requirement; }
            set
            {
                if (value >= 0 && value <= MAX_LEVEL)
                    requirement = value;
                else
                    requirement = 0;
            }
        }

        public string Flavor //this is my public property
        {
            get { return flavor; }
            set { flavor = value; }
        }


        /***************************************************************
        public Item() //default constructor #1

        Use: Default constructor sets all fields to zero or null.
     
        Parameters: none
			
        Returns: nothing
        ***************************************************************/
        public Item()
        {
            id = 0;
            name = "";
            type = 0;
            ilvl = 0;
            primary = 0;
            stamina = 0;
            requirement = 0;
            flavor = "";
        }

        /***************************************************************
        public Item(uint i, string n, uint ty, uint ilv, uint prim, uint stam,
        uint req, string flav) //default constructor #2

        Use: Default constructor initializes attributes for the Item class.
     
        Parameters: 
        uint i - the item id
        string n - the string name
        uint ty - the item type 
        uint ilv - the item level
        uint prim - the item primary stat
        uint stam - the item's stamina stat
        uint req - the item's requirement
        string flav - the item's flavor

        Returns: nothing
        ***************************************************************/
        public Item(uint i, string n, uint ty, uint ilv, uint prim, uint stam,
        uint req, string flav) 
        {
            id = i;
            name = n;
            type = (ItemType)ty;
            ilvl = ilv;
            primary = prim;
            stamina = stam;
            requirement = req;
            flavor = flav;
        }

        /***************************************************************
        public int CompareTo(object obj)

        Use: Compares two Item objects
     
        Parameters: 1. object obj - the object on the "right side of the operand"
       

        Returns: an integer when obj == null, otherwise recursively calls
        for further comparison 
        ***************************************************************/
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Item rightObj = obj as Item;

            if (rightObj != null)
                return name.CompareTo(rightObj.name);
            else
                throw new ArgumentException("[Item]:CompareTo argument is not an Item");
        }

      
        /***************************************************************
        public override string ToString()

        Use: Overrides the ToString() method for the Item class
     
        Parameters: none

        Returns: nothing
        ***************************************************************/
        public override string ToString()
        {
            return String.Format("({0}) {1} |{2}| --{3}--\n\t\"{4}\"\n", type, name, ilvl, requirement, flavor);

        }
    }

    public class Player : IComparable
    {
        public bool equiped = true; //to determine which ring/trinket to equip next
        public bool equiped2 = true;

        public enum Race { Orc, Troll, Tauren, Forsaken };

        private static uint MAX_LEVEL = 60;
        private static uint GEAR_SLOTS = 14;
        private static uint MAX_INVENTORY_SIZE = 20;

        readonly uint id;
        readonly string name;
        readonly Race race;
        uint level;
        uint exp;
        uint guildID;
        public uint[] gear;
        List<uint> inventory;

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

        public uint Level
        {
            get { return level; }
            set
            {
                if (value >= 0 && value <= MAX_LEVEL)
                    level = value;
                else
                    level = 0;
            }
        }

        public uint Exp
        {
            get { return exp; }
            set
            {
                exp += value;
                //if exp exceeds required experience for this player
                //to increase their level but not exceed MAX_LEVEl
                while (exp >= (level * 1000) && level != MAX_LEVEL)
                {
                    LevelUp();
                }
            }
        }

        public uint GuildId
        {
            get { return guildID;  }
            set { guildID = value; }
        }

        public uint this[int index] //this is a public indexer for gear
        {
            get { return gear[index]; }
            set { gear[index] = value; }
        }

        /***************************************************************
        public void LevelUp()

        Use: Levels up the player by subtracting their exp from level*1000
        and increasing their level
     
        Parameters: none

        Returns: nothing
        ***************************************************************/
        public void LevelUp()
        {
                exp = exp - (level * 1000);
                level++;
                Console.WriteLine("Ding!");
            
        }

        /***************************************************************
        public Player(uint id, string name, uint race, uint level,
        uint exp, uint guildID, params uint[] gear) //default constructor #1

        Use: Default constructor initializes attributes for the Player class
     
        Parameters: 
        uint id - the item id
        string name - the string name of the player
        uint race - the player's race 
        uint level- the player's level
        uint exp - the player's exp
        uint guildId - the -player's guild id
        params uint[] gear - the gear the player is weilding

        Returns: nothing
        ***************************************************************/
        public Player(uint id, string name, uint race, uint level, uint exp, uint guildID, params uint[] gear)
        {
            this.id = id;
            this.name = name;
            this.race = (Race)race;
            this.level = level;
            this.exp = exp;
            this.guildID = guildID;
            this.gear = new uint[14];
            for (int i = 0; i < gear.Length; i++)
                this.gear[i] = gear[i];
            this.inventory = new List<uint>();

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
            this.level = 0;
            this.exp = 0;
            this.guildID = 0;
            this.gear = new uint[14];
            for (uint i = 0; i < GEAR_SLOTS; i++) 
                this.gear[i] = 0;

            this.inventory = new List<uint>();
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
        public void EquipGear(uint newGearID)

        Use: Equips an item for a Player object 
     
        Parameters: 1. uint newGearID - the item id to equip

        Returns: nothing
        ***************************************************************/
        public void EquipGear(uint newGearID)
        {
            //LINQ to find the item object we will equip
            var findItem = from K in Global.itemDictionary where K.Key == newGearID select K.Value;

            foreach (Item item in findItem) //the item to equip
            {
                if (item.Requirement > Level)
                {
                    throw new ItemLevelException("You do not have the required level to equip this item");
                }
                else
                {
                    int save = 0;
                    int itemsFound = 0;

                    for (int i = 0; i < gear.Length; i++)//loop through each gear element
                    {
                        if (gear[i] != 0) //don't waste time searching for an empty item 
                        {
                            //Find the corresponding Item object                              
                            Global.itemDictionary.TryGetValue(gear[i], out Item item2);

                            //if item to equip is equal to the same item spot already equipped
                            //e.g. Helmet == Helmet, or Ring == Ring, then we know this is the correct spot to put it
                            if (item.Type == item2.Type)
                            {
                                save = i;
                                itemsFound++; //keep track of how many times we encounter the item
                            }
                        }
                    }

                    //if two of the same items are already equipped and it's a trinket
                    if (itemsFound == 2 && equiped && (int)item.Type == 11) //and we have already equipped once before
                    {
                        gear[save-1] = item.Id; //put it in the lower index
                        equiped = false;
                    }
                    else if (itemsFound == 2 && !equiped && (int)item.Type == 11)//two of the same items, trinket, havent equipped
                    {
                        gear[save] = item.Id; //put it in the upper index
                        equiped = true;
                    }
                    else if (itemsFound == 2 && equiped2 && (int)item.Type == 10) //same thing but with the ring location
                    {
                        gear[save-1] = item.Id; //put it in the lower
                        equiped2 = false;
                    }
                    else if (itemsFound == 2 && !equiped2 && (int)item.Type == 10)
                    {
                        gear[save] = item.Id; //put it in the higher index
                        equiped2 = true;
                    }
                    else if (itemsFound == 1 && ((int)item.Type == 10))//if only 1 ring currently equipped
                    {
                        if (save == 10) //10 is occupied, put it in 1 above it
                        {
                            gear[11] = item.Id; 
                        }
                        else //11 is occupied put it in 1 below it
                        {
                            gear[10] = item.Id; //put it in spot 11
                        }
                    }
                    else if(itemsFound == 1 && (int)item.Type == 11)
                    {
                        if (save == 12) //if save is gear spot 12 and empty put in lower index
                        {
                            gear[13] = item.Id; //put in lower index
                        }
                        else //the lower index is occupied
                        {
                            gear[12] = item.Id; //put in upper index
                        }
                    }
                    else if (itemsFound == 0 && ((int)item.Type == 11))
                    {
                        gear[(int)item.Type + 1] = item.Id; //put it in gear slot 12
                    }
                    else //the spot is open
                    {
                        gear[(int)item.Type] = item.Id; 
                    }

                    Console.WriteLine("{0} successfully equipped {1}!", Name, item.Name);
                }

            }
        }

        /***************************************************************
        public void UnequipGear(int gearSlot)

        Use: Unequips gear for a Player object
     
        Parameters: int gearSlot - the slot on the Player that will be unequipped

        Returns: nothing
        ***************************************************************/
        public void UnequipGear(int gearSlot)
        {

            if (gearSlot <= 11 && gearSlot >= 0)
            {
                if (inventory.Count >= MAX_INVENTORY_SIZE) //is the inventory full?
                {
                    //throw new System.ArgumentException("Inventory is full");
                    throw new InventoryFullException("Inventory is full");
                }
                else
                {
                    if (gearSlot != 10 && gearSlot != 11) //gear is not a ring or trinket
                    {
                        if (gear[gearSlot] == 0)//if it's empty
                        {
                            Console.WriteLine("That spot is already empty");
                        }
                        else
                        {
                            inventory.Add(gear[gearSlot]); //add the item to the inventory                      
                            gear[gearSlot] = 0; //set the gear slot to empty
                            Console.WriteLine("Successfully unequipped item.");
                        }
                    }
                    else
                    {
                        if (gearSlot == 10 && gear[gearSlot] != 0) //if it is a ring and slot is not empty
                        {
                            //unequip
                            inventory.Add(gear[gearSlot]); //add the item to the inventory                      
                            gear[gearSlot] = 0; //set the gear slot to empty
                            Console.WriteLine("Successfully unequipped item.");
                        }
                        else if (gearSlot == 10 && gear[gearSlot + 1] != 0)
                        {
                            //unequip
                            inventory.Add(gear[gearSlot+1]); //add the item to the inventory                      
                            gear[gearSlot+1] = 0; //set the gear slot to empty
                            Console.WriteLine("Successfully unequipped item.");
                        }
                        else if (gearSlot == 11 && gear[gearSlot + 1] != 0)
                        {
                            //unequip
                            inventory.Add(gear[gearSlot+1]); //add the item to the inventory                      
                            gear[gearSlot+1] = 0; //set the gear slot to empty
                            Console.WriteLine("Successfully unequipped item.");
                        }
                        else if (gearSlot == 11 && gear[gearSlot + 2] != 0)
                        {
                            //unequip
                            inventory.Add(gear[gearSlot+2]); //add the item to the inventory                      
                            gear[gearSlot+2] = 0; //set the gear slot to empty
                            Console.WriteLine("Successfully unequipped item.");
                        }                
                        else
                        {
                            Console.WriteLine("That spot is already empty.");
                        }
                    }
                }           
            }
            else
            {
                Console.WriteLine("'{0}' is an invalid item slot", gearSlot);
            }
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
            return String.Format("Name: {0, -10} \t Race: {1, -8}  Level: {2} \t", name, race, level);

        }
    }

    //Exception class to handle when a player's Inventory is full 
    public class InventoryFullException : Exception
    {
        public InventoryFullException() //default constructor #1
        { }

        public InventoryFullException(string message) : base(message) //default constructor #2 which passes the message to the
        { } //Exception's default constructor class

    }

    //Exception class to handle when an item's level is higher than the player's required level to equip it
    public class ItemLevelException : Exception
    {
        public ItemLevelException() //default constructor #1
        { }
   
        public ItemLevelException(string message) : base(message)//default constructor #2 which passes the message to the
        { } //Exception's default constructor class
    }
}
