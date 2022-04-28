using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace WpfApp1
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        private SeriseList<PieSerise> pieSerises;

        public SeriseList<PieSerise> PieSerise
        {
            get { return pieSerises; }
            set { pieSerises = value; OnPropertyRaised(nameof(PieSerise)); }
        }


        public MainViewModel()
        {
            PieSerise = new SeriseList<PieSerise>();
            pieSerises.Add(new PieSerise
            {
                Title = "Category#04",
                Percentage = 30,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5B9BD5")),
            });
            pieSerises.Add(
                new PieSerise
                {
                    Title = "Category#01",
                    Percentage = 250,
                    PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4472C4")),
                });

            pieSerises.Add(new PieSerise
            {
                Title = "Category#04",
                Percentage = 3,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5B9BD5")),
            });

            pieSerises.Add(new PieSerise
            {
                Title = "Category#02",
                Percentage = 50,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ED7D31")),
            });
            pieSerises.Add(new PieSerise
            {
                Title = "Category#03",
                Percentage = 30,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC000")),
            });

            pieSerises.Add(new PieSerise
            {
                Title = "Category#04",
                Percentage = 30,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5B9BD5")),
            });

        }



    }
}
