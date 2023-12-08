using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    { 
        public MainPage()
        {
            InitializeComponent();
            CreateGrid(1, 1);        
        }

        private int numRows = 1;
        private int numColumns = 1;
        private List<List<string>> gridData = new List<List<string>>();
        private List<string> rowData = new List<string>();

        private void CreateGrid(int rows, int columns)
        {
            numRows = rows;
            numColumns = columns;

            for (int i = 0; i < numRows; i++)
            {
                MyGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }

            for (int i = 0; i < numColumns; i++)
            {
                MyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }
            
            for (int row = 0; row < numRows; row++)
            {
                List<string> rowData = new List<string>();

                for (int col = 0; col < numColumns; col++)
                {
                    Entry entry = new Entry { Placeholder = $"({(char)('A' + col)}{row + 1})" };
                    MyGrid.Children.Add(entry);
                    Grid.SetRow(entry, row);
                    Grid.SetColumn(entry, col);
                    rowData.Add("");
                }
                gridData.Add(rowData);
            }
        }

        private void DestroyGrid()
        {   
            MyGrid.RowDefinitions.Clear();
            MyGrid.ColumnDefinitions.Clear();
            MyGrid.Children.Clear();          
        }

        private void PopulateLists()
        {
            gridData.Clear();
            rowData.Clear();

            Output.Text = "";

            for (int row = 0; row < numRows; row++)
            {
                List<string> rowData = new List<string>();

                for (int col = 0; col < numColumns; col++)
                {
                    var entry = (Entry)MyGrid.Children[(row * numColumns) + col];
                    rowData.Add(entry.Text);
                }
                gridData.Add(rowData);
            }
        }

        private void PopulateCells(int popNumOfRows, int popNumOfColumns, bool isRemoveColumn)
        {   
            if (!isRemoveColumn)
            {
                for (int row = 0; row < popNumOfRows; row++)
                {
                    for (int col = 0; col < popNumOfColumns; col++)
                    {
                        var entry = (Entry)MyGrid.Children[(row * popNumOfColumns) + col];
                        entry.Text = gridData[row][col];
                    }
                }
            } 
            else
            {
                int iterator = 0;
                int outOfRangePreventer = 0;
                for (int row = 0; row < numRows; row++)
                {
                    if (outOfRangePreventer == numRows * numColumns)
                    {
                        break;
                    }
                    for (int col = 0; col < numColumns + 1; col++)
                    {
                        var entry = (Entry)MyGrid.Children[(row * numColumns) + col + iterator];
                        entry.Text = gridData[row][col];
                        if (col == numColumns)
                        {
                            iterator++;
                        }
                        outOfRangePreventer++;
                        if (outOfRangePreventer == numRows * numColumns)
                        {
                            break;
                        }                       
                    }
                }
                
            }
            
        }

        private void AddRow_Clicked(object sender, EventArgs e)
        {   
            if (numRows < 8)
            {   
                PopulateLists();
                numRows++;
                DestroyGrid();                
                CreateGrid(numRows, numColumns);
                PopulateCells(numRows - 1, numColumns, false);;
            }
            else if (numRows == 8)
            {
                Output.Text = "Максимальна кількість рядків";
            }
        }

        private void RemoveRow_Clicked(object sender, EventArgs e)
        {
            if (numRows > 1)
            {   
                PopulateLists();
                numRows--;
                DestroyGrid();
                CreateGrid(numRows, numColumns);
                PopulateCells(numRows, numColumns, false);              
            }
            else if (numRows == 1)
            {
                Output.Text = "Мінімальна кількість рядків";
            }
        }

        private void AddColumn_Clicked(object sender, EventArgs e)
        {
            if (numColumns < 16)
            {
                PopulateLists();
                numColumns++;
                DestroyGrid();               
                CreateGrid(numRows, numColumns);
                PopulateCells(numRows, numColumns - 1, false);
            }
            else if (numColumns == 16)
            {
                Output.Text = "Максимальна кількість стовбців";
            }

        }

        private void RemoveColumn_Clicked(object sender, EventArgs e)
        {
            if (numColumns > 1)
            {
                PopulateLists();
                numColumns--;
                DestroyGrid();             
                CreateGrid(numRows, numColumns);
                PopulateCells(numRows, numColumns, true);               
            }
            else if (numColumns == 1)
            {
                Output.Text = "Мінімальна кількість стовбців";
            }

        }

        private void Clear_Clicked(object sender, EventArgs e)
        {
            gridData.Clear();
            rowData.Clear();

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    var entry = (Entry)MyGrid.Children[row + (numRows * col)];
                    entry.Text = string.Empty;
                }
            }
            Output.Text = "Поля очищені!";
        }

        private void OutputError(bool condition)
        {
            if (condition == false)
            {
                Output.Text = "Помилка [неправльний символ у клітинках]: ";
            }
        }

        private bool IsOperatorInGrid(string checkedSymbol)
        {
            List<string> symbols = new List<string>()
            {
                "(", ")", "+", "-", "*", "/", "^", "=", "max", "min", "mod", "div", ""
            };
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i] == checkedSymbol)
                {
                    return true;
                }
            }
            return false;
        } 
        
        private bool CheckGridData()
        {
            Output.Text = "";
            bool errorSignal = false;
            for (int row = 0; row < gridData.Count; row++)
            {
                for (int col = 0; col < gridData[row].Count; col++)
                {
                    if (!int.TryParse(gridData[row][col], out int number) && (!IsOperatorInGrid(gridData[row][col])))
                    {
                        OutputError(errorSignal);
                        Output.Text += $"[{(char)('A' + col)}{row + 1}] ";
                        errorSignal = true;
                    }
                }
            }
            if (errorSignal == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private void CalculateExpression()
        {
            List<string> listOfGridData = new List<string>();

            for (int row = 0; row < gridData.Count; row++)
            {
                for (int col = 0; col < gridData[row].Count; col++)
                {
                    listOfGridData.Add(gridData[row][col]);
                }
            }

            List<string> expression = new List<string>();
            List<List<string>> expressions = new List<List<string>>();

            foreach (string element in listOfGridData)
            {
                if (element == "=")
                {
                    expression.Add(element);
                    expressions.Add(expression);
                    expression = new List<string>();               
                }
                else
                {
                    expression.Add(element);
                }
            }

            Stack<double> valueStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();


            Output.Text = "Результат:";
            foreach (List<string> single_expression in expressions)
            {
                foreach (string token in single_expression)
                {
                    if (IsNumeric(token))
                    {
                        double value = double.Parse(token);
                        valueStack.Push(value);
                    }
                    else if (token == "(")
                    {
                        operatorStack.Push(token);
                    }
                    else if (token == ")")
                    {
                        while (operatorStack.Peek() != "(")
                        {
                            string operation = operatorStack.Pop();
                            double b = valueStack.Pop();
                            double a = valueStack.Pop();
                            double result = PerformOperation(a, b, operation);
                            valueStack.Push(result);
                        }
                        operatorStack.Pop();
                    }
                    else if (IsOperator(token))
                    {
                        while ((operatorStack.Count > 0) && (operatorStack.Peek() != "(") && (Priority(token) <= Priority(operatorStack.Peek())))
                        {
                            string operation = operatorStack.Pop();
                            double b = valueStack.Pop();
                            double a = valueStack.Pop();
                            double result = PerformOperation(a, b, operation);
                            valueStack.Push(result);
                        }
                        operatorStack.Push(token);
                    }
                }

                while (operatorStack.Count > 0)
                {
                    string operation = operatorStack.Pop();
                    double b = valueStack.Pop();
                    double a = valueStack.Pop();
                    double result = PerformOperation(a, b, operation);
                    valueStack.Push(result);
                }

                string finalResult = "";
                while (valueStack.Count > 0)
                {
                    double value = valueStack.Pop();
                    finalResult = finalResult + " " + value;
                }

                Output.Text += finalResult;
            }           
        }
            

        private int Priority(string operation)
        {
            switch (operation)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "^":
                case "max":
                case "min":
                case "mod":
                case "div":
                    return 3;
                default:
                    throw new ArgumentException($"Unknown operation: {operation}");
            }
        }   

        private bool IsNumeric(string input)
        {
            return double.TryParse(input, out _);
        }

        private bool IsOperator(string input)
        {
            string[] operators = { "+", "-", "*", "/", "^", "max", "min", "mod", "div" };
            return operators.Contains(input);
        }

        private double PerformOperation(double a, double b, string operation)
        {
            switch (operation)
            {
                case "+":
                    return a + b;
                case "-":
                    return a - b;
                case "*":
                    return a * b;
                case "/":
                    return a / b;
                case "^":
                    return Math.Pow(a, b);
                case "max":
                    return Math.Max(a, b);
                case "min":
                    return Math.Min(a, b);
                case "mod":
                    return a % b;
                case "div":
                    return (int)a / (int)b;
                default:
                    throw new ArgumentException($"Unknown operation: {operation}");
            }
        }

        private void Execute_Clicked(object sender, EventArgs e)
        {
            PopulateLists();
            if (CheckGridData() == true)
            {
                CalculateExpression();
            }                    
        }   
    }
}