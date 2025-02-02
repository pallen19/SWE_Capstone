﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Capstone.Crawlers;
using Capstone.Models;
using System.Diagnostics;

namespace Capstone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isAmazonChecked { get; set; }
        public bool isEBayChecked { get; set; }
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>(); 
        public MainWindow()
        {
            InitializeComponent();
            RunAmazonCrawler();
            RunEBayCrawler();
            //Remove later-----
            isAmazonChecked = true;
            isEBayChecked = true;
            //-----------------
            DataContext = this;
        }
        
        // I guess the runners are empty for now?
        public void RunAmazonCrawler()
        {
            
        }
        public void RunEBayCrawler()
        {

        }
        private void SettingsButton_Click(object sender, KeyEventArgs e)
        {
            SettingsWindow w1 = new SettingsWindow();
            w1.RenderSize = new Size(this.ActualWidth, this.ActualHeight);
            w1.Height = w1.RenderSize.Height;
            w1.Width = w1.RenderSize.Width;
            w1.isAmazonChecked = this.isAmazonChecked;
            w1.isEBayChecked = this.isEBayChecked;
            
            w1.Show();
            this.Hide();
        }

        private void SearchTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                
                var searchText = ((TextBox) sender).Text;
                
                this.Dispatcher.Invoke(() =>
                {
                    List<Product> resultItems = new List<Product>(); // This stores results from each crawler.
                    if (isEBayChecked)
                    {
                        var eBayCrawler = new EbayCrawler();
                        var results = Task.Run(() => eBayCrawler.SearchProduct(searchText));
                        foreach (Product? product in results.Result)
                        {
                            resultItems.Add(product);
                        }
                    }
                    if (isAmazonChecked)
                    {
                        var amazonCrawler = new AmazonCrawler();
                        var results = Task.Run(() => amazonCrawler.SearchProduct(searchText));
                        foreach (Product? product in results.Result)
                        {
                            resultItems.Add(product);
                        }
                    }
                    //Sort the results,
                    resultItems = SortPriceDesc(resultItems);

                    //Print them to the screen.
                    Products.Clear();
                    foreach (Product product in resultItems)
                    {
                        Products.Add(product);
                    }
                });

            }
        }

        //Takes a list of products and sorts it by price in descending order. This can be used to sort results from any (?) crawler.
        private List<Product> SortPriceDesc(List<Product> products)
        {
            if (products == null || products.Count == 0)
            {
                return null;
            }
            List<Product> sortedProducts = new List<Product>();
            Product temp = new Product();
            char[] tempPriceArr;
            double temp1 = 0.0;
            double[] sortPriceArr = new double[products.Count * 2]; //sortPriceArr = [price0, index of price0 in products, ..., priceN, index of priceN in products]
            //Convert price values into decimal values
            for (int i = 0; i < products.Count; i++)
            {
                tempPriceArr = products[i].Price.ToCharArray();
                tempPriceArr = Array.FindAll<char>(tempPriceArr, (c => (char.IsDigit(c) || c == '.')));
                try
                {
                    temp1 = Double.Parse(new string(tempPriceArr));
                    sortPriceArr[2 * i] = temp1;
                    sortPriceArr[2 * i + 1] = i;
                }
                catch (Exception ex) { }
            }

            //Sort the decimal values, since initially the price index = index in the list
            int min = 0; // index of min val
            double temp2 = 0;


            for (int k = 0; k < sortPriceArr.Length; k += 2)
            {
                min = k + 1;

                for (int j = k; j < sortPriceArr.Length; j += 2)
                {
                    if (sortPriceArr[j] < sortPriceArr[min - 1])
                    {
                        min = j + 1;
                    }
                }
                //Add the product at the min index to the sorted products
                sortedProducts.Add(products[(int)sortPriceArr[min]]);
                temp1 = sortPriceArr[k];
                temp2 = sortPriceArr[k + 1];
                sortPriceArr[k] = sortPriceArr[min - 1];
                sortPriceArr[k + 1] = sortPriceArr[min];
                sortPriceArr[min - 1] = temp1;
                sortPriceArr[min] = temp2;
            }
            return sortedProducts;
        }

        private void SettingsButton_Click_1(object sender, RoutedEventArgs e)
        {
            SettingsWindow w1 = new SettingsWindow();
            //w1.NavPanel = this.NavPanel;
            w1.Height = this.ActualHeight;
            w1.Width = this.ActualWidth;

            w1.Top = this.Top;
            w1.Left = this.Left;

            w1.Show();
            this.Hide();
        }
    }
}
