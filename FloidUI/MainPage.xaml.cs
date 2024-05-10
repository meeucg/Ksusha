
using System.Reflection;
using FloidContract;
using Microsoft.Maui.Controls;

namespace FloidUI
{
    public partial class MainPage : ContentPage
    {
        bool assemblyStatus;
        private Assembly assembly;
        private EntriesGrid currentGrid;
        private int[,] currentMatrixValues;

        public void FloidAlgorithm(int[,] matrix, out int[,] matrixOut)
        {
            int vertexAmount = matrix.GetLength(0);

            Parallel.For(0, vertexAmount, k =>
            {
                Parallel.For(0, vertexAmount, i =>
                {
                    Parallel.For(0, vertexAmount, j =>
                    {
                        if (matrix[i, k] != 999 && matrix[k, j] != 999 &&
                            matrix[i, k] + matrix[k, j] < matrix[i, j])
                        {
                            matrix[i, j] = matrix[i, k] + matrix[k, j];
                        }
                    });
                });
            });
            matrixOut = matrix;
        }

        public Task<int[,]> ReadMatrixfromEntriesGridAsync(EntriesGrid eg)
        {
            int size = eg.GetSize;
            var temp = new int[size,size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    temp[i, j] = ReadEntrySafely(eg.GetEntries[i,j]);
                }
            }
            return temp;
        }

        public int ReadEntrySafely(Entry entry)
        {
            try
            {
                return int.Parse(entry.Text);
            }
            catch 
            {
                return 1;
            }
        }

        public EventHandler GenerateMatrixButtonClicked(Entry readFrom)
        {
            return async (object? sender, EventArgs e) =>
            {
                int size = ReadEntrySafely(readFrom);
                if (sender is Button n)
                {
                    n.IsEnabled = false;
                    functionalStack.Children[0] = (await Task<Grid>.Run(() =>
                    {
                        currentGrid = new EntriesGrid(size, 500);
                        return currentGrid.GetGrid;
                    }));
                    n.IsEnabled = true;
                }
            };
        }

        public EventHandler AssemblyButtonClicked(Entry readFrom)
        { 
            return (object? sender, EventArgs e) =>
                {
                    string path = readFrom.Text;
                    InstallAssemblyAsync(path);
                };
        }

        private async void InstallAssemblyAsync(string path)
        {
            try 
            { 
                UploadNewDll(path);
            }
            catch
            {
                await DisplayAlert("Invalid Path", "Change the DLL path", "OK");
                return;
            }

            try
            {
                CheckForContract(assembly);
                if (!assemblyStatus)
                {
                    throw new Exception();
                }
            }
            catch
            {
                /*var alert = new Task(() => DisplayAlert("Alert", "You have been alerted", "OK"));
                alert.Start();
                await alert;*/
                await DisplayAlert("Contract not realized", "Try another realization DLL", "OK");
            }
        }

        private void UploadNewDll(string path)
        {
            Assembly asm = Assembly.LoadFrom(path);
            assembly = asm;
            //CheckForContract(asm);
        }

        private void CheckForContract(Assembly asm)
        {
            Type[] types = asm.GetTypes();

            bool hasImplementation = types.Any(t => typeof(FloidContract.Algorithm).IsAssignableFrom(t) && t.IsClass);

            assemblyStatus = hasImplementation;
        }

        public MainPage()
        {
            InitializeComponent();

            EntriesGrid eg = new(10,500);
            currentGrid = eg;
            functionalStack.Add(eg.GetGrid);

            Button runFloyd = new Button()
            {
                HeightRequest = 50,
                WidthRequest = 150,
                Text = "Run Floyd",
                Margin = new Thickness(0, 25, 0, 0),
                BackgroundColor = Colors.Black,
                TextColor = Colors.White
            };

            functionalStack.Add(runFloyd);

            loadDllButton.Clicked += AssemblyButtonClicked(dllEntry);

            generateMatrixButton.Clicked += GenerateMatrixButtonClicked(numberOfCellsEntry);

            runFloyd.Clicked += async (object? sender, EventArgs e) => 
            {
                runFloyd.IsEnabled = false;
                currentMatrixValues = await ReadMatrixfromEntriesGridAsync(currentGrid);
                FloidAlgorithm(currentMatrixValues, out currentMatrixValues);
                for (int i = 0; i < currentGrid.GetSize; i++)
                {
                    for (int j = 0; j < currentGrid.GetSize; j++)
                    {
                        currentGrid.GetEntries[i,j].Text = currentMatrixValues[i,j].ToString();
                    }
                }
                runFloyd.IsEnabled = true;
            };

            //int[,] matrix = new int[5,5];
        }
    }

}
