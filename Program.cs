using System.Runtime.InteropServices;
using System.Globalization;
using System.Threading.Tasks.Dataflow;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Gurimranjot SIngh Gill
namespace monopoly_game
{



    class Program
    {


        static Random random = new();

        //define arrays
        public int[] no_value = { 4, 8, 16, 20 }; //blocks which have no valus
        public int[] trade_value = { 5, 10, 15, 20 }; //block which will have trade value
        public int[] specialbonus = new int[5];//block which will have special bonus 
        public int[] buy_prpperty = new int[10];//block which will have option to buy new property 



        public void Board() //method to make new blocks everytime a new gaem is played
        {
            for (int i = 0; i < 5; i++)
            {
                specialbonus[i] = random.Next(1, 21);
            }

            HashSet<int> uniqueValues = new HashSet<int>();

            for (int i = 0; i < 10; i++)
            {
                int newValue;

                // Generate a new random value until a unique one is found
                do
                {
                    newValue = random.Next(1, 21);
                } while (!uniqueValues.Add(newValue));

                // Assign the unique value to the array
                buy_prpperty[i] = newValue;
            }
        }



        //method to roll the dice
        public (int current_block, int buyPropertyCaller) Dice(int current_block, int buyPropertyCaller)
        {

            int computernum = random.Next(1, 7);
            int next_block = computernum + current_block;

            //to make the player go round after block 20
            if (next_block > 20)
            {
                next_block = next_block - 20;
                //to make the available property also start from the begining after block 20
                buyPropertyCaller = 0;
            }
            return (next_block, buyPropertyCaller);
        }

        //method to take loan
        public static (int income, int loan) loan(int income, int loan_amount)
        {
            bool valid = true;
            Console.WriteLine("How much loan would you like to take?");
            string? reply = Console.ReadLine();
            while (valid) //error checking
            {
                try
                {
                    if (int.TryParse(reply, out int amount))
                    {
                        // Parsing successful
                        income += amount;
                        loan_amount += amount;
                        Console.WriteLine($"Loan of {amount} added to your income.");
                        break;
                    }
                    else
                    {
                        throw new MainGame.MyCustomException("invalid input");
                    }

                }
                catch (MainGame.MyCustomException customEx)
                {
                    // Handle the exception
                    Console.WriteLine("Error: " + customEx.Message);
                    Console.WriteLine();
                }
            }
            return (income, loan_amount);
        }

        //method to reduc the loan amount
        public (int income, int loan) reduce_loan(int income, int loan_amount)
        {
            bool valid = true;
            Console.WriteLine("How much loan would you like to reduce?");
            string? reply = Console.ReadLine();
            int amount = 0;
            if (reply != null)
            {
                amount = int.Parse(reply);
            }
            while (valid) //error checking
            {
                try
                {
                    if (amount > income)
                    {
                        income -= amount;
                        loan_amount -= amount;
                    }
                    else
                    {
                        throw new MainGame.MyCustomException("You dont have enough money");

                    }
                }
                catch (MainGame.MyCustomException customEx)
                {
                    // Handle the exception
                    Console.WriteLine("Error: " + customEx.Message);
                    Console.WriteLine();
                }
            }
            return (income, loan_amount);

        }





    }

    //main classs
    class MainGame
    {
        //initializing class members
        static Random random = new();
        string player_name = string.Empty; //name of player
        private int playerBlock = 0; //the current block player is on
        private int loan = 0; //amount of loan player has
        private int income = 0; //amount of income player has

        // Public properties to access the private fields
        public int PlayerBlock
        {
            get { return playerBlock; }
            set { playerBlock = value; }
        }

        public int Loan
        {
            get { return loan; }
            set { loan = value; }
        }

        public int Income
        {
            get { return income; }
            set { income = value; }
        }
        List<int> owned_blocks = new List<int> { }; //list of owned blocks player has
        List<string> block_name = new List<string> { }; //kist of owned blocks names i.e. "small house" ets player has

        //dictionary for the property to buy
        private static Dictionary<string, int> propertyPrices = new Dictionary<string, int>
        {
            {"small house",250},
            {"office",300},
            {"gas station",650},
            {"playground",930},
            {"library",490},
            {"apartmemt building",600},
            {"empty yard",290},
            {"big house",900},
            {"garage",1050},
            {"small house 2",290},
        };

        private static List<string> propertyKeys = new List<string>(propertyPrices.Keys);
        private static List<string> name_property = propertyKeys.ToList(); //list of all the property name

        public class MyCustomException : Exception //custom class expection

        {
            public MyCustomException() { }
            public MyCustomException(string message) : base(message) { }
            public MyCustomException(string message, Exception innerException) : base(message, innerException) { }

        }
        private int buyPropertyCaller = 0; //variable to irritate the property dictionary



        public static void Main()
        {
            bool playing = true;
            bool new_game = true;
            bool correctchoice = true;
            string final_img = @"            

   /\                                                                         ██████                                    
   \ \                                                            ████████████░░░░░░██                                        
    \ \                                                         ██░░░░░░░░░░░░██░░░░░░██████ 
     \ \                                                      ██░░░░░░░░░░░░░░██░░░░░░██░░░░██     
     /  \                                                   ██░░░░░░░░░░░░░░░░░░██░░░░██░░░░░░██  
    /    \                                                  ██░░░░░░░░░░░░░░░░░░░░██░░██░░░░░░██    
    \ > { \                                                 ██░░░░░░░░░░░░░░░░░░░░██░░████████    
    /    b \                                              ██░░████░░░░░░░░░░░░░░░░██░░██   
    \    p /                                              ██░░░░░░████░░░░░░░░░░░░████     
    / > { /                                             ██░░░░░░░░░░░░████░░░░░░░░██    
    \    /                                              ██░░░░░░░░░░░░░░░░██░░░░██  
     \  /                                                 ██░░░░░░░░░░░░░░██████   
     / /                                                ██░░████████░░░░░░██  
    / /                                                 ██░░░░░░░░░░██████  
   / /                                                    ██░░░░░░░░░░██   
   \/                                                   ██░░██████████      
                                                        ██░░░░░░░░██   
                                                          ██░░░░██                                                      
                                                            ████  
";





            while (new_game) // to restart the game
            {
                Console.WriteLine("Welcom TO MiniPoly");
                Console.WriteLine("Select Your Player:");
                Console.WriteLine("1.Northrop Grumman B-2 Spirit AKA Stealth Bomber                        2.Croissant");
                Console.Write(final_img);

                //initializing objects
                MainGame player1 = new MainGame();
                MainGame ai = new MainGame();

                //setting default values
                ai.player_name = "Computer";
                ai.Income = 350;
                player1.Income = 350;
                while (correctchoice) //error handeling
                {
                    try
                    {
                        player1.player_name = Console.ReadLine() ?? string.Empty;
                        if (player1.player_name == "1")
                        {
                            _ = "Stealth Bomber";
                            correctchoice = false;
                        }
                        else if (player1.player_name == "2")
                        {
                            _ = "croissant";
                            correctchoice = false;
                        }
                        else
                        {
                            throw new MyCustomException("Invalid Choice");

                        }
                    }
                    catch (MyCustomException customEx)
                    {
                        // Handle the exception
                        Console.WriteLine("Error: " + customEx.Message);
                        Console.WriteLine();
                    }

                }

                //making a program class instace to access the variables and method in that class
                Program instance = new Program();
                //making new board 
                instance.Board();
                new_game = false; //stoping the gaem to restart unless needed

                //initializing varibales form Program class to access and modify 
                int[] no_value = instance.no_value;
                int[] specialbonus = instance.specialbonus;
                int[] buy_prpperty = instance.buy_prpperty;
                int[] trade_value = instance.trade_value;

                Array.Sort(buy_prpperty);//sorting the array

                Console.WriteLine("Press enter to start or menu to go to menu");
                while (playing) //error handeling and taking control to when exit the game
                {
                    try
                    {
                        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------");
                        string x = Console.ReadLine() ?? string.Empty;
                        //conditions when to exit the game

                        //maing game starts
                        if (string.IsNullOrEmpty(x))
                        {
                            //the computer is first
                            //computer rolls the dice
                            var res = instance.Dice(ai.PlayerBlock, ai.buyPropertyCaller);
                            ai.playerBlock = res.current_block;
                            ai.buyPropertyCaller = res.buyPropertyCaller;

                            //gicing information to the user
                            Console.WriteLine($"Opponent block: {ai.PlayerBlock}");
                            //ai logic starts
                            if (ai.loan - ai.income > 100) //reducing the computers loan when it has enought funds
                            {
                                int loan_amount = random.Next(ai.loan, ai.income);
                                ai.income -= loan_amount;
                                ai.loan -= loan_amount;
                                Console.WriteLine($"Your opponent just gave a loan payment for {loan_amount}");
                            }
                            if (player1.owned_blocks.Contains(ai.PlayerBlock) == true) //checking if computer is currently on the player's owned block
                            {
                                //if it is, taking the index of the block from players list
                                int index = player1.owned_blocks.IndexOf(ai.PlayerBlock);
                                //taking the name of the block from the players blockname list
                                string blockname = player1.block_name[index];
                                //taking the amount from playerPrices dictionary
                                int fine = propertyPrices[blockname];
                                //adding and subtracting the amount form player and computer's income
                                if (ai.income > fine)
                                {
                                    ai.income -= fine;
                                    player1.income += fine;
                                }
                                else if (ai.income < fine)
                                { //taking a loan
                                    int loanaiAmount = random.Next(fine - ai.income, fine + 200);
                                    Console.WriteLine($"Your opponent just took a loan for {loanaiAmount}");
                                    ai.income += loanaiAmount;
                                    ai.loan += loanaiAmount;
                                    ai.income -= fine;
                                    player1.income += fine;
                                }
                                //telling the user what happened
                                Console.WriteLine($"Your oppoenet Stepped on your land and is fined {fine}");
                            }




                            else if (specialbonus.Contains(ai.PlayerBlock)) //if computer lands on blocks which gives bonus
                            {
                                int bonus = random.Next(0, 350); //making random bonus 
                                //adding it to comouters income
                                ai.income += bonus;
                                //teklling user what happened
                                Console.WriteLine($"Your opp gained {bonus}");
                            }
                            else if (buy_prpperty.Contains(ai.PlayerBlock)) //checking if the computer lands on block with property 
                            {
                                if (player1.owned_blocks.Contains(ai.PlayerBlock) && !ai.owned_blocks.Contains(ai.PlayerBlock)) //checking if either player or computer owns the block already or no
                                {
                                    //if yes, skipping the block
                                    ai.buyPropertyCaller += 1;
                                }
                                else
                                {
                                    //if no, getting the place name from the name_property list using buyPropertyCaller
                                    string place_name = name_property[ai.buyPropertyCaller];
                                    //getting the money using the name form the dictionary
                                    int money = propertyPrices[place_name];
                                    //checking the finicial start of computer
                                    int ai_condition = ai.income - money;
                                    //making choices
                                    int ai_choice = random.Next(0, 2);

                                    //checking if the random is 0 and the computer have enought money to buy the thing

                                    //if the computer want to buy 
                                    if (ai_choice == 0)
                                    {
                                        if (ai_condition < 100)
                                        { //checking ig it has enough money
                                          //taking loan
                                            int loanaiAmount;
                                            loanaiAmount = random.Next(money, money + 100);

                                            Console.WriteLine($"Your opponent just took a loan for {loanaiAmount}");
                                            ai.income += loanaiAmount;
                                            ai.loan += loanaiAmount;
                                        }
                                        //computer buying the property with enought funds


                                        //telling user that computer just baught a place
                                        Console.WriteLine($"Your opponent just bought {place_name}");
                                        //adding the block to the computer owned block list
                                        ai.owned_blocks.Add(ai.PlayerBlock);
                                        //adding the block name  to the computer owned block name list
                                        ai.block_name.Add(place_name);
                                        //detecting the money from computer
                                        ai.income -= money;
                                        //telling user what happenmed
                                        Console.WriteLine($"Your opponent's total money is {ai.income}");
                                        //adding one to irritate over the dictionary next time 
                                        ai.buyPropertyCaller += 1;
                                    }

                                    else if (ai_choice == 1) //computer decided to not buy
                                    {
                                        Console.WriteLine($"Your opponent didnt baught {place_name}");
                                        ai.buyPropertyCaller += 1;
                                    }
                                }
                            }

                            //checkimg if computer landed on a trade block and it has something to tade with
                            else if (trade_value.Contains(ai.PlayerBlock) && ai.owned_blocks.Count > 0)
                            {
                                int lenght = ai.owned_blocks.Count; //knwoing how much bocks computer owns
                                                                    //knwoing how much blocks player owns
                                int opp_lenght = player1.owned_blocks.Count;
                                //getting a random betweeen 0 ,1 and 2
                                int ai_choice = random.Next(0, 3);
                                //selecting a random block to tarde 
                                int ai_choice_asset = random.Next(0, lenght);
                                //takign the name of block as both block_name and owned_blocks are prefectly syncronised
                                string solditemname = ai.block_name[ai_choice_asset];

                                if (ai_choice == 0 && opp_lenght > 0) //if the random is 0 and computer owns something
                                {

                                    int choice_ai_opptrade = random.Next(0, opp_lenght); //taking a random to trade form playre's list
                                                                                         //taking the name of that random woned block 
                                    string gainedblockname = player1.block_name[choice_ai_opptrade];
                                    //asking user if they want to trade the blocks or no
                                    Console.WriteLine($"Your oppoenet have made an offer to trade {solditemname} for {gainedblockname}");
                                    string reply2 = Console.ReadLine() ?? string.Empty;
                                    //if user replyed yes
                                    if (reply2 == "y")
                                    {
                                        //assigning the block which computer wants to trade to a variable
                                        int solditemT = ai.owned_blocks[ai_choice_asset];
                                        //assigning the block which player wants to trade to a variable
                                        int gainedblock = player1.owned_blocks[choice_ai_opptrade];

                                        //removing the block and blockname form computre's lists
                                        ai.owned_blocks.RemoveAt(ai_choice_asset);
                                        ai.block_name.RemoveAt(ai_choice_asset);

                                        //adding the gained block to computer's lists
                                        ai.owned_blocks.Add(gainedblock);
                                        ai.block_name.Add(gainedblockname);

                                        //removing the traded block from player's list
                                        player1.owned_blocks.RemoveAt(choice_ai_opptrade);
                                        player1.block_name.RemoveAt(choice_ai_opptrade);

                                        //adding the traded block to player's list
                                        player1.owned_blocks.Add(solditemT);
                                        player1.block_name.Add(solditemname);

                                        //telling player that the trade is done
                                        Console.WriteLine("Trade Done");
                                    }
                                    else //if user chose not to do the trade
                                    {
                                        Console.WriteLine("Trade not done");
                                    }
                                }


                                else if (ai_choice == 1) //if the random is 1, computer choose to sell a block
                                {
                                    int original_price = propertyPrices[solditemname]; //taking the price of the block
                                                                                       //taking random number for which the block will be sold
                                    int ai_offer = random.Next(original_price - 30, original_price + 200);

                                    if (ai_offer < original_price) //checking if that number is less then the original amount for the block
                                    {
                                        //adding the offer
                                        ai.income += ai_offer;
                                        //removing the block from computer's lisyts
                                        ai.owned_blocks.RemoveAt(ai_choice_asset);
                                        ai.block_name.RemoveAt(ai_choice_asset);

                                    }
                                    else //if the offer price is bigger then the original price
                                    {
                                        Console.WriteLine("deal not accpeted");
                                    }
                                }
                            }
                            if (ai.Loan > 750 || player1.Loan > 750 || ai.income > 2000 || player1.Income > 2000)
                            {
                                if (ai.Loan > 750 || player1.income > 2000) //specifing the reson given to user for ending the game
                                    Console.WriteLine("You Won");
                                else if (player1.Loan > 750 || ai.Income > 2000)
                                {
                                    Console.WriteLine("You lost");
                                }
                                Console.WriteLine("Thank you for playing");
                                playing = false;
                                break;
                            }


                            //COMPUTER LOGIC ENDS
                            Console.WriteLine("");


                            //Player turn
                            //player rolling dice
                            var res2 = instance.Dice(player1.PlayerBlock, player1.buyPropertyCaller);
                            player1.playerBlock = res2.current_block;
                            player1.buyPropertyCaller = res2.buyPropertyCaller;
                            //telling the player what block he is on

                            Console.WriteLine($"Your block: {player1.PlayerBlock}");
                            Console.WriteLine("");


                            //checking if player landed on computer oened blocks
                            if (ai.owned_blocks.Contains(player1.PlayerBlock) == true)
                            {
                                //taking the amount of the property
                                int index = ai.owned_blocks.IndexOf(player1.PlayerBlock);
                                string blockname = ai.block_name[index];
                                int fine = propertyPrices[blockname];
                                //fining player and adding the amount to computer's income
                                if (player1.income > fine)
                                {
                                    player1.Income = player1.Income - fine;
                                    ai.Income = ai.Income + fine;
                                    Console.WriteLine($"Oops! You Stepped on opponents land and you are fined {fine}");
                                }
                                else if (player1.income < fine)
                                { //if user dont have enought money , they take loan
                                    Console.WriteLine($"You are fined {fine}, but you dont have enoguht money to give off your loan");
                                    var result = Program.loan(player1.income, player1.loan);
                                    player1.income = result.income - fine;
                                    player1.loan = result.loan;
                                }
                                //telling user that happened

                                Console.WriteLine($"Your new balance is {player1.Income}");

                            }

                            else if (no_value.Contains(player1.PlayerBlock)) //checking if player landed on  an empty block
                            {

                            }
                            else if (specialbonus.Contains(player1.PlayerBlock)) //checking if player landed on a bonus block
                            {

                                int bonus = random.Next(0, 350); //generating random bonus
                                                                 //adding it to player's income
                                player1.Income += bonus;
                                //telling plsyer they won a bonus
                                Console.WriteLine($"You got a BONUS! of {bonus}");
                            }
                            //checking if player landed on property block 
                            else if (buy_prpperty.Contains(player1.PlayerBlock))
                            {
                                bool isValidInput = true; //for error handeling
                                                          //taking the property name
                                string place_name = name_property[player1.buyPropertyCaller];
                                //taking the property's value
                                int money = propertyPrices[place_name];


                                //checking if anyone owns the block
                                if (ai.owned_blocks.Contains(player1.PlayerBlock) && !player1.owned_blocks.Contains(player1.PlayerBlock))
                                { //if yes, move to next property
                                    player1.buyPropertyCaller = player1.buyPropertyCaller + 1;
                                }
                                //else
                                else if (!ai.owned_blocks.Contains(player1.PlayerBlock))
                                {

                                    while (isValidInput)
                                    {
                                        try
                                        { //telling user to buy or no
                                            Console.WriteLine($"would you like to buy {place_name} for {money} ?");
                                            Console.WriteLine("Press y or n");
                                            string? reply5 = Console.ReadLine();
                                            //if user said yes
                                            if (reply5 != null && reply5.ToLower() == "y")
                                            {  //checking if user have enough money
                                                if (player1.income < money)
                                                { //if not, taking a loan
                                                    Console.WriteLine("You dont have enought money to buy");
                                                    Console.WriteLine("Would you like to take a loan? enter y or n");
                                                    string? reply6 = Console.ReadLine();
                                                    if (reply6 == "y")
                                                    { //if user said yes ot loan, take him to the loan method
                                                        var result = Program.loan(player1.income, player1.loan);
                                                        player1.income = result.income;
                                                        player1.loan = result.loan;

                                                        continue; //looping agina

                                                    }
                                                    else if (reply6 == "n")
                                                    { //if user said no

                                                        player1.buyPropertyCaller = player1.buyPropertyCaller + 1; //skipping the block property
                                                        isValidInput = false;

                                                        break;//breaking the loop


                                                    }
                                                    else
                                                    { //error handeling
                                                        throw new MyCustomException("Invalid input");

                                                    }
                                                } //if player have enoguht money
                                                else if (player1.income > money)
                                                { //buy the block
                                                    Console.WriteLine($"Congrats you just baught a {place_name}");
                                                    //add block to player lists 
                                                    player1.owned_blocks.Add(player1.playerBlock);
                                                    player1.block_name.Add(place_name);
                                                    //detuct the income
                                                    player1.income = player1.income - money;
                                                    //tell player that he baught a place
                                                    Console.WriteLine($"Your total money is {player1.income}");
                                                    player1.buyPropertyCaller += 1;
                                                    isValidInput = false; //breaking the loop

                                                    break;
                                                }

                                            }
                                            else if (reply5 != null && reply5.ToLower() == "n") //user dont want to buy
                                            {
                                                break; //breaks the loop
                                            }
                                            else //error handeling
                                            {
                                                throw new MyCustomException("Please enter y or n only");
                                            }
                                            //skipping the property block 
                                        }
                                        catch (MyCustomException customEx)
                                        {
                                            // Handle the exception
                                            Console.WriteLine("Error: " + customEx.Message);
                                            Console.WriteLine();
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                        }


                                    }
                                }

                            } //checking of player landed on trade block and they have blocks to trade
                            else if (trade_value.Contains(player1.PlayerBlock) && player1.owned_blocks.Count > 0)
                            {
                                bool isvalidinputrade = true; //error hadeling
                                                              //asking user input
                                Console.WriteLine("Would you like to trade or sell any of your assets?");
                                Console.WriteLine("Press y or n");
                                //taking the lenght of player's and computers owned blocks
                                int lenght = player1.owned_blocks.Count;
                                int opp_lenght = ai.owned_blocks.Count;

                                while (isvalidinputrade)
                                {
                                    try
                                    {
                                        string? reply = Console.ReadLine();
                                        if (reply == "y") //if player replyed yes
                                        {
                                            Console.WriteLine("Select the number of an owned asset which you would like to sell or tade"); //asking user to input the property nthey want to trade/sell
                                            for (int i = 0; i < lenght; i++) //displaying all the property owned by player
                                            {
                                                Console.Write(i + ". ");
                                                Console.WriteLine(player1.block_name[i]);
                                            } //if user changes theor mind to not sell/trade
                                            Console.WriteLine("If not, enter 'n'");
                                            string chosen_asset = Console.ReadLine() ?? string.Empty;
                                            if (double.TryParse(chosen_asset, out double number)) //if user chose a number
                                            {
                                                if (int.TryParse(chosen_asset, out int num_asset)) //prasing the user input 
                                                { //asking user what they want to do with the selected property
                                                    Console.WriteLine("Would you like to sell or trade?");
                                                    Console.WriteLine("Enter t for trade and s for sell");
                                                    string? user_in = Console.ReadLine();
                                                    if (user_in == "t") //if user want to trade
                                                    {  //asking user which one of computers owned property they would like to trade
                                                        Console.WriteLine("Chose the number of the oppenents asset which you would like to trade");
                                                        if (opp_lenght > 1) //displaying the computer owned properties
                                                        {
                                                            while (true)//error handeling
                                                            {
                                                                for (int i = 0; i < opp_lenght; i++)
                                                                {
                                                                    Console.Write(i + ". ");
                                                                    Console.WriteLine(ai.block_name[i]);
                                                                } //taking the user input and converting it into int
                                                                int opp_trade = Convert.ToInt32(Console.ReadLine());
                                                                if (opp_lenght < opp_trade) //checking if user input is valid
                                                                {
                                                                    Console.WriteLine("Your opponent dosent contain this block");
                                                                    Console.WriteLine("Please selct a valid block");
                                                                    continue;
                                                                }
                                                                else if (opp_lenght > opp_trade) //if user choses the correct input
                                                                {
                                                                    //making sure user chose the correct number        
                                                                    Console.WriteLine($"Would you like to trade {player1.block_name[num_asset]} for opponent's  {ai.block_name[opp_trade]}");
                                                                    Console.WriteLine("Press y or n");
                                                                    string reply4 = Console.ReadLine() ?? string.Empty;
                                                                    while (true)
                                                                    {
                                                                        if (reply4 != null && reply4 == "y") //if user input is y
                                                                        {
                                                                            //making changes to user and computer's lists as necessary

                                                                            int solditemT = player1.owned_blocks[num_asset];
                                                                            string solditemname = player1.block_name[num_asset];

                                                                            int gainedblock = ai.owned_blocks[opp_trade];
                                                                            string gainedblockname = ai.block_name[opp_trade];

                                                                            player1.owned_blocks.RemoveAt(num_asset);
                                                                            player1.block_name.RemoveAt(num_asset);

                                                                            player1.owned_blocks.Add(gainedblock);
                                                                            player1.block_name.Add(gainedblockname);

                                                                            ai.owned_blocks.RemoveAt(opp_trade);
                                                                            ai.block_name.RemoveAt(opp_trade);

                                                                            ai.owned_blocks.Add(solditemT);
                                                                            ai.block_name.Add(solditemname);


                                                                            Console.WriteLine("Trade Done");
                                                                            isvalidinputrade = false;
                                                                            break;

                                                                        }
                                                                        else if (reply4 == "n") //if user dont want to trade
                                                                        {
                                                                            Console.WriteLine("Trade cancelled");
                                                                        }
                                                                        else //error handeling
                                                                        {
                                                                            Console.WriteLine("Please enter y or n only");
                                                                            continue;

                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Please enter valid input");
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                        else //if computer dont own anyting to trade 
                                                        {
                                                            Console.WriteLine("Your opponent do not have any thing to trade with");
                                                            break;
                                                        }


                                                    }
                                                    else if (user_in == "s") //if user want to sell
                                                    { //asking user the starting amount
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Enter the starting amount ");
                                                            string reply2 = Console.ReadLine() ?? string.Empty;
                                                            int sale_option = int.Parse(reply2); //changint it into int

                                                            if (sale_option < ai.income) //it it is less then the original amount for the property
                                                            {
                                                                Console.WriteLine("Deal completed"); //telling user deal is done
                                                                                                     //adding momney to players income
                                                                player1.income += sale_option;
                                                                //removing the property from players lists
                                                                player1.owned_blocks.RemoveAt(num_asset);
                                                                player1.block_name.RemoveAt(num_asset);
                                                                isvalidinputrade = false;//breaks the main loop
                                                                break; //breks the loop
                                                            }
                                                            else if (sale_option > ai.income) //else 
                                                            {
                                                                Console.WriteLine("deal not accpeted as the amount is way higer then the original amount");
                                                                continue; //loops agian 
                                                            }
                                                        }
                                                    }

                                                    else //error handeling
                                                    {
                                                        throw new MyCustomException("Please enter s or t only");
                                                    }
                                                }


                                            } //if user chose to not trade or sell anything
                                            else if (chosen_asset == "n")
                                            {
                                                break;
                                            }
                                            else if (lenght > int.Parse(chosen_asset)) //if player chose a block they dont won
                                            {
                                                throw new MyCustomException("You do not own this block");
                                            }
                                            else //error handeling
                                            {
                                                throw new MyCustomException("Please enter y or n only");
                                            }

                                        }
                                        else if (reply == "n")
                                        {
                                            Console.WriteLine("Deal cancelled");
                                            break;
                                        }
                                        else
                                        {
                                            throw new MyCustomException("Please enter y or n only");
                                        }
                                    }
                                    catch (MyCustomException customEx)
                                    {
                                        // Handle the exception
                                        Console.WriteLine("Error: " + customEx.Message);
                                        Console.WriteLine();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }

                                }
                            }
                            if (ai.Loan > 750 || player1.Loan > 750 || ai.income > 2000 || player1.Income > 2000)
                            {
                                if (ai.Loan > 750 || player1.income > 2000) //specifing the reson given to user for ending the game
                                    Console.WriteLine("You Won");
                                else if (player1.Loan > 750 || ai.Income > 2000)
                                {
                                    Console.WriteLine("You lost");
                                }
                                Console.WriteLine("Thank you for playing");
                                playing = false;
                                break;
                            }





                            //displaying the main variables after every turn
                            Console.WriteLine("");
                            Console.WriteLine($"Your income = {player1.Income}                                                                     computer income={ai.Income}");
                            Console.WriteLine($"Your loan   = {player1.Loan}                                                                       computer loan  ={ai.Loan}");
                            Console.WriteLine("");
                            Console.WriteLine("Press enter to start or menu to go to menu");

                        }
                        else if (x.ToLower() == "exit") //to end
                        {
                            break;
                        }
                        //if user wants to resuce loan
                        else if (x.ToLower() == "reduce loan")
                        {
                            var res = instance.reduce_loan(player1.income, player1.loan);
                            player1.income = res.income;
                            player1.loan = res.loan;

                        } //option to navigate to menu
                        else if (x.ToLower() == "menu")
                        {
                            Console.WriteLine("Exit                                                                     computer blocks");
                            Console.WriteLine("redo                                                                     owned blocks");
                        } //to restatrt the game
                        else if (x.ToLower() == "redo")
                        {
                            new_game = true;
                            correctchoice = true;
                            break;
                        } //to display a list of woned blocks
                        else if (x.ToLower() == "owned blocks")
                        {
                            if (player1.owned_blocks.Count() > 0)
                            {
                                for (int i = 0; i < player1.owned_blocks.Count(); i++)
                                    Console.WriteLine(player1.owned_blocks[i]);
                            }
                            else
                            {
                                Console.WriteLine("You dont own any blocks");
                            }
                        } //to display a list of computer owned blocks
                        else if (x.ToLower() == "computer blocks")
                        {
                            if (ai.owned_blocks.Count > 0)
                            {
                                for (int i = 0; i < ai.owned_blocks.Count(); i++)
                                    Console.WriteLine(ai.owned_blocks[i]);
                            }
                            else
                            {
                                Console.WriteLine("You dont own any blocks");
                            }
                        } //to display player's list
                        else if (x.ToLower() == "player list")
                        {
                            Console.WriteLine(ai.player_name);
                            Console.WriteLine(player1.player_name);
                        }

                        else //error handeling
                        {
                            Console.WriteLine("Please chose valid input");
                        }
                    }
                    catch (MyCustomException customEx)
                    {
                        // Handle the exception
                        Console.WriteLine("Error: " + customEx.Message);
                        Console.WriteLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }

            }
        }
    }
}


