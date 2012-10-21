using Okazuki.SH.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okazuki.SH.DataModel
{
    public class MainPageViewModel : BindableBase
    {
        private static readonly MainPageViewModel Default = new MainPageViewModel();

        public static MainPageViewModel GetDefault()
        {
            return Default;
        }

        public MainPageViewModel()
        {
            this.Height = Constraint.DefaultHeight.ToString();
            this.Calcurate();
        }

        private string height;

        public string Height
        {
            get { return this.height; }
            set 
            {
                if (this.SetProperty(ref this.height, value))
                {
                    this.OnPropertyChanged("HasError");
                    this.Calcurate();
                }
            }
        }

        public bool HasError
        {
            get
            {
                var dummy = default(int);
                return !int.TryParse(this.Height, out dummy);
            }
        }

        private SittingHeight sittingHeight;

        public SittingHeight SittingHeight
        {
            get { return this.sittingHeight; }
            set { this.SetProperty(ref this.sittingHeight, value); }
        }

        public void Calcurate()
        {
            if (this.HasError)
            {
                return;
            }

            this.SittingHeight = SittingHeightCalculator.Execute(int.Parse(height));
        }

    }
}
