using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GameStore
{
    public class GameStoreContext : DbContext
    {
        public GameStoreContext() : base("GameStoreEntities")
        {
        }

        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Console> Consoles { get; set; }
    }
    public class Console
    {
        [Key]
        public int ConsoleID { get; set; }

        public string ConsoleName { get; set; }
        public string Manufacturer { get; set; }
        public int ReleaseYear { get; set; }
    }
    public class Seller
    {
        [Key]
        public int SellerID { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Salary { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    public class Game
    {
        [Key]
        public int GameId { get; set; }

        public string GameTitle { get; set; }
        public int ReleaseYear { get; set; }
        public decimal Price { get; set; }
        public int ConsoleID { get; set; }

        [ForeignKey("ConsoleID")]
        public Console Console { get; set; }
      
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private Seller _selectedSeller;
        private Game _selectedGame;
        private string _newSellerFirstName;
        private string _newSellerLastName;
        private decimal _newSellerSalary;

        public ObservableCollection<Seller> Sellers { get; set; }
        public ObservableCollection<Game> Games { get; set; }
        public ObservableCollection<Console> Consoles { get; set; }

        public ICommand AddSellerCommand { get; set; }
        public ICommand DeleteSellerCommand { get; set; }
        public ICommand AddGameCommand { get; set; }
        public ICommand DeleteGameCommand { get; set; }

        public Seller SelectedSeller
        {
            get { return _selectedSeller; }
            set
            {
                _selectedSeller = value;
                if (value != null)
                {
                    NewSellerFirstName = value.FirstName;
                    NewSellerLastName = value.LastName;
                    NewSellerSalary = value.Salary;
                }
                OnPropertyChanged(nameof(SelectedSeller));
            }
        }
        
        public Game SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                OnPropertyChanged(nameof(SelectedGame));
            }
        }
        
        public MainViewModel()
        {
            using (var context = new GameStoreContext())
            {
                Sellers = new ObservableCollection<Seller>(context.Sellers.ToList());
                Games = new ObservableCollection<Game>(context.Games.Include(g => g.Console).ToList());
                Consoles = new ObservableCollection<Console>(context.Consoles.ToList());
            }

            AddSellerCommand = new RelayCommand(AddSeller);
            DeleteSellerCommand = new RelayCommand(DeleteSeller, CanDeleteSeller);
            AddGameCommand = new RelayCommand(AddGame);
            DeleteGameCommand = new RelayCommand(DeleteGame, CanDeleteGame);
        }

       
        public string NewSellerFirstName
        {
            get { return _newSellerFirstName; }
            set
            {
                _newSellerFirstName = value;
                OnPropertyChanged(nameof(NewSellerFirstName));
            }
        }

        
        public string NewSellerLastName
        {
            get { return _newSellerLastName; }
            set
            {
                _newSellerLastName = value;
                OnPropertyChanged(nameof(NewSellerLastName));
            }
        }

        
        public decimal NewSellerSalary
        {
            get { return _newSellerSalary; }
            set
            {
                _newSellerSalary = value;
                OnPropertyChanged(nameof(NewSellerSalary));
            }
        }

        private void AddSeller(object parameter)
        {
            
            Seller newSeller = new Seller
            {
                FirstName = NewSellerFirstName,
                LastName = NewSellerLastName,
                Salary = NewSellerSalary
            };

           
            using (var context = new GameStoreContext())
            {
                context.Sellers.Add(newSeller);
                context.SaveChanges();
            }

         
            Sellers.Add(newSeller);

           
            NewSellerFirstName = "";
            NewSellerLastName = "";
            NewSellerSalary = 0;
        }


        private void DeleteSeller(object parameter)
        {
            if (SelectedSeller != null)
            {
                using (var context = new GameStoreContext())
                {
                    
                    context.Entry(SelectedSeller).State = EntityState.Deleted;
                    context.SaveChanges();

                   
                    Sellers.Remove(SelectedSeller);

                    NewSellerFirstName = "";
                    NewSellerLastName = "";
                    NewSellerSalary = 0;
                }
            }
        }

        private bool CanDeleteSeller(object parameter)
        {
            return SelectedSeller != null;
        }
        private string _newGameTitle;
        public string NewGameTitle
        {
            get { return _newGameTitle; }
            set
            {
                _newGameTitle = value;
                OnPropertyChanged(nameof(NewGameTitle));
            }
        }

        private int _newGameReleaseYear;
        public int NewGameReleaseYear
        {
            get { return _newGameReleaseYear; }
            set
            {
                _newGameReleaseYear = value;
                OnPropertyChanged(nameof(NewGameReleaseYear));
            }
        }

        private decimal _newGamePrice;
        public decimal NewGamePrice
        {
            get { return _newGamePrice; }
            set
            {
                _newGamePrice = value;
                OnPropertyChanged(nameof(NewGamePrice));
            }
        }

        private Console _selectedConsole;
        public Console SelectedConsole
        {
            get { return _selectedConsole; }
            set
            {
                _selectedConsole = value;
                OnPropertyChanged(nameof(SelectedConsole));
            }
        }

        private void AddGame(object parameter)
        {
            
            Game newGame = new Game
            {
                GameTitle = NewGameTitle,
                ReleaseYear = NewGameReleaseYear,
                Price = NewGamePrice,
                
                Console = SelectedConsole
            };

            
            using (var context = new GameStoreContext())
            {
                context.Games.Add(newGame);
                context.SaveChanges();
            }

            
            Games.Add(newGame);

            
            NewGameTitle = "";
            NewGameReleaseYear = 0;
            NewGamePrice = 0;
            SelectedConsole = null;
        }

        private void DeleteGame(object parameter)
        {
            if (SelectedGame != null)
            {
                using (var context = new GameStoreContext())
                {
                    
                    context.Entry(SelectedGame).State = EntityState.Deleted;
                    context.SaveChanges();

                    
                    Games.Remove(SelectedGame);
                }
            }
        }

        private bool CanDeleteGame(object parameter)
        {
            return SelectedGame != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}