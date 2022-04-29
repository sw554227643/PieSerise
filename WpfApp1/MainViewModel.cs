using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace WpfApp1
{
    public class MainViewModel : ViewModelBase
    {
        

        private ObservableCollection<PieSerise> pieSerises;

        public ObservableCollection<PieSerise> PieSerise
        {
            get { return pieSerises; }
            set { pieSerises = value;  }
        }


        public MainViewModel()
        {
            AddCommand = new RelayCommand(Add);
            DeleteCommand = new RelayCommand(Delete);

            PieSerise = new ObservableCollection<PieSerise>();
            PieSerise.Add(new PieSerise
            {
                Title = "Category#04",
                Percentage = 30,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5B9BD5")),
            });
            PieSerise.Add(
                new PieSerise
                {
                    Title = "Category#01",
                    Percentage = 250,
                    PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4472C4")),
                });

            PieSerise.Add(new PieSerise
            {
                Title = "Category#04",
                Percentage = 49,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5B9BD5")),
            });

            PieSerise.Add(new PieSerise
            {
                Title = "Category#02",
                Percentage = 50,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ED7D31")),
            });
            PieSerise.Add(new PieSerise
            {
                Title = "Category#03",
                Percentage = 30,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC000")),
            });

            PieSerise.Add(new PieSerise
            {
                Title = "Category#04",
                Percentage = 30,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5B9BD5")),
            });

        }

        private void Delete()
        {
            PieSerise.Remove(PieSerise[new Random().Next(0, pieSerises.Count - 1)]);

        }


        private void Add()
        {
            pieSerises.Add(new PieSerise
            {
                Title = "Category#02",
                Percentage = 50,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ED7D31")),
            });
        }

        public RelayCommand AddCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
    }
}
