using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CoffeeLPT.View;
using SbsSW.SwiPlCs;


namespace CoffeeLPT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            //WindowLunchCoffee lunchCoffee=new WindowLunchCoffee();
            //lunchCoffee.Show();
            StringBuilder st=new StringBuilder();
            PlQuery q = new PlQuery("member(A, [a,b,c])");
            foreach (PlTermV s in q.Solutions)
                st.AppendLine(s[0].ToString());
            int x = 0;
        }
    }
}
