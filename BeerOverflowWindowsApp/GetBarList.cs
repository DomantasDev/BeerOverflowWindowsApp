﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BeerOverflowWindowsApp
{
    public partial class GetBarList : Form
    {
        public GetBarList()
        {
            InitializeComponent();
            latitudeBox.Text = "54.684815";
            longitudeBox.Text = "25.288464";
            radiusTextBox.Text = "500";
        }

        private async void Go_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                GetBarListGoogle barListGoogle = new GetBarListGoogle();
                GetBarListFourSquare barListFourSquare = new GetBarListFourSquare();
                var result = await barListGoogle.GetBarsAroundAsync(GetLatitude(), GetLongitude() , GetRadius());
                result = combineLists(result, await barListFourSquare.GetBarsAroundAsync(GetLatitude(), GetLongitude(), GetRadius()));
                DisplayData(result);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Something went wrong with the message: " + exception.Message);
            }
        }

        private List<Bar> combineLists(List<Bar> primaryList, List<Bar> secondaryList)
        {
            var length = secondaryList.ToArray().Length;
            for (int i = 0; i < length; i++)
            {
                if (!primaryList.Contains(secondaryList[i]))
                {
                    primaryList.Add(secondaryList[i]);
                }
            }
            return primaryList;
        }


        // Clears the display first, then adds text to display
        private void DisplayData(List<Bar> resultData)
        {
            resultTextBox.Clear();
            foreach (var result in resultData)
            {
                resultTextBox.AppendText(result.GetName());
                resultTextBox.AppendText(Environment.NewLine);
            }
        }

        private string GetLatitude ()
        {
            return latitudeBox.Text;
        }

        private string GetLongitude ()
        {
            return longitudeBox.Text;
        }

        private string GetRadius ()
        {
            return radiusTextBox.Text;
        }
    }
}
