
namespace Lab2Solution
{
    public partial class MainPage : ContentPage
    {

        /**
        * Name: Duaa Ahmad
        * Description: Crossword entries listed out using a bit.io database
        * Date: 10/9/2022
        * Bugs: Both sorts remove tne date from the listview
        */
        public MainPage()
        {
            InitializeComponent();
            EntriesLV.ItemsSource = MauiProgram.ibl.GetEntries();
        }

        /// <summary>
        /// Adds an entry to the list
        /// This also could have been a property
        /// </summary>
        /// <returns>ObservableCollection of entrties</returns>
        void AddEntry(System.Object sender, System.EventArgs e)
        {
            String clue = clueENT.Text;
            String answer = answerENT.Text;
            String date = dateENT.Text;

            int difficulty;
            bool validDifficulty = int.TryParse(difficultyENT.Text, out difficulty);

            if (validDifficulty)
            {
                InvalidFieldError invalidFieldError = MauiProgram.ibl.AddEntry(clue, answer, difficulty, date);
                if(invalidFieldError != InvalidFieldError.NoError)
                {
                    DisplayAlert("An error has occurred while adding an entry", $"{invalidFieldError}", "OK");
                }
            }
            else
            {
                DisplayAlert("Difficulty", $"Please enter a valid number", "OK");
            }
        }

        /// <summary>
        /// Represents all entries sorted by clue
        /// This also could have been a property
        /// </summary>
        /// <returns>ObservableCollection of entrties</returns>
        void SortByClue(System.Object sender, System.EventArgs e)
        {
            MauiProgram.ibl.SortByClue();
        }

        /// <summary>
        /// Represents all entries sorted by answer
        /// This also could have been a property
        /// </summary>
        /// <returns>ObservableCollection of entrties</returns>
        void SortByAnswer(System.Object sender, System.EventArgs e)
        {
            EntriesLV.ItemsSource = MauiProgram.ibl.SortByAnswer();
            EntriesLV.ItemsSource = MauiProgram.ibl.GetEntries();
        }

        /// <summary>
        /// Deletes an entry based on selected entru
        /// This also could have been a property
        /// </summary>
        /// <returns>ObservableCollection of entrties</returns>
        void DeleteEntry(System.Object sender, System.EventArgs e)
        {
            Entry selectedEntry = EntriesLV.SelectedItem as Entry;
            EntryDeletionError entryDeletionError = MauiProgram.ibl.DeleteEntry(selectedEntry.Id);
            if(entryDeletionError != EntryDeletionError.NoError)
            {
                DisplayAlert("An error has occurred while deleting an entry", $"{entryDeletionError}", "OK");
            }
        }

        /// <summary>
        /// Edits an entry based on selected entry
        /// This also could have been a property
        /// </summary>
        /// <returns>ObservableCollection of entrties</returns>
        void EditEntry(System.Object sender, System.EventArgs e)
        {

            Entry selectedEntry = EntriesLV.SelectedItem as Entry;
            selectedEntry.Clue = clueENT.Text;
            selectedEntry.Answer = answerENT.Text;
            selectedEntry.Date = dateENT.Text;


            int difficulty;
            bool validDifficulty = int.TryParse(difficultyENT.Text, out difficulty);
            if (validDifficulty)
            {
                selectedEntry.Difficulty = difficulty;
                Console.WriteLine($"Difficuilt is {selectedEntry.Difficulty}");
                EntryEditError entryEditError = MauiProgram.ibl.EditEntry(selectedEntry.Clue, selectedEntry.Answer, selectedEntry.Difficulty, selectedEntry.Date, selectedEntry.Id);
                if(entryEditError != EntryEditError.NoError)
                {
                    DisplayAlert("An error has occurred while editing an entry", $"{entryEditError}", "OK");
                }
            }


        }

        /// <summary>
        /// Fills in textbox with selected entries to corresponding entry box
        /// This also could have been a property
        /// </summary>
        /// <returns>ObservableCollection of entrties</returns>
        void EntriesLV_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
        {
            Entry selectedEntry = e.SelectedItem as Entry;
            clueENT.Text = selectedEntry.Clue;
            answerENT.Text = selectedEntry.Answer;
            difficultyENT.Text = selectedEntry.Difficulty.ToString();
            dateENT.Text = selectedEntry.Date;

        }




    }
}

